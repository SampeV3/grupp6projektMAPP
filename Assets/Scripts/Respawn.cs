using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{
    void OnEnable()
    {
        PlayerTakeDamage.OnRespawn += OnRespawn;
        PlayerTakeDamage.OnTakeDamage += OnTakeDamage;
    }


    void OnDisable()
    {
        PlayerTakeDamage.OnRespawn -= OnRespawn;
        PlayerTakeDamage.OnTakeDamage -= OnTakeDamage;
    }


    void OnRespawn(PlayerTakeDamage playerTakeDamage)
    {
        
    }

    void OnTakeDamage(PlayerTakeDamage playerTakeDamage, int damageTaken)
    {
            
    }
}