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

    [SerializeField] private DataPersistanceManager dataPersistanceManager;

    List<GameObject> spawnedObjects = new List<GameObject>();

    [SerializeField]
    private GraphTest graphTest;

    [SerializeField] GameObject playerAlliedUnit;

    [SerializeField] private Transform itemParent;

    [SerializeField]
    private CinemachineVirtualCamera cinemachineCamera;

    public UnityEvent RegenerateDungeon;
    public UnityEvent OnFinishedDungeonPlacement;
    
    //Bake a navmesh at runtime - Elias
    //Documentation: https://github.com/h8man/NavMeshPlus/wiki/HOW-TO#intro
    [FormerlySerializedAs("Surface2D")] public NavMeshSurface surface2D;

    public static Transform staticItemParent;
    public static Transform getItemParent()
    {
        if (staticItemParent == null)
        {
            var list = GameObject.FindGameObjectsWithTag("ItemParent");
            if (list.Length > 0)
            {
                return list[0].transform;
            }
        }        
        return staticItemParent;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("New Room");
            ProcedurallyCreateNewDungeon();
        }
    }

    public static void ClearAllChildren(Transform transform)
    {
        Debug.Log(transform.childCount);
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }

        Debug.Log(transform.childCount);
    }

    public void ProcedurallyCreateNewDungeon()
    {
        itemParent = getItemParent();
        dataPersistanceManager.SaveGame();
        
        ClearAllChildren(itemParent);
        RegenerateDungeon?.Invoke(); //Generera procedurellt ett ny karta.
    }

    public void GenerateRoomContent(DungeonData dungeonData)
    {
        spawnedObjects.Clear();
        ClearAllChildren(itemParent);

        SelectPlayerAndBossSpawnPoint(dungeonData);
        SelectEnemySpawnPoints(dungeonData);

        foreach (GameObject item in spawnedObjects)
        {
            if(item != null)
                item.transform.SetParent(itemParent, false);
        }
        
        surface2D.BuildNavMeshAsync();

        Invoke("OnCompleted", 0.5f);
    }

    private void OnCompleted ()
    {
        OnFinishedDungeonPlacement.Invoke();
    } 

    public static HashSet<Vector2Int> GetPositionsDistantEnoughFrom(Vector2Int positionToAvoid, Dictionary<Vector2Int, HashSet<Vector2Int>> positionsDictionary)
    {
        
        HashSet<Vector2Int> farEnoughFrom = new HashSet<Vector2Int>();
        float farthest = 0;
        foreach (var key in positionsDictionary.Keys.ToList())
        {
            float distance = Vector2Int.Distance(positionToAvoid, key);
            if (distance >= farthest)
            {
                farEnoughFrom.Clear();
                farthest = distance;
                farEnoughFrom.Add(key);
            }
        }

        return farEnoughFrom;
        
        //HashSet<Vector2Int> farEnoughFrom = positionsDictionary.Keys
        //    .Where(key => Vector2Int.Distance(positionToAvoid, key) >= minimumDistance)
        //    .ToHashSet();
            //return farEnoughFrom;
    }
    
    private void SelectPlayerAndBossSpawnPoint(DungeonData dungeonData)
    {
        //V�lj d�r spawnpointen skapas
        //Id�!
        //Skapa en ny metod
        //loopa igenom ALLA positioner och j�mf�r med bosspositionen
        //sen kolla magnituden (avst�ndet fr�n bossspawnen) och v�lj en spawn point som �r s� l�ngtifr�n bossen som m�jligt
        //detta g�r det m�jligt att ut�va mer high level level design i rougelike spelet.



        //dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);


        HashSet<Vector2Int>  eligibleSpawnPoints =
            GetPositionsDistantEnoughFrom(dungeonData.bossRoomPosition, dungeonData.roomsDictionary);
        int randomRoomIndex = UnityEngine.Random.Range(0, eligibleSpawnPoints.Count);
        Vector2Int playerSpawnPoint = eligibleSpawnPoints.ElementAt(randomRoomIndex);
        graphTest.RunDijkstraAlgorithm(playerSpawnPoint, dungeonData.floorPositions);

        Vector2Int roomIndex = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);

        List<GameObject> placedPrefabs = playerRoom.ProcessRoom(
            playerSpawnPoint,
            dungeonData.roomsDictionary.Values.ElementAt(randomRoomIndex),
            dungeonData.GetRoomFloorWithoutCorridors(roomIndex)
            );

        
        
        //FocusCameraOnThePlayer(placedPrefabs[placedPrefabs.Count - 1].transform);

        spawnedObjects.AddRange(placedPrefabs);

        int bossRoomIndex = dungeonData.bossRoomIndex;
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

    private void SpawnAlly()
    {
        Transform transform = IsPlayer.FindPlayerTransformAutomaticallyIfNull();
        GameObject ally = Instantiate(playerAlliedUnit, transform.position, transform.rotation, getItemParent());
    }

    private void OnEnable()
    {
        itemParent = getItemParent();
        UIController.OnSpawnAlly += SpawnAlly;
        PlayerTakeDamage.OnPermaDeathAction += ProcedurallyCreateNewDungeon;
        LevelElevator.ToNextLevel += LevelElevatorOnToNextLevel;
    }

    private void OnDisable()
    {
        UIController.OnSpawnAlly -= SpawnAlly;
        PlayerTakeDamage.OnPermaDeathAction -= ProcedurallyCreateNewDungeon;
        LevelElevator.ToNextLevel -= LevelElevatorOnToNextLevel;
    }

    private void LevelElevatorOnToNextLevel()
    {
        print("TODO: INCREASE DIFFICULITY");
        ProcedurallyCreateNewDungeon();
        
        
    }
}
