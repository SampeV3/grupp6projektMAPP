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

    void Start()
    {
        currentHealth = maxHealth;
        updateHealthBar();
    }

    void updateHealthBar()
    {
        healthBarSprites[currentHealth].SetActive(true);
    }

    void takeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth < 0)
        {
            //Spela upp player death animation? effekter? ljud? delay?
            transform.position = spawnPos.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            takeDamage(1);
            updateHealthBar();
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
