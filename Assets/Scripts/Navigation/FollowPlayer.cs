using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

public class Chase : MonoBehaviour
{
    
    public static ArrayList otherEnemies = new ArrayList();
    [SerializeField] private Transform target;
    private NavMeshAgent agent;
    private bool stop;
    public bool showPath;
    public bool showAhead;
    public Vector3 offsetPosition = new Vector3(0, 0, 0);
    public bool alliedToPlayer = true;
    
    
    public GameObject projectilePrefab;
    public Transform rangedTarget;
    public int attackRange = 30;
    public int HP = 5;
    public float shootDelay = 1f;
    
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
        if (!alliedToPlayer)
        {
            otherEnemies.Add(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (!alliedToPlayer)
        {
            otherEnemies.Remove(this.gameObject);
        }
    }

    private void Awake()
    {
        if (target == null)
        {
            target = IsPlayer.FindPlayerTransformAutomaticallyIfNull();
        }
        StartCoroutine(Combat());
    }

    void FixedUpdate()
    {
        agent.speed = 8f;
        agent.SetDestination(target.position + offsetPosition);
    }
    
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
        foreach (GameObject enemy in otherEnemies)
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
        rangedTarget = alliedToPlayer ? FindNearestEnemy() : IsPlayer.FindPlayerTransformAutomaticallyIfNull();
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
        if (!alliedToPlayer)
        {
            BulletID bulletID = projectile.GetComponent<BulletID>(); //spar data om vem som skadar spelaren i kulan så vi kan räkna ut vem som dödade den !
            bulletID.KillerGameObject = gameObject;
        }
    }
    
    //control loop
    private IEnumerator Combat()
    {
        while (HP > 0)
        {
            CheckAttack();  
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
        HP -= damageAmount;
        OnHealthChanged();
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Navigate.DrawGizmos(agent, showPath, showAhead);
    }
    
}