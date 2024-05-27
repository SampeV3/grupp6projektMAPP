using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerRoom : RoomGenerator, IDataPersistance
{
    public GameObject player;
    public GameObject testAlly;
    private int alliesToSpawn;

    public List<ItemPlacementData> itemData;

    [SerializeField]
    private PrefabPlacer prefabPlacer;


    public void LoadData(GameData data)
    {
        alliesToSpawn = data.allies;
    }

    public override List<GameObject> ProcessRoom(
        Vector2Int roomCenter, 
        HashSet<Vector2Int> roomFloor, 
        HashSet<Vector2Int> roomFloorNoCorridors)
    {

        ItemPlacementHelper itemPlacementHelper = 
            new ItemPlacementHelper(roomFloor, roomFloorNoCorridors);

        List<GameObject> placedObjects = 
            prefabPlacer.PlaceAllItems(itemData, itemPlacementHelper);

        Vector2Int playerSpawnPoint = roomCenter;

        GameObject playerObject 
            = PrefabPlacer.CreateObject(player, playerSpawnPoint + new Vector2(0.5f, 0.5f));

        
        if ((testAlly) != null)
        { //spawn allies near the player if the player had any last time he or she played.
            for (int i = 0; i < alliesToSpawn; i++)
            {
                GameObject ally = Instantiate<GameObject>(testAlly, playerObject.transform.position, playerObject.transform.rotation, RoomContentGenerator.getItemParent());
                
                //GameObject ally = PrefabPlacer.CreateObject(this.testAlly, playerSpawnPoint + new Vector2(0.5f, 0.5f));
                placedObjects.Add(ally);
            }            
        } 
        

        placedObjects.Add(playerObject);

        return placedObjects;
    }

    public void SaveData(ref GameData data)
    {
        //it's not good to save e.g. data.allies = Chase.alliesAlive here since the UI Controller is a better place to do that,
        //but we still need this method for the interface. / Elias.
    }
}

public abstract class PlacementData
{
    [Min(0)]
    public int minQuantity = 0;
    [Min(0)]
    [Tooltip("Max is inclusive")]
    public int maxQuantity = 0;
    public int Quantity
        => UnityEngine.Random.Range(minQuantity, maxQuantity + 1);
}

[Serializable]
public class ItemPlacementData : PlacementData
{
    public ItemData itemData;
}

[Serializable]
public class EnemyPlacementData : PlacementData
{
    public GameObject enemyPrefab;
    public Vector2Int enemySize = Vector2Int.one;
}

