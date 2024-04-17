using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IsPlayer : MonoBehaviour
{
    public Transform playerTransform;
    //Jag la till denna kodsnutt för att göra det enklare i editorn. /Elias
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
}
