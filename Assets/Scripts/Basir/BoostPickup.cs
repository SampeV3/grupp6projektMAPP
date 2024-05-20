using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPickup : MonoBehaviour
{
    private UIController uIController;

    private void Start()
    {
        uIController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && uIController.inventoryBoostPickupAmount < 5)
        {
            Destroy(gameObject, 0.1f);
            uIController.inventoryBoostPickupAmount += uIController.healthPickupAmountToIncrease;
        }
    }
}
