using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairPickup : MonoBehaviour
{
    PlayerTakeDamage playerHealth;
    public float healthBonus = 1f;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerTakeDamage>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerHealth.currentHealth < playerHealth.maxHealth) 
        {
            Destroy(gameObject);
            playerHealth.currentHealth += playerHealth.currentHealth + (int) healthBonus;
        }
    }
}
