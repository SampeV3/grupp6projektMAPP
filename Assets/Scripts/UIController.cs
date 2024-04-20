using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject inventoryPanel, pausePanel;
    private void Start()
    {
        inventoryPanel.SetActive(false);
        pausePanel.SetActive(false);
    }
    public void OpenInventory()
    {
        Time.timeScale = 0;
        inventoryPanel.SetActive(true);
    }
    public void ExitPanel()
    {
        Time.timeScale = 1;
        inventoryPanel ?.SetActive(false);
        pausePanel ?.SetActive(false);
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
