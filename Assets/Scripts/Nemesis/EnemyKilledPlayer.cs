using UnityEngine;

namespace Nemesis
{
    public class EnemyKilledPlayer : MonoBehaviour
    {
        public static bool nemesisEnabled = true; //så att spelaren kan ha som preferens att ha det eller inte ;)

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
        
        
        


    }
    
    
    
}
