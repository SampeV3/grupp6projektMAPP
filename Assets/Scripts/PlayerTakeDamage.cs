using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeDamage : MonoBehaviour
{
    //public int health;
    [SerializeField] private Transform spawnTransform;
    private Vector3 spawnPosition;
    public int currentHealth;
    public int maxHealth = 5;
    public GameObject[] healthBarSprites = new GameObject[6];

    //Events hj�lper till att decoupla koden och h�lla saker mer separerade ifr�n varandra.
    public delegate void RespawnAction(PlayerTakeDamage playerTakeDamage); //metod signatur f�r subscribers till eventet
    public static event RespawnAction OnRespawn;
    public delegate void TakeDamageAction(PlayerTakeDamage playerTakeDamage, int damageTaken); //metod signatur f�r subscribers till eventet
    public static event TakeDamageAction OnTakeDamage; //hur eventet avfyras fr�n detta script.

    public delegate void CombatSituationChanged(bool isInCombat, string combatSituation);
    public static event CombatSituationChanged OnCombatSituationChanged;
    
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

    public void DoRespawn()
    {
        //Spela upp player death animation? effekter? ljud? delay?

        transform.position = spawnTransform ? spawnTransform.position : spawnPosition;
        playerDied = false;
        currentHealth = maxHealth;
        UpdateHealthBar();
        if (OnRespawn != null) OnRespawn(this); //trigga eventet s� att andra script kan lyssna.
    }
    
    void TakeDamage(int damageAmount, Collider2D other)
    {
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
                EnemyKilledPlayer(other.gameObject.GetComponent<BulletID>());
                
                //How about we tell this script to respawn the player
                //when there is a cutscene event that fires!!
                //then the player can choose when to exit the nemesis scene :D
                
                Invoke(nameof(DoRespawn), 1f);
            }
            else
            {
                DoRespawn();   
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int damageAmount = 1;
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
    }

    private void EnemyKilledPlayer (BulletID info)
    {
        if (OnKilledBy != null) OnKilledBy(this, info);
    }

}
