using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class GameData
{
    public int totalDeathCount = 0;
    public int totalKillCount = 0;
    public SerializableDictionary<string, int> totalEnemyKillTrackingDictionary;

    public int level = 0;
    public int experience_required = 100;
    public int XP = 0; //current XP
    public int coins = 0;
    








    public Vector3 playerPosition = Vector3.zero;
    public SerializableDictionary<string, bool> coinsCollected; //Example
    //DICTIONARIES ARE NOT SUPPORTED IN JSON.


    public GameData()
    {
        this.totalDeathCount = 0;
        this.coinsCollected = new SerializableDictionary<string, bool>();

        //Neat way to create gui ID:s automatically. These can be used for the dictionary. You have to right click the inspector.
        //[ContextMenu("Generate GUIID for id")]
        //private void GenerateGuiid()
        //{
        //    string id = System.Guid.NewGuid().ToString();
        //}

    }

    public override string ToString()
    {
        return string.Format("Deathcount {0}", totalDeathCount);

    }

}
