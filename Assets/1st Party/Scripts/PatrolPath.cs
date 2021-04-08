using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolPath : MonoBehaviour
{

    public Transform[] patrolPoints;
    public float[] patrolPauseTimes;
    public UnityEngine.AI.NavMeshAgent agent;
    private int listPosition;
    private bool isWaiting;

    // Start is called before the first frame update
    void Start()
    {
        if (patrolPoints.Length != patrolPauseTimes.Length)
        {
            throw new System.ArgumentException("Patrol Points and Pause Times must have same length");
        }
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(patrolPoints[listPosition].position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Patrol()
    {
        if (isWaiting)
        {
            return;
        }
        if (!agent.hasPath)
        {
            isWaiting = true;
            Invoke(nameof(StopWaiting), patrolPauseTimes[listPosition]);
            listPosition++;
            if (listPosition >= patrolPoints.Length)
            {
                listPosition = 0;
            }
        }
    }

    private void StopWaiting()
    {
        isWaiting = false;
        agent.SetDestination(patrolPoints[listPosition].position);
    }

}
