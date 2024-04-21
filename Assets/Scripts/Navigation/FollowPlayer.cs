using System;
using UnityEngine;
using UnityEngine.AI;

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

    private void OnDrawGizmos()
    {
        Navigate.DrawGizmos(agent, showPath, showAhead);
    }
}