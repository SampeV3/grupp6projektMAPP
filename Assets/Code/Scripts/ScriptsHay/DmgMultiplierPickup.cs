using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMultiplierItem : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player collided with damage multiplier item.");
            StartCoroutine(EnemyAI.TempDmgIncrease(1, 60));
            Destroy(gameObject); // Disable the item after collision
        }
    }
}