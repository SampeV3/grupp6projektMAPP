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

        //update when enemy kills player
        //when player kills enemy
        //when player flees from enemy
        //when enemey flees from player

        public static void InsertEnemyDataToEnemyInstances()
        {
            
            foreach (EnemyMonoBehaviour enemy in EnemyMonoBehaviour.Instances)
            {
                foreach (var (key, enemyData) in independentEnemyDataDict)
                {
                    print(enemyData.id + " " + enemyData.name + " " + enemyData.kills + " " + enemyData.enemyType);  
                    if (enemyData.enemyType.Equals("SpearAI"))
                    {
                        print("SpearAI equals SpearAI   " + enemyData.enemyType.Trim().Equals(enemy.enemyType.Trim()));
                        print(!enemyIDsSpawned.Contains(enemyData.id));
                    }
                    string loadedType = enemyData.enemyType.Trim();
                    string currentType = enemy.enemyType.Trim();
                    bool sameEnemyType = string.Equals(loadedType, currentType);
                    if (!sameEnemyType)
                    {
                        print(loadedType + " and " + currentType + " are not equals to each other why!!! tf");
                    }
                    

                    bool notAlreadyCreated = !enemy.enemyDataFieldDefined;
                    bool notAlreadySpawned = !enemyIDsSpawned.Contains(enemyData.id);

                    if (notAlreadyCreated && sameEnemyType && notAlreadySpawned)
                    {
                        enemy.SetEnemyData(enemyData);
                        enemy.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                        enemyIDsSpawned.Add(key);
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
            PlayerTakeDamage.OnKilledBy += OnPlayerKilled;
        }

        void OnDisable()
        {
            PlayerTakeDamage.OnKilledBy -= OnPlayerKilled;
        }

        private void OnPlayerKilled(PlayerTakeDamage playerTakeDamage, EnemyData enemyData, GameObject enemyKiller)
        {
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
