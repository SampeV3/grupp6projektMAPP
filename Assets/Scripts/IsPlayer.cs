using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Events;

public abstract class EnemyMonoBehaviour : MonoBehaviour
{
    // Inheritors have to implement this (just like with an interface) (as an override though).
    public abstract bool GetIsChasingPlayer();

    public string enemyType;
    
    protected EnemyData persistentEnemyData = null;
    public bool enemyDataFieldDefined = false;

    public EnemyData GetEnemyData()
    {
        print("Return persistentEnemyData " + this.persistentEnemyData);
        if (this.persistentEnemyData == null)
        {
            return null;
        }

        return this.persistentEnemyData;
    }

    public void SetEnemyData(EnemyData newPersistentEnemyData)
    {
        this.enemyDataFieldDefined = newPersistentEnemyData != null;
        this.persistentEnemyData = newPersistentEnemyData;
    }

    protected static void OnDied(EnemyData enemyData)
    {
        if (enemyData != null) {
            Nemesis.NemesisController.independentEnemyDataDict.Remove(enemyData.id);
        }
    }

    private static readonly HashSet<EnemyMonoBehaviour> instances = new HashSet<EnemyMonoBehaviour>();

    // public read-only access to the instances by only providing a clone
    // of the HashSet so nobody can remove items from the outside
    public static HashSet<EnemyMonoBehaviour> Instances => new HashSet<EnemyMonoBehaviour>(instances);

    protected virtual void Awake()
    {
        // simply register yourself to the existing instances
        instances.Add(this);
        Nemesis.NemesisController.OnEnemySpawned(this);
    }

    protected virtual void OnDestroy()
    {
        // don't forget to also remove yourself at the end of your lifetime
        instances.Remove(this);
        if (this.GetEnemyData() != null)
        {
            Nemesis.NemesisController.OnEnemyDestroyed(this.GetEnemyData());
        }
    }
}


public class IsPlayer : MonoBehaviour
{
    public Transform playerTransform;
    //Jag la till denna kodsnutt f�r att g�ra det enklare i editorn. /Elias
    public static Transform FindPlayerTransformAutomaticallyIfNull()
    {
        // get root objects in scene
        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        // iterate root objects and do something
        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject gameObject = rootObjects[i];
            if (gameObject.GetComponent<IsPlayer>())
            {
                return gameObject.GetComponent<IsPlayer>().playerTransform;
            }
        }
        return null;
    }
    
    //https://hatchjs.com/unity-list-of-gameobjects/
    //way more hands on: https://stackoverflow.com/questions/49329764/get-all-components-with-a-specific-interface-in-unity

    public static HashSet<EnemyMonoBehaviour> GetAllEnemies()
    {
        return EnemyMonoBehaviour.Instances;
    }

    public static List<EnemyMonoBehaviour> GetEnemiesPlayerIsInCombatWith()
    {
        var enemies = new List<EnemyMonoBehaviour>(); 
        foreach(var enemy in GetAllEnemies())
        {
            if (enemy.GetIsChasingPlayer() == true)
            {
                enemies.Add(enemy);
            }
        }
        return enemies;
    }
    
    public static bool GetIsPlayerInCombat()
    {
        foreach(var enemy in GetAllEnemies())
        {
            if (enemy.GetIsChasingPlayer() == true)
            {
                return true;
            }
        }
        return false;
    }
    
    
    
    
}
