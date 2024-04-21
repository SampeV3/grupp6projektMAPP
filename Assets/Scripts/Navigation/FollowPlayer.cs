using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Chase : MonoBehaviour
{
    [SerializeField] private Transform target;
    private NavMeshAgent agent;
    private bool stop;
    public bool showPath;
    public bool showAhead;

    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        StartCoroutine(Combat());
    }

    private void Awake()
    {
        if (target == null)
        {
            target = IsPlayer.FindPlayerTransformAutomaticallyIfNull();
        }
    }

    // Update is called once per frame
    void Update()
    {
        agent.speed = 8f;
        agent.SetDestination(target.position);
    }

    public GameObject projectilePrefab;
    public Transform rangedTarget;
    public int attackRange = 30;
    public int HP = 5;
    public float shootDelay = 1f;

    private Transform FindNearestEnemy()
    //TODO: optimize this, the game is -- suprisingly -- becoming laggy in the editor.
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
            
            Invoke(nameof(CheckAttack), 0.5f); // Sätt tid till hur länge animationen körs
            yield return new WaitForSeconds(shootDelay);
        }
    }
    
    
    
    private void OnDrawGizmos()
    {
        Navigate.DrawGizmos(agent, showPath, showAhead);
    }
}