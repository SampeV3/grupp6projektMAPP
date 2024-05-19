using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    
    public GameObject inventoryButton, pauseButton, inventoryPanel, upgradesPanel;
    public List<GameObject> inactiveWhilePaused;
    public List<GameObject> inactiveWhilePromptedQuestion;
    

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

    public AudioClip clickSound;

    public Color inventoryItemUnavailable;

    public Transform animatorObject;
    
    private void Start()
    {
        xPPoint.text = "00";
        inventoryPanel.SetActive(false);
        upgradesPanel.SetActive(false);
    }

    
    [SerializeField]
    private static Dictionary<string, UpgradableStat> _upgradableStats = new Dictionary<string, UpgradableStat>();

    [SerializeField] private List<UpgradeTemplateReferences> upgradeReferenceClasses = new List<UpgradeTemplateReferences>();
    
    private void FixedUpdate()
    {
        xPPoint.text = "XP:" + playerSupervisor.XP + "/" + playerSupervisor.experienceRequired;
        levelInfo.text = "Level: " + playerSupervisor.level;
    }
    private void Update()
    {
        if (inventoryHealthPickupAmount == 0)
        {
            inventoryButtonsInPanel[2].GetComponent<Animator>().enabled = false;
            changeItemColor(inventoryButtonsInPanel[2].image, false);
        }
        inventoryButtonsInPanel[2].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventoryHealthPickupAmount + "/3";

        if (inventoryBoostPickupAmount == 0)
        {
            inventoryButtonsInPanel[3].GetComponent<Animator>().enabled = false;
            changeItemColor(inventoryButtonsInPanel[3].image, false);
        }
        inventoryButtonsInPanel[3].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + inventoryBoostPickupAmount;

    }

    public void OpenInventory()
    {
        SetTimeScale();
        inventoryPanel.SetActive(true);
    }
    public void ExitInventory()
    {
        
        StartCoroutine(AnimationDelay("ExitInventory", 0.5f));
    }

    public void OpenUpgradesPanel()
    {
        //SetTimeScale();
        upgradesPanel.SetActive(true);
    }

    public void ExitUpgradesPanel()
    {
        //SetTimeScale();
        StartCoroutine(AnimationDelay("ExitUpgrades", 1f));
    }

    private void SetTimeScale()
    {
        Time.timeScale = (Time.timeScale == 0) ? 1 : 0;

        if (Time.timeScale > 0)
        {
            SetActiveInList(inactiveWhilePaused, true);
        }
        else
        {
            SetActiveInList(inactiveWhilePaused, false);
        }

    }
    public void ReturnToMainMenu()
    {
       StartCoroutine(AnimationDelay("MainMenu", 1f));
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
        PickupScript.OnPickup += PickupComplete;
    }

    private void OnDisable()
    {
        PlayerTakeDamage.OnCombatSituationChanged -= OnCombatChanged;
        PickupScript.OnPickup -= PickupComplete;
    }

    public void IncreaseHealthFromInventory()
    {
        if (playerTakeDamage.currentHealth < playerTakeDamage.maxHealth && inventoryBoostPickupAmount > 0)
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
            image.color = inventoryItemUnavailable; //färg för icke-tillgängliga items
        }
    }

    //tillagt av Elias: uppgradderingar :D

    private class UpgradableStat
    {
        private bool isPerk;
        private int level;
        public static int MAX_LEVEL = 12;
        private double modifier = 1;
        [SerializeField] double upgradeAmount = 0.03;

        public bool CanLevelUp()
        {
            print((level < MAX_LEVEL) + " level " + level);
            return level < MAX_LEVEL;
        }
        
        public bool IncreaseModifier()
        {

            if (!CanLevelUp())
            {
                return false;
            }
            
            this.modifier += upgradeAmount;
            level++;
            
            return true;
        }
        
        public double GetModifier()
        {
            return modifier;
        }

        public double GetPercentageModifier()
        {
            return Math.Round(modifier * 100) - 100;
        }

        public int GetLevel ()
        {
            return level;
        }

        public bool GetIsPerk()
        {
            return isPerk;
        }

        public void ResetSkillIfNotPerk()
        {
            if (this.isPerk == false)
            {
                this.level = 0;
                this.modifier = 1;
            }
        }

        public void ResetSkill()
        {
            this.level = 0;
            this.modifier = 0;
        }
        
        public UpgradableStat (int initialLevel, bool isPerk)
        {
            this.isPerk = isPerk;
            print("Reinitiliaze skill " + this.level + " to " + initialLevel);
            for (int i = 0; i < initialLevel; i++)
            {
                print("Increase level " + i);
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

    public void SetShopLabelPointsText()
    {
        string text =
            string.Format(
                "You have {0} skill points to spend and {1} perk points to spend. Perks are permanent upgrades while skill points will reset upon dying.",
                playerSupervisor.in_run_points_to_spend, playerSupervisor.perkPoints);
        shopLabelText.text = text;
    }
    
    public void UpgradeSkill(UpgradeTemplateReferences refs, string skillName)
    {
        print("update skill " + skillName);
        if (!_upgradableStats.ContainsKey(skillName))
        {
            print( skillName + "Not added in list! so adding it now! :D");
            _upgradableStats.Add(skillName, new UpgradableStat(0, refs.isPerk));
        }
        var skill = _upgradableStats[skillName];
        SetShopLabelPointsText();
        if (skill.CanLevelUp() == false)
        {
            print("This skill is already maxed out at " + skill.GetLevel() + "/" + UpgradableStat.MAX_LEVEL);
            return;
        }
        
        int costToBuy = 1; //TODO: flytta till upgrade klassen?
        
        if (refs.isPerk)
        {
            if (playerSupervisor.PurchaseWithPerkPoints(costToBuy) == false)
            {
                print("You do not have enough perk points to spend. Try advancing through some more floors.");
                return;
            }
            
        }
        else
        {
            if (playerSupervisor.purchaseWith_in_run_points_to_spend(costToBuy) == false) //This will update the points.
            {
                print("You do not have enough in_run_points_to_spend. Try levelling up some more :)");
                return;
            }
        }
        
        SetShopLabelPointsText();
        
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
            button.onClick.RemoveAllListeners();
            //button.onClick.RemoveListener(() => UpgradeSkill(refs, button.gameObject.name));
        }
    }

    private const string PerkIdentifierString = "#";
    public void SaveData(ref GameData data)
    {
        print("Save data from UI Controller");
        print("These keys will be stored: ");
        foreach (string skillName in _upgradableStats.Keys)
        {
            print(skillName);
        }

        foreach (string unmodifiedSkillName in _upgradableStats.Keys)
        {
            string skillNameToStore = unmodifiedSkillName; //Only use for the data.skillLevels dictionary
            UpgradableStat skill = _upgradableStats[unmodifiedSkillName];
            
            print("Store " + unmodifiedSkillName + " at level " + _upgradableStats[unmodifiedSkillName].GetLevel());
            
            //Serialize name key as a perk //Note that this could also be moved to the upgradableStat class I suppose...
            if (skill.GetIsPerk())
            {
                skillNameToStore = unmodifiedSkillName + PerkIdentifierString;
            }
            
            if (!data.skillLevels.ContainsKey(skillNameToStore))
            {
                data.skillLevels.Add(skillNameToStore, _upgradableStats[unmodifiedSkillName].GetLevel());
            }
            else
            {
                data.skillLevels[skillNameToStore] = _upgradableStats[unmodifiedSkillName].GetLevel();
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
        
        foreach (var unmodifiedSkillName in data.skillLevels.Keys)
        {
            string skillName = unmodifiedSkillName;
            int levelLoaded = data.skillLevels[skillName];
            print("Load " + skillName + " at current level " + levelLoaded);
            print("Loaded" + skillName);
            
            //Deserialize perk identifier
            bool isPerk = skillName.EndsWith(PerkIdentifierString);
            if (isPerk)
            {
                skillName = skillName.Remove(unmodifiedSkillName.Length - PerkIdentifierString.Length);
            }
            
            _upgradableStats[skillName] = new UpgradableStat(levelLoaded, isPerk);
        }
        
        //only really need to run this line if the menu is open when the game starts, e.g. when testing
        //OnOpenUpgradeMenu();
    }

    [SerializeField] private List<GameObject> inactiveWhilePlayerFrozen;
    public void OnCharacterFrozen()
    {
        SetActiveInList(inactiveWhilePlayerFrozen, false);    
    }
    
    
    
    public void OnPermaDeath()
    {
        foreach (string skillName in _upgradableStats.Keys)
        {
            _upgradableStats[skillName].ResetSkillIfNotPerk();
        }
        inventoryHealthPickupAmount = 0;
        inventoryBoostPickupAmount = 0;

    }


    private IEnumerator AnimationDelay(String command, float delay)
    {
        if (command == "ExitInventory")
        {
            inventoryPanel.GetComponent<Animator>().SetTrigger("End");
            yield return new WaitForSecondsRealtime(delay);
            inventoryPanel.SetActive(false);
            SetTimeScale();

        }
        if(command == "MainMenu")
        {
            yield return new WaitForSecondsRealtime(delay);
            SceneManager.LoadScene(0);
        }

        if (command == "ExitUpgrades")
        {
            upgradesPanel.GetComponent<Animator>().SetTrigger("End");
            yield return new WaitForSecondsRealtime(delay);
            
            upgradesPanel.SetActive(false);
            

        }



    }


    [SerializeField] private PromptData promptFields;
    
    public void PromptQuestion(PromptQuestion prompt)
    {
        if (promptFields.promptObject.activeInHierarchy)
        {
            print("Already prompting the user");
            return;
        }
        SetActiveInList(inactiveWhilePromptedQuestion, false);
        promptFields.questionText.text = prompt.questionText;
        promptFields.cancelText.text = prompt.cancelText;
        promptFields.progressText.text = prompt.progressText;
        promptFields.promptObject.SetActive(true);
        promptFields.CancelButton.onClick.AddListener(CancelButtonOnclicked);
        promptFields.ProgressButton.onClick.AddListener(ProgressButtonOnclicked);

        void cleanupConnections()
        {
            promptFields.CancelButton.onClick.RemoveListener(CancelButtonOnclicked);
            promptFields.ProgressButton.onClick.AddListener(ProgressButtonOnclicked);
            
        }
        
        void CancelButtonOnclicked()
        {
            cleanupConnections();
            StartCoroutine(prompt.OnCancel(promptFields));
        }


        void ProgressButtonOnclicked()
        {
            cleanupConnections();
            StartCoroutine(prompt.OnProgress(promptFields));
        }
    }

    
    
}

public abstract class PromptQuestion : MonoBehaviour
{
    public string name;
    public string questionText;
    public string cancelText;
    public string progressText;
    
    public abstract IEnumerator OnCancel(PromptData promptFields);
    public abstract IEnumerator OnProgress(PromptData promptFields);


}



