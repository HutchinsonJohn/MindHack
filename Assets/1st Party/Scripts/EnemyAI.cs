using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    public Transform target;
    UnityEngine.AI.NavMeshAgent agent;
    private PatrolPath patrol;
    private FieldOfView fow;
    private int alertState; // 0 = patrol, 1 = caution, 2 = searching, 3 = engaging
    private float engageDistance = 10f;
    private float shootingDistance = 5f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrol = GetComponent<PatrolPath>();
        fow = GetComponent<FieldOfView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fow.FindTarget())
        {
            agent.SetDestination(fow.bestTarget.position);
            transform.LookAt(fow.bestTarget);
            agent.stoppingDistance = 5;
            switch (alertState)
            {
                case 0:
                    if (Vector3.Distance(transform.position, fow.bestTarget.position) > 10)
                    {
                        alertState = 1;
                        //pause, then walk towards
                    }
                    else
                    {
                        //play alert sound
                        //alert all other enemies that there is a target
                        alertState = 3;
                    }
                    break;
                case 1:
                    if (Vector3.Distance(transform.position, fow.bestTarget.position) > 10)
                    {
                        //continute to destination
                    }
                    else
                    {
                        //play alert sound
                        //alert all other enemies that there is a target
                        alertState = 3;
                    }
                    break;
                default:
                    //engage
                    break;
            }
        } else
        {
            switch (alertState)
            {
                case 1:
                    agent.stoppingDistance = 5;
                    //continue to destination, then look around in place, then resume patrol
                    break;
                case 2:
                    agent.stoppingDistance = 0;
                    //look around the area, then resume patrol
                    break;
                case 3:
                    agent.stoppingDistance = 0;
                    //continue to destination, look around, then switch to searching state
                    break;
                default:
                    agent.stoppingDistance = 0;
                    patrol.Patrol();
                    break;
            }   
        }
    }
}
