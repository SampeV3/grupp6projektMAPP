using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{
    void OnEnable()
    {
        PlayerTakeDamage.OnRespawn += OnRespawn;
    }


    void OnDisable()
    {
        PlayerTakeDamage.OnRespawn -= OnRespawn;

    }


    void OnRespawn(PlayerTakeDamage playerTakeDamage)
    {
        
    }

    void OnTakeDamage(PlayerTakeDamage playerTakeDamage, int damageTaken)
    {
            
    }
}