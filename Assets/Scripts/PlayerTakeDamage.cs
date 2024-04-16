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

    //Events hjälper till att decoupla koden och hålla saker mer separerade ifrån varandra.
    public delegate void RespawnAction(PlayerTakeDamage playerTakeDamage); //metod signatur för subscribers till eventet
    public static event RespawnAction OnRespawn;
    public delegate void TakeDamageAction(PlayerTakeDamage playerTakeDamage, int damageTaken); //metod signatur för subscribers till eventet
    public static event TakeDamageAction OnTakeDamage; //hur eventet avfyras från detta script.

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
            print(i);
            if (isEqualToIndex)
            {
                batterySprite.SetActive(true);
                print(i);
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
            OnRespawn(this); //trigga eventet så att andra script kan lyssna.

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
}
