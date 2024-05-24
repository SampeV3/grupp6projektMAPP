using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MortarAI : EnemyMonoBehaviour
{
    [SerializeField] private Material flashOnHit;
    [SerializeField] private GameObject mortarIndicator;
    [SerializeField] private Transform player;
    //[SerializeField] private AudioClip projectileSFX;
    
    private Animator anim;
    private AudioSource audioSource;
    
    private SpriteRenderer sprd;
    private Material originalMat;
    private bool playerDetected;
    private Coroutine combatCoroutine;
    private bool isDead, droppedLoot = false;
    private bool canDealDamage = true;
    private static int dmgMultiplier = 1;
    void Start()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false); //tillagt av Basir
        HP = 10;
        audioSource = GetComponent<AudioSource>();
        sprd = GetComponent<SpriteRenderer>();
        originalMat = sprd.material;
    }

    protected override void Awake()
    {
        base.Awake(); // Make sure to always keep that line
        if (player == null)
        {
            player = IsPlayer.FindPlayerTransformAutomaticallyIfNull(); //Tillagt av Elias
        }
    }

    private IEnumerator Combat()
    {
        while (HP > 0)
        {
            //anim.SetTrigger("Attack");
            StartCoroutine(SpawnMortar());
            yield return new WaitForSeconds(2.5f);
        }
    }

    private IEnumerator SpawnMortar()
    {
        GameObject mortarAim = Instantiate(mortarIndicator, player.position, Quaternion.identity);
        BulletID hitIdentifier = mortarAim.GetComponent<BulletID>();
        hitIdentifier.KillerGameObject = gameObject;
        
        Transform desiredChild = mortarAim.transform.Find("MortarBackground");
        SpriteRenderer mortarsprd = desiredChild.GetComponent<SpriteRenderer>();
        Color ogColor = mortarsprd.color;
        ogColor.a = 0.1f;
        mortarsprd.color = ogColor;
        while (mortarsprd != null && mortarsprd.color.a < 1f)
        {
            mortarAim.transform.Rotate(0f, 0f, 15f * Time.fixedDeltaTime);
            ogColor.a += 0.5f * Time.deltaTime;
            mortarsprd.color = ogColor;
            yield return null;
            if(HP <= 0) { Destroy(mortarAim); }
        }
        if (mortarsprd != null)
        {
        mortarAim.GetComponent<CircleCollider2D>().enabled = true;
        Destroy(mortarAim, 0.1f);

        }
    }

    public override void TakeDamage()
    {
        HP -= 1 * dmgMultiplier;
        OnDied();
        StartCoroutine(Flash());

        if (HP <= 0)
        {
            isDead = true;
            if (combatCoroutine != null)
            {
                StopCoroutine(combatCoroutine);
            }
            sprd.color = Color.red;
            StartCoroutine(DropDelay()); //Tillagt av Basi
            Destroy(gameObject, 1f);     //Destroy(gameObject, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerAttack")) 
        {
            Destroy(other.gameObject);
            TakeDamage();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlasmaGunLaser") && canDealDamage)
        {
            HP -= 0.5f * dmgMultiplier;
            StartCoroutine(Flash());
            canDealDamage = false;
            Invoke("damageCooldownReset", .2f);

        }

        if (HP <= 0)
        {


            OnDied();
            isDead = true;


           /*if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
           */
            if (combatCoroutine != null)
            {
                StopCoroutine(combatCoroutine);
            }
            sprd.color = Color.red;
            dropLoot();
            Destroy(gameObject, 1f);
        }
    }


    private bool hasRunned = false;
    private void OnDied()
    {
        if (hasRunned) return;
        hasRunned = true;
        SingletonClass.OnEnemyKilled(this);
        int XP_TO_AWARD_PLAYER_FOR_KILLING_ENEMY = 10;
        SingletonClass.AwardXP(XP_TO_AWARD_PLAYER_FOR_KILLING_ENEMY);
    }

    private IEnumerator Flash()
    {
        if (isDead) yield break;
        sprd.material = flashOnHit;
        yield return new WaitForSeconds(0.125f);
        sprd.material = originalMat;
    }

    private void Update()
    {
        if (!playerDetected)
        {
            if (IsPlayerWithinDetectionRadius())
            {
                combatCoroutine = StartCoroutine(Combat());
                playerDetected = true;
            }
        }
        else
        {
            if (!IsPlayerWithinDetectionRadius())
            {
                playerDetected = false;
                if (combatCoroutine != null)
                {
                    StopCoroutine(combatCoroutine);
                }
            }
        }
    }

    private bool IsPlayerWithinDetectionRadius()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= 10f)
        {
            return true;
        }
        if (player == null)
        {
            return false;
        }
        return false;
    }
    
    public override bool GetIsChasingPlayer()
    {
        return playerDetected;
    } 

    private IEnumerator DropDelay() //tillagt av Basir
    {
        yield return new WaitForSeconds(0.9f);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(0).gameObject.transform.parent = null;
    }

    private void dropLoot()
    {
        if (droppedLoot) return;
        droppedLoot = true;
        GetComponent<LootBag>().InstantiateLoot(transform.position);
    }

    private void damageCooldownReset()
    {
        canDealDamage = true;
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().WakeUp();
    }

    public static void DamageMultiplier(int amount)
    {
        dmgMultiplier += amount;
    }

    public static IEnumerator TempDmgIncrease(int amount, float time)
    {
        DamageMultiplier(amount);
        Debug.Log("Damage multiplier increased to: " + dmgMultiplier);
        yield return new WaitForSeconds(time);
        DamageMultiplier(-amount);
        Debug.Log("Damage multiplier decreased to: " + dmgMultiplier);
    }

}