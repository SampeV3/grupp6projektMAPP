using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using NavMeshPlus.Components;
using UnityEngine.Serialization;

public class RoomContentGenerator : MonoBehaviour
{
    [SerializeField]
    private RoomGenerator playerRoom, defaultRoom, bossRoom;

    List<GameObject> spawnedObjects = new List<GameObject>();

    [SerializeField]
    private GraphTest graphTest;


    public Transform itemParent;

    [SerializeField]
    private CinemachineVirtualCamera cinemachineCamera;

    public UnityEvent RegenerateDungeon;
    
    
    //Bake a navmesh at runtime - Elias
    //Documentation: https://github.com/h8man/NavMeshPlus/wiki/HOW-TO#intro
    [FormerlySerializedAs("Surface2D")] public NavMeshSurface surface2D;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var item in spawnedObjects)
            {
                Destroy(item);
            }
            RegenerateDungeon?.Invoke(); //Generera procedurellt ett ny karta.
        }
    }
    public void GenerateRoomContent(DungeonData dungeonData)
    {
        foreach (GameObject item in spawnedObjects)
        {
            DestroyImmediate(item);
        }
        spawnedObjects.Clear();

        SelectPlayerAndBossSpawnPoint(dungeonData);
        SelectEnemySpawnPoints(dungeonData);

        foreach (GameObject item in spawnedObjects)
        {
            if(item != null)
                item.transform.SetParent(itemParent, false);
        }
        
        surface2D.BuildNavMeshAsync();

        
    }


    
    private void SelectPlayerAndBossSpawnPoint(DungeonData dungeonData)
    {
        //V�lj d�r spawnpointen skapas
        //Id�!
        //Skapa en ny metod
        //loopa igenom ALLA positioner och j�mf�r med bosspositionen
        //sen kolla magnituden (avst�ndet fr�n bossspawnen) och v�lj en spawn point som �r s� l�ngtifr�n bossen som m�jligt
        //detta g�r det m�jligt att ut�va mer high level level design i rougelike spelet.



        int randomRoomIndex = UnityEngine.Random.Range(0, dungeonData.bossRoomIndex);
        Vector2Int playerSpawnPoint = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);



        graphTest.RunDijkstraAlgorithm(playerSpawnPoint, dungeonData.floorPositions);

        Vector2Int roomIndex = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);

        List<GameObject> placedPrefabs = playerRoom.ProcessRoom(
            playerSpawnPoint,
            dungeonData.roomsDictionary.Values.ElementAt(randomRoomIndex),
            dungeonData.GetRoomFloorWithoutCorridors(roomIndex)
            );

        
        
        //FocusCameraOnThePlayer(placedPrefabs[placedPrefabs.Count - 1].transform);

        spawnedObjects.AddRange(placedPrefabs);

        int bossRoomIndex = (dungeonData.bossRoomIndex) != null ? dungeonData.bossRoomIndex : dungeonData.roomsDictionary.Count - 1;
        roomIndex = dungeonData.roomsDictionary.Keys.ElementAt(bossRoomIndex);
        Vector2Int bossSpawnPoint = dungeonData.bossRoomPosition;
        placedPrefabs = bossRoom.ProcessRoom(
             bossSpawnPoint,
             dungeonData.roomsDictionary.Values.ElementAt(bossRoomIndex)
             ,
             dungeonData.GetRoomFloorWithoutCorridors(roomIndex)
             ) ;

        spawnedObjects.AddRange(placedPrefabs);

        dungeonData.roomsDictionary.Remove(bossSpawnPoint); //disable default room to trigger for the bossroom :)
        dungeonData.roomsDictionary.Remove(playerSpawnPoint);
    }

    private void FocusCameraOnThePlayer(Transform playerTransform)
    {
        cinemachineCamera.LookAt = playerTransform;
        cinemachineCamera.Follow = playerTransform;
    }

    private void SelectEnemySpawnPoints(DungeonData dungeonData)
    {
        foreach (KeyValuePair<Vector2Int,HashSet<Vector2Int>> roomData in dungeonData.roomsDictionary)
        { 
            spawnedObjects.AddRange(
                defaultRoom.ProcessRoom(
                    roomData.Key,
                    roomData.Value, 
                    dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)
                    )
            );

        }
    }

}
