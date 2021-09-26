using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles enemy patrolling behavior
/// </summary>
public class PatrolPath : MonoBehaviour
{

    public Transform[] patrolPoints;
    public float[] patrolPauseTimes;
    public float[] lookDirections;

    private NavMeshAgent agent;
    private EnemyAI enemyAI;
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
        enemyAI = GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(patrolPoints[listPosition].position);
    }

    /// <summary>
    /// Handles enemy patrolling behavior
    /// </summary>
    public void Patrol()
    {
        if (offPath)
        {
            offPath = false;
            isWaiting = false;
            agent.SetDestination(patrolPoints[listPosition].position);
        }
        else if (isWaiting)
        {
            if (lookDirections[listPosition] != -1)
            {
                Quaternion look = Quaternion.Euler(0, lookDirections[listPosition], 0);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    look,
                    turnSpeed * Time.deltaTime
                );
            }
            return;
        }
        if (!agent.hasPath)
        {

            isWaiting = true;
            Invoke(nameof(StopWaiting), patrolPauseTimes[listPosition]);
        }
    }

    /// <summary>
    /// Gives enemy new patrol destination if they are alive and not offPath
    /// </summary>
    private void StopWaiting()
    {
        listPosition++;
        if (listPosition >= patrolPoints.Length)
        {
            listPosition = 0;
        }
        isWaiting = false;
        if (!offPath && !enemyAI.killed && !enemyAI.hacked)
        {
            agent.SetDestination(patrolPoints[listPosition].position);
        }

    }

    /// <summary>
    /// Sets offPath to true
    /// </summary>
    public void OffPath()
    {
        offPath = true;
    }

}
