using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Nemesis
{
    public class NemesisController : MonoBehaviour, IDataPersistance
    {
        public static bool nemesisEnabled = true; //så att spelaren kan ha som preferens att ha det eller inte ;)
        public static SerializableDictionary<string, EnemyData> independentEnemyDataDict = new SerializableDictionary<string, EnemyData>();
        private static List<string> enemyIDsSpawned = new List<string>();
        
        public static void InsertEnemyDataToEnemyInstances()
        {
            
            foreach (EnemyMonoBehaviour enemy in EnemyMonoBehaviour.Instances)
            {
                foreach (var (key, enemyData) in independentEnemyDataDict)
                {
                    //print(" " + enemyData.name + " " + enemyData.kills + " " + enemyData.enemyType);  
                    EnemyType loadedType = enemyData.enemyType;
                    EnemyType currentType = enemy.enemyType;
                    
                    bool sameEnemyType = (int)loadedType == (int)currentType;
                    //print(enemyData.enemyType + " is the same as " + enemy.enemyType + " = " + sameEnemyType);
                    bool notAlreadyCreated = !enemy.enemyDataFieldDefined;
                    bool notAlreadySpawned = !enemyIDsSpawned.Contains(enemyData.id);

                    if (notAlreadyCreated && sameEnemyType && notAlreadySpawned)
                    {
                        enemyData.SetDidEncounter(false);
                        enemy.SetEnemyData(enemyData);
                        enemyIDsSpawned.Add(key);
                        if (enemy.TryGetComponent<SpriteRenderer>(out SpriteRenderer rend))
                        {
                            rend.color = Color.green;
                        } 
                        
                    }
                    
                }

            }
            
            
        }

         
        public void SaveData(ref GameData data)
        {
            data.enemies = independentEnemyDataDict;
        }

        public void LoadData(GameData data)
        {
            independentEnemyDataDict = data.enemies;
        }

        void OnEnable()
        {
            PlayerTakeDamage.OnKilledByEvent += OnPlayerKilled;
        }

        void OnDisable()
        {
            PlayerTakeDamage.OnKilledByEvent -= OnPlayerKilled;
        }

        private void OnPlayerKilled(PlayerTakeDamage playerTakeDamage, EnemyData enemyData, GameObject enemyKiller)
        {
            if (independentEnemyDataDict.ContainsKey(enemyData.id))
            {
                Debug.Log("Error enemy id already exists, trying to make a new one!");
                enemyData.id = enemyData.id + independentEnemyDataDict.Count;
            }
            independentEnemyDataDict.Add(enemyData.id, enemyData);   
        }

        public static void OnEnemyKilled(EnemyData enemyData)
        {
            if (enemyData != null)
            {
                independentEnemyDataDict.Remove(enemyData.id);
            }
        }

        public static void OnEnemyDestroyed(EnemyData enemyData)
        {
            enemyIDsSpawned.Remove(enemyData.id);
        }

        public static void OnEnemySpawned(EnemyMonoBehaviour enemy)
        {
            InsertEnemyDataToEnemyInstances();
        }

    }

   


}
