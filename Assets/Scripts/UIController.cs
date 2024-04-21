using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject inventoryPanel, pausePanel, inventoryButton, pauseButton;
    public PlayerSupervisor playerSupervisor;
    public TextMeshProUGUI xPPoint, levelInfo;
    private void Start()
    {
        inventoryPanel.SetActive(false);
        pausePanel.SetActive(false);
        
        xPPoint.text = "00";
    }

    private void FixedUpdate()
    {
        xPPoint.text = "" + playerSupervisor.XP + " / " + playerSupervisor.experience_required;
        levelInfo.text = "Level: " + playerSupervisor.level;
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
}
