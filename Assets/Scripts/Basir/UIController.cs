using Codice.Client.Common.GameUI;
using Codice.CM.SEIDInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class UIController : MonoBehaviour
{
    private class PickupScript : MonoBehaviour
    {
        public delegate void PickupAction (GameObject pickedUpObject);
        public static event PickupAction OnPickup;

        private bool pickedUp = false;
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision != null && collision.gameObject.tag == "Player")
            {
                if (!pickedUp)
                {
                    pickedUp = true;
                    OnPickup(gameObject);
                }
            }
        }
    }

    public bool disableInventoryAndMenuButtonWhileInCombat = false;
    
    public GameObject inventoryPanel, pausePanel, inventoryButton, pauseButton;
    public List<GameObject> inactiveWhileInventoryOpen;

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
    public bool isWeapon_2_picked = false;

    public bool isBossDead;

    //private bool isBoostActivated = false;  Ska användas senare när boost item har en funktion

    public Color inventoryItemUnavailable;
    
    private void Start()
    {
        inventoryPanel.SetActive(false);
        pausePanel.SetActive(false);

        isBossDead = false;
        
        xPPoint.text = "00";
    }

    private void FixedUpdate()
    {
        xPPoint.text = playerSupervisor.XP + " / " + playerSupervisor.experience_required;
        levelInfo.text = "Level: " + playerSupervisor.level;
    }
    private void Update()
    {
        if (inventoryButtonsInPanel[2].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == "0")
        {
            changeItemColor(inventoryButtonsInPanel[2].image, false);
        }
        inventoryButtonsInPanel[2].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + inventoryHealthPickupAmount;

        if (inventoryButtonsInPanel[3].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == "0")
        {
            changeItemColor(inventoryButtonsInPanel[3].image, false);
        }
        else { return; }
        inventoryButtonsInPanel[3].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + inventoryBoostPickupAmount;

        if (!isWeapon_2_picked)
        {
            changeItemColor(inventoryButtonsInPanel[1].image, false);
        }
    }
    public void OpenInventory()
    {
        Time.timeScale = 0;
        inventoryPanel.SetActive(true);
        inventoryButton.SetActive(false);
        pauseButton.SetActive(false);
        SetActiveInList(inactiveWhileInventoryOpen, false);
    }
    public void ExitPanel()
    {
        SetActiveInList(inactiveWhileInventoryOpen, true);
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

    void DropItem( Vector3 position, MinibossAI boss)
    {
        //GameObject newItem = CreateObject(boss.drop, position);
        //PickupScript pickup = newItem.AddComponent<PickupScript>();
        
    }

    void PickupComplete (GameObject item)
    {
        print("Hey Basir, I picked up this " + item + " what do I do with it?");
    }

    public GameObject CreateObject(GameObject prefab, Vector3 placementPosition)
    {
        if (prefab == null)
            return null;
        GameObject newItem;
        if (Application.isPlaying)
        {
            newItem = Instantiate(prefab, placementPosition, Quaternion.identity);
        }
        else
        {
            newItem = Instantiate(prefab);
            newItem.transform.position = placementPosition;
            newItem.transform.rotation = Quaternion.identity;
        }

        return newItem;
    }

    private void OnEnable()
    {
        PlayerTakeDamage.OnCombatSituationChanged += OnCombatChanged;
        //MinibossAI.OnMiniBossDied += DropItem;
        PickupScript.OnPickup += PickupComplete;
    }

    private void OnDisable()
    {
        PlayerTakeDamage.OnCombatSituationChanged -= OnCombatChanged;
        //MinibossAI.OnMiniBossDied -= DropItem;
        PickupScript.OnPickup -= PickupComplete;
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
        if (weapon_1_Selected && isBossDead)
        {
            weapon_2_Selected = true;
            weapon_1_Selected = false;
        }
    }

    private void SetActiveInList(List<GameObject> gameObjects, bool active)
    {
        foreach (var gameObject in gameObjects)
        {
            gameObject.SetActive(active);
        }
    }





    private void changeItemColor (Image image, bool isAvailable)
    {
        if (!isAvailable)
        {
            image.color = inventoryItemUnavailable;
        }
    }

}
