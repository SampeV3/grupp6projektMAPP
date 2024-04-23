using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairPickup : MonoBehaviour
{
    PlayerTakeDamage playerHealth;
    private UIController uIController;
    public float healthBonus = 1f;

    private void Start()
    {
        uIController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>(); //tillagt av Basir
        // Find the player GameObject with the "Player" tag and get its PlayerTakeDamage component
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerHealth = playerObject.GetComponent<PlayerTakeDamage>();
        }
        else
        {
            Debug.LogError("Player GameObject not found with the tag 'Player'.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerHealth != null && collision.gameObject.CompareTag("Player"))
        {
            // Check if player's current health is less than max health before applying bonus
            if (playerHealth.currentHealth < playerHealth.maxHealth)
            {
                Destroy(gameObject);
                playerHealth.currentHealth += (int)healthBonus;
                playerHealth.UpdateHealthBar();
            }
            if (playerHealth.currentHealth == playerHealth.maxHealth && uIController.inventoryHealthPickupAmount < 3) //tillagt av Basir
            {
                uIController.inventoryHealthPickupAmount += 1;
                gameObject.SetActive(false);
            }
        }
    }

}
