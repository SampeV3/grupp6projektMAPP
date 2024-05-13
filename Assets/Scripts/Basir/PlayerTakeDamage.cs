using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using static PlayerTakeDamage;
public class PlayerTakeDamage : MonoBehaviour, IDataPersistance
{
    //public int health;
    
    [SerializeField] private Transform spawnTransform;
    private Vector3 spawnPosition;
    public double takeDamageGraceDuration = 0.4;
    public int currentHealth;
    public int maxHealth = 5;
    public GameObject[] healthBarSprites = new GameObject[6];

    [FormerlySerializedAs("OnPermaDeath")] [Description("Fires upon permanent death.")]
    public UnityEvent onPermaDeath;
    [Description("Event when the players actions should be frozen.")]
    public UnityEvent onFreezeActions; 
    
    
    
    public delegate void PermaDeathAction(); //metod signatur f�r subscribers till eventet
    public static event PermaDeathAction OnPermaDeathAction;

    //Events hj�lper till att decoupla koden och h�lla saker mer separerade ifr�n varandra.
    public delegate void RespawnAction(PlayerTakeDamage playerTakeDamage); //metod signatur f�r subscribers till eventet
    public static event RespawnAction OnRespawn;
    
    
    public delegate void TakeDamageAction(PlayerTakeDamage playerTakeDamage, int damageTaken); //metod signatur f�r subscribers till eventet
    public static event TakeDamageAction OnTakeDamage; //hur eventet avfyras fr�n detta script.

    public delegate void CombatSituationChanged(bool isInCombat, string combatSituation);
    public static event CombatSituationChanged OnCombatSituationChanged;

    
    
    public delegate void DoRespawnAction();

    public static event DoRespawnAction ExecuteDoRespawn;
    
    
    public delegate void PlayerKilledByAction(PlayerTakeDamage playerTakeDamage, EnemyData enemyData, GameObject enemyKiller);
    public static event PlayerKilledByAction OnKilledBy;
    private bool playerDied = false;
    public bool IsInCombat = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        if (spawnTransform == null)
        {
            spawnPosition = transform.position;
        }
        
