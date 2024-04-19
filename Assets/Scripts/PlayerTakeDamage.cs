using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeDamage : MonoBehaviour
{
    //public int health;
    [SerializeField] private Transform spawnPos;
    public int currentHealth;
    public int maxHealth = 5;
    public GameObject[] healthBarSprites = new GameObject[6];

    //Events hj�lper till att decoupla koden och h�lla saker mer separerade ifr�n varandra.
    public delegate void RespawnAction(PlayerTakeDamage playerTakeDamage); //metod signatur f�r subscribers till eventet
    public static event RespawnAction OnRespawn;
    public delegate void TakeDamageAction(PlayerTakeDamage playerTakeDamage, int damageTaken); //metod signatur f�r subscribers till eventet
    public static event TakeDamageAction OnTakeDamage; //hur eventet avfyras fr�n detta script.

    void Start()
    {
        currentHealth = maxHealth;
        updateHealthBar();
    }

    void updateHealthBar()
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

    void takeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            //Spela upp player death animation? effekter? ljud? delay?
            transform.position = spawnPos.position;
            currentHealth = maxHealth;
            OnRespawn(this); //trigga eventet s� att andra script kan lyssna.

        }
        updateHealthBar();
        OnTakeDamage(this, damageAmount); //object reference not set to an instance of a object?

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            //handle enemy bullet:
            takeDamage(1);
            updateHealthBar();
            Destroy(other.gameObject); 
        }
        if (other.gameObject.CompareTag("Laser"))
        {
            takeDamage(1);
            updateHealthBar();
        }
        if (other.gameObject.CompareTag("MortarAttack"))
        {
            takeDamage(1);
            updateHealthBar();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Bugster")) {
            takeDamage(1);
            updateHealthBar();
            Destroy(other.gameObject);
        }
    }
}
