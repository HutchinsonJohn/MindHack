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
    private bool playerSpotted;

    Coroutine _rotationCoroutine;

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
        //Debug.Log(alertState);
        if (fow.FindTarget())
        {
            playerSpotted = true;
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
            playerSpotted = false;
            switch (alertState)
            {
                case 1:
                    agent.stoppingDistance = 5;
                    if (!agent.hasPath)
                    {
                        //look around in place, then resume patrol
                        //look coroutine
                        if (_rotationCoroutine == null)
                            _rotationCoroutine = StartCoroutine(LookCoroutine());
                    }
                    
                    break;
                case 2:
                    agent.stoppingDistance = 0;
                    //look around the area, then resume patrol
                    break;
                case 3:
                    agent.stoppingDistance = 0;
                    //continue to destination, look around, then switch to searching state
                    if (!agent.hasPath)
                    {
                        if (_rotationCoroutine == null)
                            _rotationCoroutine = StartCoroutine(LookCoroutine());
                    }
                    break;
                default:
                    agent.stoppingDistance = 0;
                    patrol.Patrol();
                    break;
            }   
        }
    }

    IEnumerator LookCoroutine()
    {
        // Until our current rotation aligns with the target...
        while (Quaternion.Dot(transform.rotation, Quaternion.Euler(0,90,0)) < 1f)
        {
            if (playerSpotted)
            {
                yield return null;
            }
            // Rotate at a consistent speed toward the target.
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0, 90, 0),
                90f * Time.deltaTime
            );

            // Adjust our position to preserve the relationship to the pivot.
            //Vector3 offset = transform.TransformPoint(_localOffset);
            //transform.position = rotatePoint - offset;

            // Wait a frame, then resume.
            yield return null;
        }
        if (!playerSpotted)
        {
            alertState = 0;
        }
        
        // Clear the coroutine so the next input starts a fresh one.
        _rotationCoroutine = null;
    }

}
