using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolPath : MonoBehaviour
{

    public Transform[] patrolPoints;
    public float[] patrolPauseTimes;
    public float[] lookDirections;
    public NavMeshAgent agent;
    private int listPosition;
    private bool isWaiting;
    private bool offPath;
    private float turnSpeed = 180f;

    // Start is called before the first frame update
    void Start()
    {
        if (patrolPoints.Length != patrolPauseTimes.Length && patrolPoints.Length != lookDirections.Length)
        {
            throw new System.ArgumentException("Patrol Points, Pause Times, and Look Directions must have same length");
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
        if (offPath)
        {
            offPath = false;
            isWaiting = false;
            agent.SetDestination(patrolPoints[listPosition].position);
        } else if (isWaiting)
        {
            Quaternion look = Quaternion.Euler(0, lookDirections[listPosition], 0);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                look,
                turnSpeed * Time.deltaTime
            );
            return;
        }
        if (!agent.hasPath)
        {
            
            isWaiting = true;
            Invoke(nameof(StopWaiting), patrolPauseTimes[listPosition]);
        }
    }

    private void StopWaiting()
    {
        listPosition++;
        if (listPosition >= patrolPoints.Length)
        {
            listPosition = 0;
        }
        isWaiting = false;
        if (!offPath)
        {
            agent.SetDestination(patrolPoints[listPosition].position);
        }
        
    }

    public void OffPath()
    {
        offPath = true;
    }

}
