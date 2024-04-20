using UnityEngine;
using System.Collections;

public class PlayerSupervisor : MonoBehaviour, IDataPersistance
{
    //Events som är till för UI-script så att de kan vara helt separerade från detta script!
    public delegate void LevelUpAction(int newLevel); //metod signatur för subscribers till eventet
    public static event LevelUpAction OnLevelUp;



    public int deathCount = 0;
    public int level = 0;
    public int experience_required = 100;
    public int XP = 0;
    public int coins = 0;

    private bool level_up_check_loop_is_running = false;



    void OnEnable()
    {
        PlayerTakeDamage.OnRespawn += OnRespawn;
        PlayerTakeDamage.OnTakeDamage += OnTakeDamage;
        PlayerTakeDamage.OnKilledBy += OnKilledBy;
        SingletonClass.OnXPAdded += AddXP;
    }

    void OnDisable()
    {
        PlayerTakeDamage.OnRespawn -= OnRespawn;
        PlayerTakeDamage.OnTakeDamage -= OnTakeDamage;
        PlayerTakeDamage.OnKilledBy -= OnKilledBy;
        SingletonClass.OnXPAdded -= AddXP;

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
        
        
        can_level_up();
    }

    void can_level_up ()
    {
        if (level_up_check_loop_is_running)
        {
            return;
        }
        level_up_check_loop_is_running = true;

        //potentially: move to the FixedUpdate loop :)
        int levelsAdded = 0;
        while (this.XP >= this.experience_required)
        {
            //Level up

            double xp_increase_modifier = 1.2;
            //this.XP -= experience_required;
            this.experience_required = (int)(this.experience_required * xp_increase_modifier);
            this.level++;
            levelsAdded++;
            OnLevelUp(level);
        }
        
        print("Levelled up " + levelsAdded + " times");
        level_up_check_loop_is_running = false;
    }

    private void OnKilledBy(PlayerTakeDamage playerTakeDamage, BulletID info)
    {
        print("Player killed by " + info.KillerGameObject.name + " hahahah");
    }


    public void LoadData(GameData data)
    {
        this.deathCount = data.totalDeathCount;
        this.XP = data.XP;
        this.coins = data.coins;
        //this.experience_required = data.experience_required;
        can_level_up();

    }

    public void SaveData(ref GameData data)
    {
        data.totalDeathCount = this.deathCount;
        data.XP = this.XP;
        data.level = this.level;
        //data.experience_required = this.experience_required;

        
    }

    
    


}