        StartCoroutine(CheckIfInCombatWhileLoop());
    }
    
    private bool runCombatLoop = true;
    private IEnumerator CheckIfInCombatWhileLoop()
    {
        
        while (runCombatLoop)
        {
            var isChased = IsPlayer.GetIsPlayerInCombat();
            if (isChased != IsInCombat)
            {
                if (OnCombatSituationChanged != null)
                {
                    OnCombatSituationChanged(isChased, "chase"); //fire event delegate
                }
            }

            if (isChased)
            {
                OnEnemyEncounter(IsPlayer.GetEnemiesPlayerIsInCombatWith()); //Nemesis encounter check
            }
            
            IsInCombat = isChased;
            yield return new WaitForSeconds(1); // Delay for float second
        }
    }
    
    public void UpdateHealthBar()
    {
        for (int i = 0; i <= maxHealth; i++)
        {
            GameObject batterySprite = healthBarSprites[i];
            bool isEqualToIndex = i == currentHealth;
            //kan förenklas till: batterySprite.SetActive(isEqualToIndex);
            
            if (isEqualToIndex)
            {
                batterySprite.SetActive(true);
            } else
            {
                batterySprite.SetActive(false);
            }
        }
    }

    private void RepositionToStartLocation()
    {
        transform.position = spawnTransform ? spawnTransform.position : spawnPosition;
        playerDied = false;
        currentHealth = maxHealth;
        UpdateHealthBar();

    }
    
    public void DoRespawn()
    {
        
        if (OnRespawn != null) OnRespawn(this); //trigga eventet s� att andra script kan lyssna.


        onPermaDeath.Invoke();
        //Spela upp player death animation? effekter? ljud? delay?
        //lägg till scripts till eventet. 

        if (OnPermaDeathAction != null)
        {
            Invoke(nameof(RegenerateDungeon), 0.1f); //add extra delay...
        }
        else
        {
            RepositionToStartLocation();
        }
    }

    private void RegenerateDungeon()
    {
        OnPermaDeathAction();
    }

    private bool TakeDamageGrace = false;
    private void ResetGrace()
    {
        TakeDamageGrace = false;
    }
    
    void TakeDamage(int damageAmount, Collider2D other)
    {

        if (TakeDamageGrace)
        {
            return;
        }

        TakeDamageGrace = true;
        float graceDuration = (float)this.takeDamageGraceDuration * (float) UIController.GetSkillModifier("Health Grace Duration");
        print("Health grace duration: " + graceDuration);
        Invoke(nameof(ResetGrace), graceDuration);
        
        
        if (currentHealth < 0)
        {
            if (OnTakeDamage != null)
                OnTakeDamage(this, damageAmount); //object reference not set to an instance of a object?
        }
        currentHealth -= damageAmount;
        UpdateHealthBar();
        
        if (currentHealth <= 0 && !playerDied)
        {
            playerDied = true;
            if (Nemesis.NemesisController.nemesisEnabled && other.gameObject.GetComponent<BulletID>() != null)
            {
                onFreezeActions.Invoke();
                EnemyKilledPlayer(other.gameObject.GetComponent<BulletID>());
                
                //How about we tell this script to respawn the player
                //when there is a cutscene event that fires!!
                //then the player can choose when to exit the nemesis scene :D
                
                
            }
            else
            {
                print((other.gameObject.GetComponent<BulletID>() != null) + " BulletID not found so respawn immidiately");

                DoRespawn();   
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        int damageAmount = 1;
        int spearDamage = 2;
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            //handle enemy bullet:
            TakeDamage(damageAmount, other);
            UpdateHealthBar();
            Destroy(other.gameObject); 
        }
        else if (other.gameObject.CompareTag("Laser"))
        {
            
            TakeDamage(damageAmount, other);
            UpdateHealthBar();
        }
        else if (other.gameObject.CompareTag("MortarAttack"))
        {
            TakeDamage(damageAmount, other);
            UpdateHealthBar();
        }
        else if (other.gameObject.CompareTag("Spear"))
        {
            TakeDamage(spearDamage, other);
            UpdateHealthBar();
        }
    }
    
    private void EnemyKilledPlayer (BulletID info)
    {
        //NOW!
        //Name the Enemy into a random name or ID not already used
        //remember that name
       
        
        GameObject enemy = info.KillerGameObject;
        EnemyMonoBehaviour superEnemyClass = enemy.GetComponent<EnemyMonoBehaviour>();
        EnemyData enemyData = superEnemyClass.GetEnemyData() != null ? superEnemyClass.GetEnemyData() : new EnemyData();
        enemyData.SetDidEncounter(true); // ????
        
        string randomName = "Bert the AI killer";
        
        enemyData.kills++;
        
        enemyData.name = randomName;
        enemyData.enemyType = superEnemyClass.enemyType;
        
        string killerDialouge = "Hahaha! I killed you! Now that might even give me a promotion!";
        
        
        MoveCameraClass moveCam = new MoveCameraClass
        {
            secondsDuration = 3f,
            targetTransform = enemy.transform,
            dialougeText = killerDialouge,
            callbackMethodName = null,
            doRespawn = true
        };
        StartCoroutine(CameraToTarget(moveCam));
        
        print("Create new killer EnemyData");
        superEnemyClass.SetEnemyData(enemyData);
        if (OnKilledBy != null) OnKilledBy(this, enemyData, info.KillerGameObject);
    }
    
    public CameraFollow cameraFollow;
    [FormerlySerializedAs("camera")] public Camera playerCamera;
    public GameObject cameraTest;
    
    public TextMeshProUGUI dialougeText;

    private class MoveCameraClass
    {
        public bool scaleTime = false;
        public float secondsDuration = 2f;
        public string dialougeText = "";
        public string callbackMethodName = null;
        public bool doRespawn = false;
        public Transform targetTransform;
    }
    
    //Improvements I can make: 
    //1. use a unity cutscene plugin editor rather than this simple method
    //2. cinemacamera rather than the current camera
    private IEnumerator CameraToTarget(MoveCameraClass camInfo)
    {
        if (camInfo.scaleTime)
        {
            Time.timeScale = 0.1f;
        }
        for (int i = 1; i < 3; i++)
        {
            if (i >= 2)
            {
                if (camInfo.scaleTime)
                {
                    Time.timeScale = 1;
                }
                DisableEnemyCam();
                if (camInfo.callbackMethodName != null)
                {
                    Invoke(camInfo.callbackMethodName, 0f);
                }
                if (camInfo.doRespawn)
                {
                    if (OnRespawn != null)
                    {
                        OnRespawn(this);
                    }
                    ExecuteDoRespawn();
                }
                break;
            }
            EnableEnemyCam(camInfo.targetTransform, camInfo.dialougeText);
            yield return new WaitForSeconds(camInfo.secondsDuration);
        }
    }
    
    private void EnableEnemyCam(Transform enemyTransform, string text)
    {
        //TODO zoom the camera closer to the enemy
        playerCamera.orthographicSize = 2;
        dialougeText.gameObject.SetActive(true);
        dialougeText.text = text;
        
        
        cameraFollow.followTransform = enemyTransform;
    }

    private void DisableEnemyCam()
    {
        dialougeText.gameObject.SetActive(false);
        cameraFollow.followTransform = gameObject.transform;
        playerCamera.orthographicSize = 7;
    }

    private SerializableDictionary<string, int> enemiesWithKills;

    private void OnEnemyEncounter(List<EnemyMonoBehaviour> enemies)
    {
        foreach (var enemy in enemies)
        {
            if (enemy.enemyDataFieldDefined)
            {
                EncounterOldEnemy(enemy);
            }
        }
        
        
        //PROBLEM! När kameran zoomas in kommer fiender fortsätta anfalla. Finns det möjlighet att pausa spelet?
        
        
        //determine if player has met an enemy previously
        
        
        //if met previously
        
        //say something about that encounter :)
        
        //then run the game as usual
        
        //information I want the enemy to have
        //number of player kills
        //if possible; HOW, the context of those kills
        //e.g. did the enemy STEAL the kill, did the enemy kill you all by themselves, etc?
        //name so the player may recognize the generated villain
        //apperance
        //I can store it all in a string like this:
        // name=Big Evil Robot level = 10 playerKills = 20 usedDialouges = {"hahaha", "lol 20 kills hahahahahah"}
        //did someone make a class to string serializer?
        
        


    }

    private void EncounterOldEnemy(EnemyMonoBehaviour enemy)
    {
        EnemyData enemyData = enemy.GetEnemyData();
        bool didEncounterBefore = enemyData.GetDidEncounter();
        if (didEncounterBefore || playerDied)
        {
            return;
        }
        
        enemyData.SetDidEncounter(true);

        string encounterText =
            "Hey, it's you! Remember me? I killed you earlier.. and now I will do it again haha!";
        
       
        MoveCameraClass moveCam = new MoveCameraClass
        {
            scaleTime = true,
            secondsDuration = 4f / 10f,
            targetTransform = enemy.transform,
            dialougeText = encounterText,
            callbackMethodName = null,
            doRespawn = false,
        };
        StartCoroutine(CameraToTarget(moveCam));
    }


    private void OnEnable()
    {
        ExecuteDoRespawn += DoRespawn;
        
    }

    private void OnDisable()
    {
        ExecuteDoRespawn -= DoRespawn;
    }
    private SerializableDictionary<string, EnemyData> enemyDataDict;
    public void SaveData(ref GameData data)
    {
        
    }

    public void LoadData(GameData data)
    {
      
    }
}

[System.Serializable]
public class EnemyData
{
    public string id;
    public EnemyType enemyType;
    public string name;
    public int kills;

    private bool didEncounter = false;

    public float GetExtraHealth()
    {
        return kills * 2;
    }
    
    
    
    public void SetDidEncounter(bool encountered)
    {
        didEncounter = encountered;
    }

    public bool GetDidEncounter()
    {
        return didEncounter;
    }

    public EnemyData() //constructor
    {
        string itemID = System.Guid.NewGuid().ToString();
        id = itemID;
        kills = 0;
        name = "";
    }

}

