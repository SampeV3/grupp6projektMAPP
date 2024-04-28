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
    
    
    public delegate void PlayerKilledByAction(PlayerTakeDamage playerTakeDamage, BulletID info);
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
            bool isChased = IsPlayer.GetIsPlayerInCombat();
            if (isChased != IsInCombat)
            {
                if (OnCombatSituationChanged != null) OnCombatSituationChanged(isChased, "chase");
                OnEnemyEncounter(isChased);
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
            if (Nemesis.EnemyKilledPlayer.nemesisEnabled && other.gameObject.GetComponent<BulletID>() != null)
            {
                onFreezeActions.Invoke();
                EnemyKilledPlayer(other.gameObject.GetComponent<BulletID>());
                
                //How about we tell this script to respawn the player
                //when there is a cutscene event that fires!!
                //then the player can choose when to exit the nemesis scene :D
                
                
            }
            else
            {
                DoRespawn();   
            }
        }
    }

    private void EnemyKilledPlayer (BulletID info)
    {
        StartCoroutine(KillCamera(info.KillerGameObject.transform));
        if (OnKilledBy != null) OnKilledBy(this, info);   
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

    public CameraFollow cameraFollow;
    [FormerlySerializedAs("camera")] public Camera playerCamera;
    public GameObject cameraTest;
    
    public TextMeshProUGUI dialougeText;
    
    [SerializeField] private Dictionary<string, GameObject> enemyKillDict;
    
    private IEnumerator KillCamera(Transform enemyTransform)
    {
        GameObject enemyGameObject = enemyTransform.gameObject;
        for (int i = 1; i < 3; i++)
        {
            if (i >= 2)
            {
                
                dialougeText.gameObject.SetActive(false);
                cameraFollow.followTransform = gameObject.transform;
                playerCamera.orthographicSize = 7;
                if (ExecuteDoRespawn != null)
                {
                    ExecuteDoRespawn();
                }
                break;
            }

            playerCamera.orthographicSize = 2;
            dialougeText.gameObject.SetActive(true);

            //NOW!
            //Name the Enemy into a random name not already used
            //remember that name
            //store that name in a list
            //when a prefab is instanciated, determine wheter to use that name or not.
            
            
            dialougeText.text = "Hahaha! I killed you! Now that might even give me a promotion!";
            
            cameraFollow.followTransform = enemyTransform;
            yield return new WaitForSeconds(2f);
        }
    }

    private void OnEnemyEncounter(bool isChased)
    {
        if (!isChased)
        {
            return;
        }
        var enemies = IsPlayer.GetEnemiesPlayerIsInCombatWith();
        

        //determine if player has met an enemy previously
        
        
        //if met previously
        
        //say something about that encounter :)
        
        //then run the game as usual
        



    }
    


    private void OnEnable()
    {
        ExecuteDoRespawn += DoRespawn;
    }

    private void OnDisable()
    {
        ExecuteDoRespawn -= DoRespawn;
    }

    public void SaveData(ref GameData data)
    {
        
    }

    public void LoadData(GameData data)
    {
        
    }
}


    

