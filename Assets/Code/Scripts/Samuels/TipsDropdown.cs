using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipsDropdown : MonoBehaviour
{

    [SerializeField] GameObject WalkPanel;
    [SerializeField] GameObject ShootPanel;
    [SerializeField] GameObject DashPanel;
    [SerializeField] GameObject HealingPanel;
    [SerializeField] GameObject PowerUpPanel;
    [SerializeField] GameObject UpgradesPanel;
    [SerializeField] GameObject ChangeWeponPanel;


    public void HandleChoosenTips(int tipValue)
    {
        WalkPanel.SetActive(true);
        ShootPanel.SetActive(false);
        DashPanel.SetActive(false);
        HealingPanel.SetActive(false);
        PowerUpPanel.SetActive(false);
        UpgradesPanel.SetActive(false);
        ChangeWeponPanel.SetActive(false);

        if (tipValue == 0)
        {
            WalkPanel.SetActive(true);
            ShootPanel.SetActive(false);
            DashPanel.SetActive(false);
            HealingPanel.SetActive(false);
            PowerUpPanel.SetActive(false);
            UpgradesPanel.SetActive(false);
            ChangeWeponPanel.SetActive(false);

        }
        if (tipValue == 1)
        {
            WalkPanel.SetActive(false);
            ShootPanel.SetActive(true);
            DashPanel.SetActive(false);
            HealingPanel.SetActive(false);
            PowerUpPanel.SetActive(false);
            UpgradesPanel.SetActive(false);
            ChangeWeponPanel.SetActive(false);
        }
        if (tipValue == 2)
        {
            WalkPanel.SetActive(false);
            ShootPanel.SetActive(false);
            DashPanel.SetActive(true);
            HealingPanel.SetActive(false);
            PowerUpPanel.SetActive(false);
            UpgradesPanel.SetActive(false);
            ChangeWeponPanel.SetActive(false);
        }
        if (tipValue == 3)
        {
            WalkPanel.SetActive(false);
            ShootPanel.SetActive(false);
            DashPanel.SetActive(false);
            HealingPanel.SetActive(true);
            PowerUpPanel.SetActive(false);
            UpgradesPanel.SetActive(false);
            ChangeWeponPanel.SetActive(false);
        }
        if (tipValue == 4)
        {
            WalkPanel.SetActive(false);
            ShootPanel.SetActive(false);
            DashPanel.SetActive(false);
            HealingPanel.SetActive(false);
            PowerUpPanel.SetActive(true);
            UpgradesPanel.SetActive(false);
            ChangeWeponPanel.SetActive(false);
        }
        if (tipValue == 5)
        {
            WalkPanel.SetActive(false);
            ShootPanel.SetActive(false);
            DashPanel.SetActive(false);
            HealingPanel.SetActive(false);
            PowerUpPanel.SetActive(false);
            UpgradesPanel.SetActive(true);
            ChangeWeponPanel.SetActive(false);
        }
        if (tipValue == 6)
        {
            WalkPanel.SetActive(false);
            ShootPanel.SetActive(false);
            DashPanel.SetActive(false);
            HealingPanel.SetActive(false);
            PowerUpPanel.SetActive(false);
            UpgradesPanel.SetActive(false);
            ChangeWeponPanel.SetActive(true);
        }
    }
}
