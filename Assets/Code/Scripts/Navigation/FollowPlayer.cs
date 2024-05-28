using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

public class Chase : MonoBehaviour
{
    private static int alliesAlive = 0;

    [SerializeField] private Transform target;
    private NavMeshAgent agent;
    public bool showPath;
    public bool showAhead;
    private bool isDead = false;
    public Vector3 offsetPosition = new Vector3(0, -1, 0);

    public LayerMask obstacleMask, targetMask;
    private bool CheckLineOfSight(Transform startTransform, Transform target, float length)
    {
        Vector3 directionToPlayer = target.position - startTransform.position;

        RaycastHit2D hitPlayer = Physics2D.Raycast(startTransform.position, directionToPlayer, length, targetMask);
        RaycastHit2D hitWall = Physics2D.Raycast(startTransform.position, directionToPlayer, length, obstacleMask);

        if (hitWall.collider.CompareTag("Wall") && hitPlayer.collider.CompareTag("Player") && hitPlayer.distance < hitWall.distance)
        {
            return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        StartCoroutine(Combat());
    }



    private void Awake()
    {
        alliesAlive++;
        if (target == null)
        {
            target = IsPlayer.FindPlayerTransformAutomaticallyIfNull();
        }
    }

    private void OnDeath()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        agent.speed = 8f;
        agent.SetDestination(target.position + offsetPosition);
    }

    public GameObject projectilePrefab;
    public Transform rangedTarget;
    public int attackRange = 30;
    public double HP = 5;
    public float originalShootDelay = 1f;

    private Transform FindNearestEnemy()
    //TODO: optimize this, the game is -- suprisingly -- (sometimes) becoming laggy in the editor.
    {
        float closest = attackRange + 1;
        Transform closestEnemy = null;
        foreach (var enemy in EnemyMonoBehaviour.Instances)
        {
            float distance = Vector3.Distance(this.transform.position, enemy.gameObject.transform.position);
            if (distance < closest)
            {
                closest = distance;
                closestEnemy = enemy.gameObject.transform;
            }
        }

        return closestEnemy;
    }
    
    private void CheckAttack()
    {
        rangedTarget = FindNearestEnemy();
        if (rangedTarget != null && Vector3.Distance(transform.position, rangedTarget.position) < attackRange)
        {
            RangedAttack();   
        }
    }

    public void SetRangedTarget(Transform newRangedTarget)
    {
        rangedTarget = newRangedTarget;
    }
    
    private void RangedAttack()
    {
        GameObject projectile = Instantiate(projectilePrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        Vector2 direction = (rangedTarget.position - projectile.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.GetComponent<Rigidbody2D>().rotation = angle;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * 3.5f;
    }
    
    //control loop
    private IEnumerator Combat()
    {
        while (HP > 0)
        {
            CheckAttack();
            float bonusValue = (float)UIController.GetSkillModifier("Allied Fire Rate");
            float shootDelay = (2 - bonusValue) * originalShootDelay; 
            yield return new WaitForSeconds(shootDelay);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int damageAmount = 1;
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            //handle enemy bullet:
            TakeDamage(damageAmount, other);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Laser"))
        {
            TakeDamage(damageAmount, other);
        }
        else if (other.gameObject.CompareTag("MortarAttack"))
        {
            TakeDamage(damageAmount, other);
        }
    }

    private void OnHealthChanged()
    {
        print("allied unit damaged oh no! " + HP);
    }

    private void TakeDamage(int damageAmount, Collider2D other)
    {
        HP = HP * UIController.GetSkillModifier("Allied Health");
        HP -= damageAmount;
        OnHealthChanged();
        if (HP <= 0 && !isDead)
        {
            isDead = true;
            OnDeath();
        }
    }

    public static int GetAlliesAlive() //Make class attribute alliesAlive not settable outside this class to minimize the risk of undesired outcomes.
    {
        return alliesAlive;
    }

    //Called when the player is perma killed.
    public static void OnPermaDeath()
    {
        alliesAlive = 0;
    }

    private void OnDestroy()
    {
        alliesAlive--;
    }


    private void OnDrawGizmos()
    {
        Navigate.DrawGizmos(agent, showPath, showAhead);
    }
}