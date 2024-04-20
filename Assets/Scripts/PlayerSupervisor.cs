using UnityEngine;
using System.Collections;

public class PlayerSupervisor : MonoBehaviour, IDataPersistance
{
    void OnEnable()
    {
        PlayerTakeDamage.OnRespawn += OnRespawn;
        PlayerTakeDamage.OnTakeDamage += OnTakeDamage;
        
    }

    private int deathCount = 0;

    void OnDisable()
    {
        PlayerTakeDamage.OnRespawn -= OnRespawn;
        PlayerTakeDamage.OnTakeDamage -= OnTakeDamage;
    }


    void OnRespawn(PlayerTakeDamage playerTakeDamage)
    {
        this.deathCount++;
        
    }

    void OnTakeDamage(PlayerTakeDamage playerTakeDamage, int damageTaken)
    {
            
    }

    public void LoadData(GameData data)
    {
        this.deathCount = data.totalDeathCount;
    }

    public void SaveData(ref GameData data)
    {
        data.totalDeathCount = this.deathCount;

        

    }

   


}