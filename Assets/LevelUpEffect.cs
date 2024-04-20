using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpEffect : MonoBehaviour
{
    public GameObject levelUIParentToHideAndShow;
    public GameObject textObject;
    public float timeToShow = 5f;

    void OnEnable()
    {
        PlayerSupervisor.OnLevelUp += OnLevelUp;

    }


    void OnDisable()
    {
        PlayerSupervisor.OnLevelUp -= OnLevelUp;
    }

    void Awake()
    {
        levelUIParentToHideAndShow.SetActive(false);
    }

    void OnLevelUp(int level)
    {
        levelUIParentToHideAndShow.SetActive(true);
        textObject.GetComponent<Text>().text = "Level Up " + level + "!";
        Invoke("Hide", timeToShow);
    }

    void Hide()
    {
        levelUIParentToHideAndShow.SetActive(false);
    }

}
