using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MortarAI : MonoBehaviour
{
    [SerializeField] private Material flashOnHit;
    [SerializeField] private GameObject mortarIndicator;
    [SerializeField] private Transform player;
    //[SerializeField] private AudioClip projectileSFX;
    
    private Animator anim;
    private AudioSource audioSource;
    
    private SpriteRenderer sprd;
    private Material originalMat;
    private int HP;
    private bool playerDetected;
    private Coroutine combatCoroutine;
    private bool isDead = false;
    void Start()
    {
        HP = 10;
        audioSource = GetComponent<AudioSource>();
        sprd = GetComponent<SpriteRenderer>();
        originalMat = sprd.material;
    }

    private void Awake()
    {
        if (player == null)
        {
            player = IsPlayer.FindPlayerTransformAutomaticallyIfNull();
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
        Transform desiredChild = mortarAim.transform.Find("MortarBackground");
        SpriteRenderer mortarsprd = desiredChild.GetComponent<SpriteRenderer>();
        Color ogColor = mortarsprd.color;
        ogColor.a = 0.1f;
        mortarsprd.color = ogColor;
        while (mortarsprd.color.a < 1f)
        {
            mortarAim.transform.Rotate(0f, 0f, 15f * Time.fixedDeltaTime);
            ogColor.a += 0.5f * Time.deltaTime;
            mortarsprd.color = ogColor;
            yield return null;
            if(HP <= 0) { Destroy(mortarAim); }
        }
        mortarAim.GetComponent<CircleCollider2D>().enabled = true;
        Destroy(mortarAim, 0.1f);
    }
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerAttack"))
        {
            Destroy(other.gameObject);
            HP -= 1;
            OnDied();
            StartCoroutine(Flash());
            if (HP <= 0)
            {
                isDead = true;
                if (combatCoroutine != null)
                {
                    StopCoroutine(combatCoroutine);
                }
                flashOnHit = sprd.material;
                sprd.color = Color.red;
                Destroy(gameObject, 1f);
            }
        }
    }

    private bool hasRunned = false;
    private void OnDied()
    {
        if (hasRunned) return;
        hasRunned = true;
        SingletonClass.OnEnemyKilled();
        int XP_TO_AWARD_PLAYER_FOR_KILLING_ENEMY = 10;
        SingletonClass.AwardXP(XP_TO_AWARD_PLAYER_FOR_KILLING_ENEMY);
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
        return false;
    }
}