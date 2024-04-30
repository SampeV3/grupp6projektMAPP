using System.Runtime.CompilerServices;
using UnityEngine;

namespace Nemesis
{
    public class NemesisController : MonoBehaviour, IDataPersistance
    {
        public static bool nemesisEnabled = true; //så att spelaren kan ha som preferens att ha det eller inte ;)
        private SerializableDictionary<string, EnemyData> enemyDataDict;
        //update when enemy kills player
        //when player kills enemy
        //when player flees from enemy
        //when enemey flees from player
        
        //how?
        //idea. 1. every script needs to reference this list only
        //idea 2. 
        
        
        
        public static CameraFollow cameraFollow;

        public static CameraFollow CenterCamera(GameObject enemy)
        {
            cameraFollow.followTransform = enemy.transform;
            return cameraFollow;
        }

        public static void saySomethingEvil()
        {
            //OK SO NOW I KNOW I need to..
            
            //Store data for the enemy,
            //  Did I kill you before?
            //  When I kill you the first time,
            //      what do I say?
            //  How did the enemy kill the player? Assist, accident
            //  Did you flee from me before? I.e. I targeted you and then you fled?
            //          then the enemy can say.. oh so you decided to show up again?
                // need information about player - enemy interactions basically!
                
                //alright great!
                
        }

        public void AddOldEnemyDataToExistingEnemies()
        {
            foreach (EnemyMonoBehaviour enemy in IsPlayer.GetAllEnemies())
            {
                if (!enemy.isActiveAndEnabled || enemy.persistentEnemyData != null)
                {
                    continue;
                }
                AddEnemyDataToEnemyMonoBehaviour(enemy);
            }
        }

        private void AddEnemyDataToEnemyMonoBehaviour(EnemyMonoBehaviour enemy)
        {
            foreach (var (key, enemyData) in enemyDataDict)
            {
                if (enemy.enemyType.Equals(enemyData.enemyType))
                {
                    enemy.persistentEnemyData = enemyData;
                    enemyDataDict.Remove(key);
                    break;
                }
            }
        }

        public void UpdateEnemyDataDictionary()
        {
            foreach (EnemyMonoBehaviour enemy in IsPlayer.GetAllEnemies())
            {
                EnemyData enemyData = enemy.persistentEnemyData;
                if (enemyData == null)
                {
                    continue;
                }
                enemyDataDict[enemyData.name] = enemyData;
            }
        }
        
        public void SaveData(ref GameData data)
        {
            data.enemies = enemyDataDict;
        }

        public void LoadData(GameData data)
        {
            print("EnemyDataDict " + data.enemies);
            foreach(string enemyName in data.enemies.Keys)
            {
                EnemyData enemyData = data.enemies[enemyName];
                print(enemyData.kills + " " + enemyData.name);

            }
        
            enemyDataDict = data.enemies;
        }
        
    }
    
    
    
}
