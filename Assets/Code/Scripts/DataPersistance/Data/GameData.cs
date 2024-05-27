using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]


public class GameData
{
    public int totalDeathCount = 0;
    public int totalKillCount = 0;
    public SerializableDictionary<string, int> totalEnemyKillTrackingDictionary;

    public int allies;
    public int level = 0;
    public int experience_required = 100;
    public int XP = 0; //current XP
    
    
    public int soft_coins = 0;
    public int premium_coins = 0;
    
    
    
    public int in_run_points_to_spend;
    public int perkPoints = 0;

    public SerializableDictionary<string, EnemyData> enemies;
    
    public Vector3 playerPosition = Vector3.zero;
    //DICTIONARIES ARE NOT SUPPORTED IN JSON so a custom SerializableDictionary is used.
    public SerializableDictionary<string, bool> coinsCollected; //Example
    
    public SerializableDictionary<string, int> skillLevels; //Example


    public GameData()
    {
        this.allies = 0;
        this.totalDeathCount = 0;
        this.coinsCollected = new SerializableDictionary<string, bool>();
        this.skillLevels = new SerializableDictionary<string, int>();
        this.in_run_points_to_spend = 0;
        this.perkPoints = 0;
        this.enemies = new SerializableDictionary<string, EnemyData>();
        //Neat way to create gui ID:s automatically. These can be used for the dictionary. You have to right click the inspector.
        //[ContextMenu("Generate GUIID for id")]
        //private void GenerateGuiid()
        //{
        //    string id = System.Guid.NewGuid().ToString();
        //}

    }

    public override string ToString()
    {
        return string.Format("Deathcount {0}, skill level count {0}", totalDeathCount, this.skillLevels.Count);

    }

}
