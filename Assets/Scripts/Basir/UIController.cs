using Codice.Client.Common.GameUI;
using Codice.CM.SEIDInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class UIController : MonoBehaviour, IDataPersistance
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

    //private bool isBoostActivated = false;  Ska anv�ndas senare n�r boost item har en funktion

    public Color inventoryItemUnavailable;
    
    private void Start()
    {
        inventoryPanel.SetActive(false);
        pausePanel.SetActive(false);

        isBossDead = false;
        
        xPPoint.text = "00";
    }

    
    [SerializeField]
    private static Dictionary<string, UpgradableStat> _upgradableStats = new Dictionary<string, UpgradableStat>();

    [SerializeField] private List<UpgradeTemplateReferences> upgradeReferenceClasses = new List<UpgradeTemplateReferences>();
    
    private void FixedUpdate()
    {
        xPPoint.text = playerSupervisor.XP + " / " + playerSupervisor.experienceRequired;
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

    public void ActivateBoost() //ska ut�kas n�r boost item har en funktion
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

    //tillagt av Elias: uppgradderingar :D

    private class UpgradableStat
    {
        private int level;
        public static int MAX_LEVEL = 5;
        private double modifier = 1;
        [SerializeField] double upgradeAmount = 0.03;

        public bool CanLevelUp()
        {
            print((level < MAX_LEVEL) + " level " + level);
            return level < MAX_LEVEL;
        }
        
        private bool LevelUp()
        {
            if (!CanLevelUp()) return false;
            this.modifier += upgradeAmount;
            level += 1;
            return true;
        }

        public bool IncreaseModifier()
        {
            return LevelUp();
        }
        
        public double GetModifier()
        {
            return modifier;
        }

        public double GetPercentageModifier()
        {
            return (1 - modifier) * 100;
        }

        public int GetLevel ()
        {
            return level;
        }
        
        public UpgradableStat (int initialLevel)
        {
            this.level = initialLevel;
            for (int i = 0; i < initialLevel; i++)
            {
                IncreaseModifier();
            }
        }
        
    }

    public static double GetSkillModifier(string skillName)
    {
        if (_upgradableStats.ContainsKey(skillName))
        {
            return _upgradableStats[skillName].GetModifier();
        }
        else
        {
            return (double)1;
        }
        
    }
    public TextMeshProUGUI shopLabelText;

    public void SetInRunShopLabeLText()
    {
        shopLabelText.text = "You have " + playerSupervisor.in_run_points_to_spend + " amount of skill points to spend";
    }
    public void UpgradeSkill(UpgradeTemplateReferences refs, string skillName)
    {
        print("update skill " + skillName);
        if (!_upgradableStats.ContainsKey(skillName))
        {
            print( skillName + "Not added in list!");
            _upgradableStats.Add(skillName, new UpgradableStat(0));
        }
        var skill = _upgradableStats[skillName];

        
        int costToBuy = 1;
        char perkChar = char.Parse("_");
        if (skillName.EndsWith(perkChar))
        {
            
        }
        else
        {
            SetInRunShopLabeLText();
            if (skill.CanLevelUp() == false)
            {
                print("This skill is already maxed out at " + skill.GetLevel() + "/" + UpgradableStat.MAX_LEVEL);
                return;
            }
            if (playerSupervisor.purchaseWith_in_run_points_to_spend(costToBuy) == false)
            {
                print("You do not have enough in_run_points_to_spend. Try levelling up some more :)");
                return;
            }
            
            SetInRunShopLabeLText();
        }
        


        
        var success = skill.IncreaseModifier();
        if (success)
        {
            UpdateSkillInfoText(refs, skillName, skill);
        }
        print("These keys are now in the dictionary: ");
        foreach (string _skillName in _upgradableStats.Keys)
        {
            print(_skillName);
        }
    }

    private static void UpdateSkillInfoText(UpgradeTemplateReferences refs, string skillName, UpgradableStat skill)
    {
        refs.infoText.text = "Upgrade " + skillName + " level " + skill.GetLevel() + " out of " + UpgradableStat.MAX_LEVEL + " currently is giving a bonus of " + skill.GetPercentageModifier() + "%.";
    }

    public void OnOpenUpgradeMenu()
    {
        SetInRunShopLabeLText();
        foreach (UpgradeTemplateReferences refs in upgradeReferenceClasses)
        {
            Button button = refs.upgradeButton;
            print("Add listener to onClick! to " + button);
            string skillName = button.gameObject.name;
            if (_upgradableStats.ContainsKey(skillName))
            {
                UpdateSkillInfoText(refs, skillName, _upgradableStats[skillName]);
            }
            button.onClick.AddListener(() => UpgradeSkill(refs, skillName));
        }
    }

    public void OnCloseUpgradeMenu()
    {
        foreach (UpgradeTemplateReferences refs in upgradeReferenceClasses)
        {
            Button button = refs.upgradeButton;
            print("Remove listener to onClick! to " + button);
            button.onClick.RemoveListener(() => UpgradeSkill(refs, button.gameObject.name));
        }
    }
    
    public void SaveData(ref GameData data)
    {
        print("Save data from UI Controller");
        print("These keys will be stored: ");
        foreach (string skillName in _upgradableStats.Keys)
        {
            print(skillName);
        }

        foreach (string skillName in _upgradableStats.Keys)
        {
            print("Store " + skillName + " at level " + _upgradableStats[skillName].GetLevel());
            if (!data.skillLevels.ContainsKey(skillName))
            {
                data.skillLevels.Add(skillName, _upgradableStats[skillName].GetLevel());
            }
            else
            {
                data.skillLevels[skillName] = _upgradableStats[skillName].GetLevel();
            }
            
        }
        OnCloseUpgradeMenu();
    }

    public void LoadData(GameData data)
    {
        
        print("These keys will be loaded: ");
        foreach (string skillName in _upgradableStats.Keys)
        {
            print(skillName);
        }
        
        foreach (var skillName in data.skillLevels.Keys)
        {
            print("Loaded" + skillName);
            int level = data.skillLevels[skillName];
            print("Load" + skillName + " level " + level);
            _upgradableStats[skillName] = new UpgradableStat(level);
        }
        
        //only really need to run this line if the menu is open when the game starts, e.g. when testing
        //OnOpenUpgradeMenu();
    }    

}


