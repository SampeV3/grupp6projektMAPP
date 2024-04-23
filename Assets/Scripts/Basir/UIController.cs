using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public bool disableInventoryAndMenuButtonWhileInCombat = false;

    public GameObject inventoryPanel, pausePanel, inventoryButton, pauseButton;

    public PlayerSupervisor playerSupervisor;
    public PlayerTakeDamage playerTakeDamage;

    public TextMeshProUGUI xPPoint, levelInfo;

    public List<Button> inventoryButtonsInPanel;

    public int healthPickupAmountToIncrease = 1;
    public int boostPickupAmountToIncrease = 1;
    public int inventoryHealthPickupAmount = 0;
    public int inventoryBoostPickupAmount = 0;

    public bool weapon_1_Selected = true;
    public bool weapon_2_Selected = false;
    //private bool isBoostActivated = false;  Ska användas senare när boost item har en funktion
    private void Start()
    {
        inventoryPanel.SetActive(false);
        pausePanel.SetActive(false);
        
        xPPoint.text = "00";
    }

    private void FixedUpdate()
    {
        xPPoint.text = playerSupervisor.XP + " / " + playerSupervisor.experience_required;
        levelInfo.text = "Level: " + playerSupervisor.level;
    }
    private void Update()
    {
        inventoryButtonsInPanel[2].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + inventoryHealthPickupAmount;
        inventoryButtonsInPanel[3].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + inventoryBoostPickupAmount;
    }
    public void OpenInventory()
    {
        Time.timeScale = 0;
        inventoryPanel.SetActive(true);
        inventoryButton.SetActive(false);
        pauseButton.SetActive(false);
    }
    public void ExitPanel()
    {
        Time.timeScale = 1;
        inventoryPanel ?.SetActive(false);
        pausePanel ?.SetActive(false);
        pauseButton.SetActive(true);
        inventoryButton?.SetActive(true);
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        inventoryButton.SetActive(false);
        pauseButton.SetActive(false);
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void OnCombatChanged(bool isInCombat, string situation)
    {
        if (!disableInventoryAndMenuButtonWhileInCombat) return;
        if (isInCombat)
        {
            inventoryButton.SetActive(false);
            pauseButton.SetActive(false);
        }
        else
        {
            inventoryButton.SetActive(true);
            pauseButton.SetActive(true);
        }
    }
    
    private void OnEnable()
    {
        PlayerTakeDamage.OnCombatSituationChanged += OnCombatChanged;
    }

    private void OnDisable()
    {
        PlayerTakeDamage.OnCombatSituationChanged -= OnCombatChanged;
    }

    public void IncreaseHealthFromInventory()
    {
        if (playerTakeDamage.currentHealth < playerTakeDamage.maxHealth && inventoryHealthPickupAmount > 0)

        {
            playerTakeDamage.currentHealth += healthPickupAmountToIncrease;
            inventoryHealthPickupAmount -= 1;
            playerTakeDamage.UpdateHealthBar();      
        }
    }

    public void ActivateBoost() //ska utökas när boost item har en funktion
    {
        if (inventoryBoostPickupAmount < 2 && inventoryBoostPickupAmount != 0)
        {
            inventoryBoostPickupAmount -= 1;
        }
    }

    public void SelectWeapon_1()
    {
        if (weapon_2_Selected)
        {
            weapon_1_Selected = true;
            weapon_2_Selected = false;
        }
    }

    public void SelectWeapon_2()
    {
        if (weapon_1_Selected)
        {
            weapon_2_Selected = true;
            weapon_1_Selected = false;
        }
    }

}
