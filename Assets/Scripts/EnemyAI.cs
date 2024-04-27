using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using System;

public class EnemyAI : EnemyMonoBehaviour
{
    [SerializeField] private Material flashOnHit;
    [SerializeField] private GameObject projectilePrefab, playerSpotted;
    //[SerializeField] private AudioClip projectileSFX;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask playerMask, obstacleMask;

    private Rigidbody2D rigidBody2D;
    private AudioSource audioSource;
    private Material originalMat;
    private SpriteRenderer sprd;
    private Animator anim;
    private float HP;
    public float shootDelay = 1f;
    [FormerlySerializedAs("XP_TO_AWARD_PLAYER_FOR_KILLING_ENEMY")] public int xpToAwardPlayerForKillingEnemy = 100;
    private bool playerDetected = false;
    private bool isDead, droppedLoot = false;
    private bool canDealDamage = true;
    private Coroutine moveCoroutine, combatCoroutine;
    private GameObject playerSpottedWarning;
    private static int dmgMultiplier = 1;
   


    void Start()
    {
        HP = 10;
        audioSource = GetComponent<AudioSource>();
        //anim = GetComponent<Animator>();
        sprd = GetComponent<SpriteRenderer>();
        originalMat = sprd.material;
        rigidBody2D = GetComponent<Rigidbody2D>();


    }

    protected override void Awake()
    {
        base.Awake(); // Make sure to always keep that line
        if (player == null)
        {
            player = IsPlayer.FindPlayerTransformAutomaticallyIfNull(); //Tillagt av Elias
        }
    }
    
    private IEnumerator MoveAround()
    {
        while (true)
        {
            float distance = 0f;
            Vector2 randomDirection = Vector2.zero;
            while (distance < 4f)
            {
                randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, randomDirection, 50f,obstacleMask);
                if (hit.collider != null && hit.collider.CompareTag("Wall") && hit.distance >= 4f)
                {
                    distance = hit.distance;
                }
            }
            Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y) + randomDirection * UnityEngine.Random.Range(1f,distance-1.9f);
            if (targetPosition.x > transform.position.x)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),transform.localScale.y,transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
            }
            while (Vector2.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, 1.6f * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(1.5f);
        }
    }

    private int SortByDistance(GameObject o1, GameObject o2)
    {
        float dist1 = Vector2.Distance(o1.transform.position, this.transform.position);
        float dist2 = Vector2.Distance(o2.transform.position,this. transform.position);
 
        if (dist1 > dist2)
        {
            return 1;
        }
        if (dist1 < dist2)
        {
            return -1;
        }
        return 0;
    }

    private Transform FindNearestTarget()
    {
        string enemyTag = "Player";
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Array.Sort(enemies, SortByDistance);
        return enemies.Length > 0 ? enemies[0].transform : null;
    }

    private void FixedUpdate()
    {
        rigidBody2D.WakeUp();
        Transform newTarget = FindNearestTarget();
        if (newTarget) { player = newTarget; }
        if (!playerDetected)
        {
            if (IsPlayerWithinDetectionRadius())
            {
                CheckLineOfSight();
                
            }
        }
        else
        {
            if (!IsPlayerWithinDetectionRadius())
            {
                playerDetected = false;
                if (combatCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                    StopCoroutine(combatCoroutine);
                }
            }
        }
    }

    private bool IsPlayerWithinDetectionRadius()
    {
   
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= 8f)
        {   
            return true;
        }
        return false;
    }


    private IEnumerator Combat()
    {
        while (HP > 0)
        {
            //anim.SetTrigger("RangedAttack");
            Invoke(nameof(RangedAttack), 0.5f); // Sätt tid till hur länge animationen körs
            yield return new WaitForSeconds(shootDelay);
        }
    }
    private void RangedAttack()
    {
        
        GameObject projectile = Instantiate(projectilePrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        BulletID bulletID = projectile.GetComponent<BulletID>(); //spar data om vem som skadar spelaren i kulan så vi kan räkna ut vem som dödade den !
        bulletID.KillerGameObject = gameObject;
        
        Vector2 direction = (player.position - projectile.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.GetComponent<Rigidbody2D>().rotation = angle;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * 3.5f;
    }

    private void CheckLineOfSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;

        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, directionToPlayer, 50f, playerMask);
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, directionToPlayer, 50f, obstacleMask);

        if (hitWall.collider.CompareTag("Wall") && hitPlayer.collider.CompareTag("Player") && hitPlayer.distance<hitWall.distance)
        {
            playerDetected = true;
                GameObject playerSpottedWarning = Instantiate(playerSpotted, transform.position, Quaternion.identity, transform);
                playerSpottedWarning.name = "PlayerSpotted";
                playerSpottedWarning.GetComponent<Animator>().SetTrigger("PlayerDetected");
                Destroy(playerSpottedWarning, 1f);
                combatCoroutine = StartCoroutine(Combat());
                moveCoroutine = StartCoroutine(MoveAround());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerAttack") )
        {
            Destroy(other.gameObject);
            HP -= 1 * dmgMultiplier; // damagemultiplier to be added
            StartCoroutine(Flash());
            
        }

        if (HP <= 0)
        {


            OnDied();
            isDead = true;


            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            if (combatCoroutine != null)
            {
                StopCoroutine(combatCoroutine);
            }
            sprd.color = Color.red;
            dropLoot();
            Destroy(gameObject, 1f);
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


            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
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
        SingletonClass.OnEnemyKilled();
        
        SingletonClass.AwardXP(xpToAwardPlayerForKillingEnemy);
    }

    public override bool GetIsChasingPlayer()
    {
        return playerDetected;
    }

    
    private void dropLoot()
    {
        if (droppedLoot) return;
        droppedLoot = true;
        GetComponent<LootBag>().InstantiateLoot(transform.position);
    }

    private IEnumerator Flash()
    {
        if (isDead) yield break;
        sprd.material = flashOnHit;
        yield return new WaitForSeconds(0.125f);
        sprd.material = originalMat;
    }

    private void damageCooldownReset()
    {
       canDealDamage = true;
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
        //StartCoroutine(EnemyAI.TempDmgIncrease(1, 60));
    }
}
