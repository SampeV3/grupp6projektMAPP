using System.Collections;
using UnityEngine;


public class SpearAI : EnemyMonoBehaviour
{
    [SerializeField] private Material flashOnHit;
    [SerializeField] private GameObject playerSpotted;
    //[SerializeField] private AudioClip projectileSFX;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask playerMask, obstacleMask;


    private AudioSource audioSource;
    private Material originalMat;
    private SpriteRenderer sprd;
    private Animator anim;
    public int xpToAwardForKillingEnemy = 50;
    private bool playerDetected = false;
    private bool canhitSomething = false;
    private bool isDead, droppedLoot = false;
    private Coroutine combatCoroutine, rotateCoroutine;
    private GameObject playerSpottedWarning;
    private bool hitSomething = false;
    private bool canDealDamage = true;
    private float rotateFactor;
    private static int dmgMultiplier = 1;

    public override bool GetIsChasingPlayer()
    {
        return playerDetected;
    }

    void Start()
    {
        HP = 10;
        audioSource = GetComponent<AudioSource>();
        //anim = GetComponent<Animator>();
        sprd = GetComponent<SpriteRenderer>();
        originalMat = sprd.material;
        anim = GetComponent<Animator>();



    }

    protected override void Awake()
    {
        base.Awake(); // Make sure to always keep that line
        if (player == null)
        {
            player = IsPlayer.FindPlayerTransformAutomaticallyIfNull(); //Tillagt av Elias
        }
    }


    private void Update()
    {
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
                    StopCoroutine(combatCoroutine);
                    StopCoroutine(rotateCoroutine);
                }
            }
        }
    }

    private bool IsPlayerWithinDetectionRadius()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= 15)
        {
            return true;
        }
        return false;
    }


    private IEnumerator Combat()
    {
        while (HP > 0)
        {
            rotateCoroutine = StartCoroutine(RotateTowardsPlayer());
            anim.SetTrigger("Charge");
            rotateFactor = 100f;
            yield return new WaitForSeconds(2.5f);

            anim.SetTrigger("Dash");
            Invoke("CooldownReset", 0.03f);
            hitSomething = false;
            rotateFactor = 35f;
            while (!hitSomething)
            {
                transform.position = transform.position + -transform.up * 8f * Time.deltaTime;
                yield return null;
            }
            StopCoroutine(rotateCoroutine);
            anim.SetTrigger("Stunned");
            canhitSomething = false;
            Vector3 originalPosition = transform.position;
            float shakeMagnitude = 0.05f;
            foreach (BoxCollider2D collider in GetComponents<BoxCollider2D>())
            {
                collider.enabled = false;
            }
            for (int i = 0; i < 100; i++)
            {
                Vector3 shakeOffset = new Vector3(Random.Range(-shakeMagnitude, shakeMagnitude), Random.Range(-shakeMagnitude, shakeMagnitude), 0f);
                transform.position = originalPosition + shakeOffset;
                yield return new WaitForSeconds(0.01f);
            }
            anim.SetTrigger("Reverse");
            float elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                transform.position = transform.position + transform.up * 0.8f * Time.deltaTime;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            foreach (BoxCollider2D collider in GetComponents<BoxCollider2D>())
            {
                collider.enabled = true;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void CooldownReset()
    {
        canhitSomething = true;
    }


    private IEnumerator RotateTowardsPlayer()
    {
        while (true)
        {
            Vector2 directionToPlayer = player.position - transform.position;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            Quaternion desiredRotation = Quaternion.Euler(0f, 0f, angle + 90f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotateFactor * Time.deltaTime);
            yield return null;
        }
    }

    private void CheckLineOfSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;

        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, directionToPlayer, 17f, playerMask);
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, directionToPlayer, 17f, obstacleMask);

        if (hitWall.distance == 0)
        {
            hitWall.distance = 100;
        }
        if (hitPlayer.collider.CompareTag("Player") && hitPlayer.distance < hitWall.distance)
        {
            playerDetected = true;
            GameObject playerSpottedWarning = Instantiate(playerSpotted, transform.position, Quaternion.identity, transform);
            playerSpottedWarning.name = "PlayerSpotted";
            playerSpottedWarning.GetComponent<Animator>().SetTrigger("PlayerDetected");
            Destroy(playerSpottedWarning, 1f);
            combatCoroutine = StartCoroutine(Combat());
        }
    }

    public override void TakeDamage()
    {
        HP -= 1 * dmgMultiplier;
        StartCoroutine(Flash());
        if (HP <= 0)
        {
            isDead = true;
            if (combatCoroutine != null)
            {
                StopCoroutine(combatCoroutine);
                StopCoroutine(rotateCoroutine);
            }
            sprd.color = Color.red;
            dropLoot();
            Destroy(gameObject, 1f);
        }



    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerAttack"))
        {
            Destroy(other.gameObject);
            TakeDamage();
        }
        if (canhitSomething)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                hitSomething = true;
            }
            if (other.gameObject.CompareTag("Player"))
            {
                hitSomething = true;
            }
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


            
            isDead = true;


            if (rotateCoroutine != null)
             {
                 StopCoroutine(rotateCoroutine);
             }
            
            if (combatCoroutine != null)
            {
                StopCoroutine(combatCoroutine);
            }
            sprd.color = Color.red;
            OnDied();
            dropLoot();
            Destroy(gameObject, 1f);
        }
    }

    private void dropLoot()
    {
        if (!droppedLoot)
        {
            droppedLoot = true;
            GetComponent<LootBag>().InstantiateLoot(transform.position);

        }

    }

    private bool hasRunned = false;
    private void OnDied()
    {
        if (hasRunned) return;
        hasRunned = true;
        SingletonClass.OnEnemyKilled(this);

        SingletonClass.AwardXP(xpToAwardForKillingEnemy);
    }


    private IEnumerator Flash()
    {
        if (!isDead)
        {
            sprd.material = flashOnHit;
            yield return new WaitForSeconds(0.125f);
            sprd.material = originalMat;

        }
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
