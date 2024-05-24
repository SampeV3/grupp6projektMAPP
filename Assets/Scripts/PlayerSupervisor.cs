using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class PlayerSupervisor : MonoBehaviour, IDataPersistance
{
    
    //Events som �r till f�r UI-script s� att de kan vara helt separerade fr�n detta script!
    public delegate void LevelUpAction(int newLevel); //metod signatur f�r subscribers till eventet
    public static event LevelUpAction OnLevelUp;
    
    public int deathCount = 0;
    public int level = 0;
    public int in_run_points_to_spend = 0;
    public int perkPoints = 0;
    [FormerlySerializedAs("experience_required")] public int experienceRequired = 100;
    private int startingExperienceRequired;
    public int XP = 0;
    
    private bool level_up_check_loop_is_running = false;



    void OnEnable()
    {
        startingExperienceRequired = experienceRequired;
        PlayerTakeDamage.OnRespawn += OnRespawn;
        PlayerTakeDamage.OnTakeDamage += OnTakeDamage;
        PlayerTakeDamage.OnKilledByEvent += OnKilledBy;
        SingletonClass.OnXPAdded += AddXP;
        LevelElevator.BeforeNextLevel += LevelElevatorOnToNextLevel;
    }
    
    void OnDisable()
    {
        PlayerTakeDamage.OnRespawn -= OnRespawn;
        PlayerTakeDamage.OnTakeDamage -= OnTakeDamage;
        PlayerTakeDamage.OnKilledByEvent -= OnKilledBy;
        SingletonClass.OnXPAdded -= AddXP;
        LevelElevator.BeforeNextLevel -= LevelElevatorOnToNextLevel;
        
    }

    private void LevelElevatorOnToNextLevel()
    {
        print("Awarded 1 perk point");
        this.perkPoints += 1;
        
        
    }
    void OnRespawn(PlayerTakeDamage playerTakeDamage)
    {
        this.deathCount++;
        
    }

    void OnTakeDamage(PlayerTakeDamage playerTakeDamage, int damageTaken)
    {
            
    }

    public void AddXP(int XP_AMOUNT)
    {
        
        
        this.XP += XP_AMOUNT; 
        
        
        RunLevelUpLoop();
    }

    public bool purchaseWith_in_run_points_to_spend(int cost)
    {
        if (this.in_run_points_to_spend >= cost)
        {
            this.in_run_points_to_spend -= cost;
            return true;
        }
        return false;
    }

    public bool PurchaseWithPerkPoints(int cost)
    {
        if (this.perkPoints >= cost)
        {
            this.perkPoints -= cost;
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// If you earn more XP so you can level up several times, having a single if statement is not enough - we have to loop until Xp is less than (<) the XpRequired
    /// </summary>
    void RunLevelUpLoop ()
    {
        if (level_up_check_loop_is_running)
        {
            return;
        }
        level_up_check_loop_is_running = true;

        //potentially: move to the FixedUpdate loop :)
        int levelsAdded = 0;
        while (this.XP >= this.experienceRequired)
        {
            levelsAdded++;
            LevelUp();
        }
        print("Levelled up " + levelsAdded + " times");
        level_up_check_loop_is_running = false;
    }

    private void LevelUp()
    {
        double xp_increase_modifier = 1.2;
        this.XP -= experienceRequired;
        this.experienceRequired = (int)(this.experienceRequired * xp_increase_modifier);
        this.level++;
        this.in_run_points_to_spend++;
        if (OnLevelUp != null) OnLevelUp(level);
    }

    private void OnKilledBy(PlayerTakeDamage playerTakeDamage, EnemyData enemyData, GameObject KillerGameObject)
    {
        print("Someone killed the player oh no!");
        if ((KillerGameObject) == null) { print("Unknown player killer oh no"); return; } 
        print("Player killed by " + KillerGameObject.name + " hahahah");
        
        
    }

    private void Awake()
    {
        RunLevelUpLoop();
    }

    public void LoadData(GameData data)
    {
        this.deathCount = data.totalDeathCount;
        
        this.XP = data.XP;
        this.level = data.level;
        this.experienceRequired = data.experience_required;
        this.perkPoints = data.perkPoints;
        
        in_run_points_to_spend = data.in_run_points_to_spend;
        
        RunLevelUpLoop();
    }

    public void SaveData(ref GameData data)
    {
        data.totalDeathCount = this.deathCount;
        
        data.XP = this.XP;
        data.level = this.level;
        data.experience_required = this.experienceRequired;

        data.perkPoints = this.perkPoints;
        data.in_run_points_to_spend = in_run_points_to_spend;
        
    }

    public void OnPermaDeath()
    {
        this.XP = 0;
        this.level = 0;
        this.in_run_points_to_spend = 0;
        this.experienceRequired = startingExperienceRequired;
    }
    


}