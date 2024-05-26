using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static LevelElevator;



public class UIController : MonoBehaviour, IDataPersistance
{
    
    
    private class PickupScript : MonoBehaviour
    {
        public delegate void PickupAction (GameObject pickedUpObject);
        public static event PickupAction OnPickup;

        private bool pickedUp = false;
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision != null && collision.gameObject.CompareTag("Player"))
            {
                if (!pickedUp)
                {
                    pickedUp = true;
                    OnPickup(gameObject);
                }
            }
        }
    } 

    public delegate void SpawnAlly(); 

    public static event SpawnAlly OnSpawnAlly;

    public bool disableInventoryAndMenuButtonWhileInCombat = false;
    
    public GameObject inventoryButton, pauseButton;
    //public GameObject settingsPanelInPausePanel, audioSettingsPanel, GeneralSettingsPanel, tutorialInSettingsPanel, inventoryPanel, upgradesPanel, pausePanel; //Basir

    public List<GameObject> inactiveWhilePaused; //Basir
    public List<GameObject> inactiveAtStart;

    public List<GameObject> inactiveWhilePromptedQuestion;


    public PlayerSupervisor playerSupervisor;
    public PlayerTakeDamage playerTakeDamage;

    public TextMeshProUGUI xPPoint, levelInfo;//Basir

    public List<Button> inventoryButtonsInPanel; //Basir

    public int healthPickupAmountToIncrease = 1;//Basir
    public int boostPickupAmountToIncrease = 1;
    public int inventoryHealthPickupAmount = 0;
    public int inventoryBoostPickupAmount = 0;
    public int maxHealthPickUpAmount = 10;

    public static int maxInventoryBoostPickupAmount = 15; //set in the start method.

    public bool weapon_1_Selected = true;//Basir
    public bool weapon_2_Selected = false;

    public AudioClip clickSound, spawnAllySound;//delvis Basir
    private AudioSource audioSource;//Basir

    public Transform crossfadeTranstionImage;//Basir
    private Animator sceneTransitions;

    private void Start()//Basir
    {
        sceneTransitions = crossfadeTranstionImage.GetComponent<Animator>();
        
        foreach (GameObject panel in inactiveAtStart)
        {
            panel.SetActive(false);
        }
        
        audioSource = GetComponent<AudioSource>();
        maxInventoryBoostPickupAmount = 15;
        xPPoint.text = "00";
    }



    
    [SerializeField]
    private static Dictionary<string, UpgradableStat> _upgradableStats = new Dictionary<string, UpgradableStat>();

    [SerializeField] private List<UpgradeTemplateReferences> upgradeReferenceClasses = new List<UpgradeTemplateReferences>();




    
    private void FixedUpdate()//Basir
    {
        xPPoint.text = "XP:" + playerSupervisor.XP + "/" + playerSupervisor.experienceRequired;
        levelInfo.text = ": " + playerSupervisor.level;
    }

    private void Update() //Delvis Basir
    {

        if (IsAnyPanelActive())
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }

        if (inventoryHealthPickupAmount > 0)
        {
            inventoryButtonsInPanel[2].GetComponent<Animator>().enabled = true;
        }
        else
        {
            inventoryButtonsInPanel[2].GetComponent<Animator>().enabled =false;
        }

        inventoryButtonsInPanel[2].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventoryHealthPickupAmount + "/" + maxHealthPickUpAmount;

        if (inventoryBoostPickupAmount > 0)
        {
            inventoryButtonsInPanel[3].GetComponent<Animator>().enabled = true;
        }
        else
        {
            inventoryButtonsInPanel[3].GetComponent<Animator>().enabled = false;
        }

        inventoryButtonsInPanel[3].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventoryBoostPickupAmount + "/"+ maxInventoryBoostPickupAmount;

    }

    public void OpenAnyPanel(GameObject gameObject) //Basir
    {
        
        gameObject.SetActive(true);

    }

    public void ExitAnyPanel(GameObject gameObject) //Basir
    {
        
        Animator gameObjectAnimator = gameObject.GetComponent<Animator>();
        float delay = gameObjectAnimator.GetCurrentAnimatorStateInfo(0).length + 0.1f; //0.1f för att säkerställa att animationen har spelats klart
        StartCoroutine(AnimationDelay("ExitAnyPanel", gameObject, delay, gameObjectAnimator));
    }

    //public void OpenPausePanel()
    //{
    //    SetTimeScale();
    //    pausePanel.SetActive(true);

    //}

    //public void ExitPausePanel()
    //{
    //    StartCoroutine(AnimationDelay("ExitPausePanel", pausePanel, 1f));
    //}

    //public void openSettingsPanelInPausePanel()
    //{
    //    settingsPanelInPausePanel.SetActive(true);
    //}

    //public void ExitAnyPanelInSettingsPanel(GameObject gameObject)
    //{
    //    StartCoroutine(AnimationDelay("ExitPanelInPanel", gameObject, 1f));
    //}
    //public void ExitSettingsPanelInPausePanel()
    //{
    //    StartCoroutine(AnimationDelay("ExitSettingsPanel", settingsPanelInPausePanel, 1f));
    //}
    //public void OpenInventory()
    //{
    //    SetTimeScale();
    //    inventoryPanel.SetActive(true);
    //}
    //public void ExitInventory()
    //{

    //    StartCoroutine(AnimationDelay("ExitInventory", inventoryPanel, 0.5f));
    //}

    //public void OpenUpgradesPanel()
    //{
    //    //SetTimeScale();
    //    upgradesPanel.SetActive(true);
    //}

    //public void ExitUpgradesPanel()
    //{
    //    //SetTimeScale();
    //    StartCoroutine(AnimationDelay("ExitUpgrades", upgradesPanel, 1f));
    //}

    private bool IsAnyPanelActive() //Basir
    {
        foreach (GameObject obj in inactiveAtStart)
        {
            if (obj.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
    private void PauseGame()//Basir
    {
        Time.timeScale = 0;
        SetActiveInList(inactiveWhilePaused, false);
    }

    private void ResumeGame()//Basir
    {
        Time.timeScale = 1;
        SetActiveInList(inactiveWhilePaused, true);
    }
    public void ReturnToMainMenu()//Basir
    {
        float delay = sceneTransitions.GetCurrentAnimatorStateInfo(0).length + 0.01f;
        StartCoroutine(AnimationDelay("MainMenu", null, delay, sceneTransitions));
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

    public void IncreaseHealthFromInventory() //Basir
    {
        if (playerTakeDamage.currentHealth < playerTakeDamage.maxHealth && inventoryHealthPickupAmount > 0)
        {
            playerTakeDamage.currentHealth += healthPickupAmountToIncrease;
            inventoryHealthPickupAmount -= 1;
            playerTakeDamage.UpdateHealthBar();
        }
    }

    public void SpawnAllyBoost() //Basir och Elias
    {
        if (inventoryBoostPickupAmount > 0)
        {
            inventoryBoostPickupAmount -= 1;
            audioSource.PlayOneShot(spawnAllySound, 1f);
            OnSpawnAlly();
        }
    }

    public void SelectWeapon_1() //Basir
    {
        if (weapon_2_Selected)
        {
            weapon_1_Selected = true;
            weapon_2_Selected = false;
        }
    }

    public void SelectWeapon_2() //Basir
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
    

    //tillagt av Elias: uppgradderingar :D

    private class UpgradableStat
    {
        private bool isPerk;
        private int level;
        public int MAX_LEVEL = 100;
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
                "{0} : {1}",
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
            print("This skill is already maxed out at " + skill.GetLevel() + "/" + skill.MAX_LEVEL);
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
        
        
        refs.infoText.text =  skill.GetPercentageModifier() + "%.";
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


    private IEnumerator AnimationDelay(String command,GameObject gameObject, float delay, Animator gameObjectAnimator) //Basir
    {
        if (command == "ExitAnyPanel")
        {
            if (!gameObject.activeSelf)
            {
                yield break;
            }
            else
            {
                gameObjectAnimator.SetTrigger("End");
                yield return new WaitForSecondsRealtime(delay);
                gameObject.SetActive(false);
            }
            
        }
        if (command == "MainMenu")
        {
            gameObjectAnimator.SetTrigger("Start");
            yield return new WaitForSecondsRealtime(delay);
            SceneManager.LoadScene(0);
        }

        
            //if (command == "ExitInventory")
            //{
            //    gameObject.GetComponent<Animator>().SetTrigger("End");
            //    yield return new WaitForSecondsRealtime(delay);
            //    gameObject.SetActive(false);
            //    SetTimeScale();

            //}
            //if (command == "MainMenu")
            //{
            //    yield return new WaitForSecondsRealtime(delay);
            //    SceneManager.LoadScene(0);
            //}

            //if (command == "ExitUpgrades")
            //{
            //    gameObject.GetComponent<Animator>().SetTrigger("End");
            //    yield return new WaitForSecondsRealtime(delay);

            //    gameObject.SetActive(false);


            //}

            //if (command == "ExitPausePanel")
            //{
            //    gameObject.GetComponent<Animator>().SetTrigger("End");
            //    yield return new WaitForSecondsRealtime(delay);
            //    gameObject.SetActive(false);
            //    SetTimeScale();
            //}

            //if (command == "ExitSettingsPanel")
            //{
            //    yield return new WaitForSecondsRealtime(delay / 3);
            //    audioSettingsPanel.SetActive(false);
            //    tutorialInSettingsPanel.SetActive(false);
            //    GeneralSettingsPanel.SetActive(false);
            //    gameObject.GetComponent<Animator>().SetTrigger("End");
            //    yield return new WaitForSecondsRealtime(delay * 0.68f);
            //    gameObject.SetActive(false);

            //}

            //if (command == "ExitPanelInPanel")
            //{
            //    yield return new WaitForSecondsRealtime(delay / 3);
            //    if (gameObject == audioSettingsPanel)
            //    {
            //        GeneralSettingsPanel.SetActive(false);
            //        tutorialInSettingsPanel.SetActive(false);
            //    }
            //    if (gameObject == GeneralSettingsPanel)
            //    {
            //        audioSettingsPanel.SetActive(false);
            //        tutorialInSettingsPanel.SetActive(false);
            //    }
            //    if (gameObject == tutorialInSettingsPanel)
            //    {
            //        GeneralSettingsPanel.SetActive(false);
            //        audioSettingsPanel.SetActive(false);
            //    }
            //}
        
    }


    [SerializeField] private PromptData promptFields;
    
    public void PromptQuestion(PromptQuestion prompt)
    {
        if (promptFields.promptObject.activeInHierarchy)
        {
            print("Already prompting the user");
            return;
        }

        //PAUSA TIDEN

        SetActiveInList(inactiveWhilePromptedQuestion, false);
        promptFields.questionText.text = prompt.questionText;
        promptFields.cancelText.text = prompt.cancelText;
        promptFields.progressText.text = prompt.progressText;
        promptFields.promptObject.SetActive(true);
        promptFields.CancelButton.onClick.AddListener(CancelButtonOnclicked);
        promptFields.ProgressButton.onClick.AddListener(ProgressButtonOnclicked);

        void cleanupConnections()
        {
            
            //SÄTT PÅ TIDEN
            promptFields.CancelButton.onClick.RemoveListener(CancelButtonOnclicked);
            promptFields.ProgressButton.onClick.RemoveListener(ProgressButtonOnclicked);
            
        }
        
        void CancelButtonOnclicked()
        {
            cleanupConnections();
            SetActiveInList(inactiveWhilePromptedQuestion, true);
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



