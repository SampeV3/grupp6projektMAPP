using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemyAI : MonoBehaviour
{
    //[SerializeField] private Material flashOnHit;
    [SerializeField] private GameObject projectilePrefab;
    //[SerializeField] private AudioClip projectileSFX;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask playerMask, obstacleMask;

    private AudioSource audioSource;
    private Material originalMat;
    private SpriteRenderer sprd;
    private Animator anim;
    private int HP;
    private bool checkLOS = false;
    private Coroutine moveCoroutine, combatCoroutine;

    void Start()
    {
        HP = 100;
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        //originalMat = sprd.material;
        
        moveCoroutine = StartCoroutine(MoveAround());
    }

    private IEnumerator MoveAround()
    {
        while (true)
        {
            float distance = 0f;
            Vector2 randomDirection = Vector2.zero;
            while (distance < 4f)
            {
                randomDirection = Random.insideUnitCircle.normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, randomDirection, 50f,obstacleMask);
                if (hit.collider != null && hit.collider.CompareTag("Wall") && hit.distance >= 4f)
                {
                    distance = hit.distance;
                }
            }
            Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y) + randomDirection * Random.Range(1f,distance-1.9f);
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
            yield return new WaitForSeconds(2f);
        }
    }

    private void Update()
    {
        if (checkLOS)
        {
            CheckLineOfSight();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
        checkLOS = true;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            checkLOS = false;
            StopCoroutine(combatCoroutine);

        }
    }

    private IEnumerator Combat()
    {
        while (HP > 0)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist >= 0f)
            {

                anim.SetTrigger("RangedAttack");
                Invoke("RangedAttack", 0.5f); // Sätt tid till hur länge animationen körs
                yield return new WaitForSeconds(2.5f);
            }
            else
            {
                // MeleeAttack();
                yield return new WaitForSeconds(2.5f);
            }

        }
    }
    private void RangedAttack()
    {
        
        GameObject projectile = Instantiate(projectilePrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        Vector2 direction = (player.position - projectile.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.GetComponent<Rigidbody2D>().rotation = angle;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * 3.5f;
    }

    private void CheckLineOfSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;

        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, directionToPlayer, 10f, playerMask);
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, directionToPlayer, 10f, obstacleMask);

        if (hitWall.collider.CompareTag("Wall") && hitPlayer.collider.CompareTag("Player") && hitPlayer.distance<hitWall.distance)
        {
            checkLOS = false;
            StopCoroutine(moveCoroutine);
            combatCoroutine = StartCoroutine(Combat());
        }
    }
    private IEnumerator Flash()
    {
        //sprd.material = flashOnHit;
        yield return new WaitForSeconds(0.125f);
        sprd.material = originalMat;
    }
}
