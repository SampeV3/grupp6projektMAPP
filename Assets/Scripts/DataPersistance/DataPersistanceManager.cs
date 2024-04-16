using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistanceManager : MonoBehaviour
{
    public string fileName = "Default";
    public static DataPersistanceManager Instance { get; private set;}
    private GameData gameData;
    private List<IDataPersistance> dataPersistanceObjects;
    private FileDataHandler fileDataHandler;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one " + this.name + " in the scene.");
        }
        Instance = this;
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void printGameData(string startText)
    {
        Debug.Log(startText + " " + gameData.ToString());
    }

    public void LoadGame(GameData gameData)
    {
        this.gameData = this.fileDataHandler.Load();
        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults by calling void NewGame.");
            NewGame();
        }

        foreach (IDataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.LoadData(this.gameData);
        }
        printGameData("Loaded");
    }

    public void SaveGame()
    {
        foreach (IDataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.SaveData(ref gameData);
        }
        bool successfullySaved = this.fileDataHandler.Save(gameData);
        if (successfullySaved)
        {
            printGameData("Saved");
        } else
        {
            printGameData("FAILED TO SAVE");
        }
        
    }

    //Temporary control:

    private void Start()
    {
        //fileName is what matters for having more than one save file!
        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistanceObjects = FindAllDataPersistanceObjects();

        LoadGame(this.gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistance> FindAllDataPersistanceObjects ()
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistance>();

        return new List<IDataPersistance>(dataPersistanceObjects);

    }
}
