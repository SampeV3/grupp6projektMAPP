using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpEffect : MonoBehaviour
{
    public GameObject levelUIParentToHideAndShow;
    public TextMeshProUGUI description, levelText;
    
    public float timeToShow = 4f;

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
        //description.text = "Level Up!"; //Kommer uppdateras med localization
        levelText.text = "" + level;
        Invoke(nameof(Hide), timeToShow);
    }

    void Hide()
    {
        levelUIParentToHideAndShow.SetActive(false);
    }

}
