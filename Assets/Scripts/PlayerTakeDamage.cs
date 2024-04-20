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

    public delegate void PlayerKilledByAction(PlayerTakeDamage playerTakeDamage, BulletID info);
    public static event PlayerKilledByAction OnKilledBy;
    private bool playerDied;


    void Start()
    {
        currentHealth = maxHealth;
        updateHealthBar();
        if (spawnTransform == null)
        {
            spawnPosition = transform.position;
        }

    }

    public void updateHealthBar()
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
        OnRespawn(this); //trigga eventet s� att andra script kan lyssna.
    }

    void TakeDamage(int damageAmount, Collider2D other)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            if (playerDied) return;
            playerDied = true;
            if (Nemesis.EnemyKilledPlayer.nemesisEnabled && other.gameObject.GetComponent<BulletID>() != null) {
                EnemyKilledPlayer(other.gameObject.GetComponent<BulletID>()); 
                DoRespawn();
            }
            else
            {
                DoRespawn();   
            }
            
        }
        updateHealthBar();

        if (OnTakeDamage != null)
            OnTakeDamage(this, damageAmount); //object reference not set to an instance of a object?
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int damageAmount = 1;
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            //handle enemy bullet:
            TakeDamage(damageAmount, other);
            updateHealthBar();
            Destroy(other.gameObject); 
        }
        if (other.gameObject.CompareTag("Laser"))
        {
            TakeDamage(damageAmount, other);
            updateHealthBar();
        }
        if (other.gameObject.CompareTag("MortarAttack"))
        {
            TakeDamage(damageAmount, other);
            updateHealthBar();
        }
    }

    private void EnemyKilledPlayer (BulletID info)
    {
        OnKilledBy(this, info);
    }

}
