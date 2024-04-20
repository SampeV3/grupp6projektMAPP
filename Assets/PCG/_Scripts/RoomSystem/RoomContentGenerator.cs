using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
    }

    private void SelectPlayerAndBossSpawnPoint(DungeonData dungeonData)
    {
        //Välj där spawnpointen skapas
        //Idé!
        //Skapa en ny metod
        //loopa igenom ALLA positioner och jämför med bosspositionen
        //sen kolla magnituden (avståndet från bossspawnen) och välj en spawn point som är så långtifrån bossen som möjligt
        //detta gör det möjligt att utöva mer high level level design i rougelike spelet.



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
