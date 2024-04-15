using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeDamage : MonoBehaviour
{
    //public int health;
    public int currentHealth;
    public int maxHealth = 5;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void takeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth < 0)
        {
            //Spela upp player death animation? effekter? ljud? delay?
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            takeDamage(1);
        }
        if (other.gameObject.CompareTag(""))
        {
            takeDamage(1);
        }
        if (other.gameObject.CompareTag(""))
        {
            takeDamage(1);
        }

    }
}
