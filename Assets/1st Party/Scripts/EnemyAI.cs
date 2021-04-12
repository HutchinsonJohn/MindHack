using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    public Transform target;
    NavMeshAgent agent;
    private PatrolPath patrol;
    private FieldOfView fow;
    private int alertState; // 0 = patrol, 1 = caution, 2 = searching, 3 = engaging
    private float engageDistance = 10f;
    private float shootingDistance = 5f;
    private bool playerSpotted;
    private float turnAngle = 75f;
    private bool hacked;
    private bool killed;

    Coroutine rotationCoroutine;
    Coroutine searchCoroutine;
    Coroutine cautionCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrol = GetComponent<PatrolPath>();
        fow = GetComponent<FieldOfView>();
    }

    void Hacked()
    {
        hacked = true;
    }

    void Killed()
    {
        killed = true;
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        agent.SetDestination(transform.position); //This is so fucking stupid, but resetting the path does not guarantee that a path that was being generated wont continue to be generated and be set later
    }

    // Update is called once per frame
    void Update()
    {
        if (killed) {
            //dying animation/ragdoll, then the corresponding knock out effect (probably split up message into slept, killed, mindhacked)
        }
        else if (hacked) {
            //probably handle everything in player movement
        } else {
            Debug.Log(alertState);
            EnemyBehavior();
        }
        
    }

    private void EnemyBehavior()
    {
        if (fow.FindTarget())
        {
            playerSpotted = true;
            agent.SetDestination(fow.viewTarget.position);
            transform.LookAt(fow.viewTarget);
            agent.stoppingDistance = 5;
            switch (alertState)
            {
                case 0:
                    if (Vector3.Distance(transform.position, fow.viewTarget.position) > 10)
                    {
                        //pause, then walk towards
                        if (cautionCoroutine == null)
                            cautionCoroutine = StartCoroutine(CautionCoroutine());
                    }
                    else
                    {
                        //play alert sound
                        //alert all other enemies that there is a target
                        alertState = 3;
                    }
                    break;
                case 1:
                    if (Vector3.Distance(transform.position, fow.viewTarget.position) > 10)
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
                case 2:
                    alertState = 3;
                    //alert all other enemies that there is a target
                    break;
                default:
                    //engage
                    break;
            }
        }
        else
        {
            playerSpotted = false;
            agent.stoppingDistance = 0;
            switch (alertState)
            {
                case 1:
                    if (agent.remainingDistance <= agent.stoppingDistance || !agent.hasPath)
                    {
                        if (rotationCoroutine == null)
                            rotationCoroutine = StartCoroutine(LookCoroutine());
                    }

                    break;
                case 2:
                    //look around the area, then resume patrol
                    if (searchCoroutine == null)
                        searchCoroutine = StartCoroutine(SearchCoroutine());
                    break;
                case 3:
                    //continue to destination, look around, then switch to searching state
                    if (agent.remainingDistance <= agent.stoppingDistance || !agent.hasPath)
                    {
                        if (rotationCoroutine == null)
                            rotationCoroutine = StartCoroutine(LookCoroutine());
                    }
                    break;
                default:
                    patrol.Patrol();
                    break;
            }
        }
    }

    IEnumerator LookCoroutine()
    {
        // Until our current rotation aligns with the target...
        Quaternion forward = transform.rotation;
        Vector3 euler = forward.eulerAngles;
        Quaternion left = Quaternion.Euler(euler.x, euler.y - turnAngle, euler.z);
        Quaternion right = Quaternion.Euler(euler.x, euler.y + turnAngle, euler.z);
        while (Quaternion.Dot(transform.rotation, right) < 1f)
        {
            if (playerSpotted)
            {
                rotationCoroutine = null;
                yield break;
            }
            // Rotate at a consistent speed toward the target.
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                right,
                turnAngle * Time.deltaTime
            );

            // Wait a frame, then resume.
            yield return null;
        }
        yield return new WaitForSeconds(1);
        while (Quaternion.Dot(transform.rotation, left) < 1f)
        {
            if (playerSpotted)
            {
                rotationCoroutine = null;
                yield break;
            }
            // Rotate at a consistent speed toward the target.
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                left,
                turnAngle * Time.deltaTime
            );

            // Wait a frame, then resume.
            yield return null;
        }
        yield return new WaitForSeconds(1);
        while (Quaternion.Dot(transform.rotation, forward) < 1f)
        {
            if (playerSpotted)
            {
                rotationCoroutine = null;
                yield break;
            }
            // Rotate at a consistent speed toward the target.
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                forward,
                turnAngle * Time.deltaTime
            );

            // Wait a frame, then resume.
            yield return null;
        }

        alertState--;
        
        // Clear the coroutine so the next input starts a fresh one.
        rotationCoroutine = null;
    }

    IEnumerator SearchCoroutine()
    {
        agent.SetDestination(transform.position + transform.forward * 5);
        while (agent.remainingDistance <= agent.stoppingDistance || !agent.hasPath)
        {
            yield return null;
        }
        Quaternion forward = transform.rotation;
        Vector3 euler = forward.eulerAngles;
        Quaternion left = Quaternion.Euler(euler.x, euler.y - turnAngle, euler.z);
        Quaternion right = Quaternion.Euler(euler.x, euler.y + turnAngle, euler.z);
        while (Quaternion.Dot(transform.rotation, right) < 1f)
        {
            if (playerSpotted)
            {
                searchCoroutine = null;
                yield break;
            }
            // Rotate at a consistent speed toward the target.
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                right,
                turnAngle * Time.deltaTime
            );

            // Wait a frame, then resume.
            yield return null;
        }
        yield return new WaitForSeconds(1);
        while (Quaternion.Dot(transform.rotation, left) < 1f)
        {
            if (playerSpotted)
            {
                searchCoroutine = null;
                yield break;
            }
            // Rotate at a consistent speed toward the target.
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                left,
                turnAngle * Time.deltaTime
            );

            // Wait a frame, then resume.
            yield return null;
        }
        yield return new WaitForSeconds(1);
        alertState = 0;

        searchCoroutine = null;
    }

    IEnumerator CautionCoroutine()
    {
        yield return new WaitForSeconds(1);
        alertState = 1;
    }

}
