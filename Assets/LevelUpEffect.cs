using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpEffect : MonoBehaviour
{

    public GameObject textObject;

    void OnEnable()
    {
        PlayerSupervisor.OnLevelUp += OnLevelUp;

    }


    void OnDisable()
    {
        PlayerSupervisor.OnLevelUp -= OnLevelUp;
    }

    void OnLevelUp(int level)
    {
        textObject.GetComponent<Text>().text = "Level Up " + level.ToString() + "!";

    }

}
