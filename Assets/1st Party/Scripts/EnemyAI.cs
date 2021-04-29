using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private Transform target;
    NavMeshAgent agent;
    private PatrolPath patrol;
    private FieldOfView fow;
    private Actions actions;
    private Animator animator;
    private PlayerController playerController;
    private int alertState; // 0 = patrol, 1 = caution, 2 = searching, 3 = engaging
    private float engageDistance = 10f;
    private float shootingDistance = 5f;
    private bool playerSpotted;
    private float turnAngle = 75f;
    private bool hacked;
    private bool killed;
    private bool arrived;
    private CharacterController characterController;

    Coroutine lookCoroutine;
    Coroutine searchCoroutine;
    Coroutine cautionCoroutine;
    Coroutine discoverCoroutine;
    Coroutine shootingCoroutine;

    public Transform[] otherEnemies;

    private Transform lastSpotted;

    private LayerMask enemyMask;
    public LayerMask targetLayersMask;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrol = GetComponent<PatrolPath>();
        fow = GetComponent<FieldOfView>();
        line = GetComponent<LineRenderer>();
        actions = GetComponent<Actions>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        playerController.SetArsenal("AK-74M");
        enemyMask = LayerMask.NameToLayer("Enemy");
        characterController = GameObject.Find("Player").GetComponent<CharacterController>();
    }

    void Hacked()
    {
        hacked = true;
        StopAllCoroutines();
        // TODO: change layer
        agent.isStopped = true;
    }

    void Killed()
    {
        killed = true;
        StopAllCoroutines();
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        agent.isStopped = true;
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
            //Debug.Log(alertState);
            EnemyBehavior();

            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        
    }

    private void EnemyBehavior()
    {
        if (fow.FindTarget())
        {
            playerSpotted = true;
            lastSpotted = fow.viewTarget;
            agent.SetDestination(lastSpotted.position);
            StartCoroutine(GetPath());
            transform.LookAt(lastSpotted.position);
            agent.stoppingDistance = 5;
            arrived = false;
            patrol.OffPath();
            switch (alertState)
            {
                case 0:
                    if (Vector3.Distance(transform.position, lastSpotted.position) > 10)
                    {
                        //pause, then walk towards
                        if (cautionCoroutine == null)
                            cautionCoroutine = StartCoroutine(CautionCoroutine());
                    }
                    else
                    {
                        //play alert sound
                        if (discoverCoroutine == null)
                            discoverCoroutine = StartCoroutine(DiscoverCoroutine(lastSpotted.position));
                    }
                    break;
                case 1:
                    if (Vector3.Distance(transform.position, lastSpotted.position) > 10)
                    {
                        //continute to destination
                    }
                    else
                    {
                        //play alert sound
                        agent.isStopped = true;
                        if (discoverCoroutine == null)
                            discoverCoroutine = StartCoroutine(DiscoverCoroutine(lastSpotted.position));
                    }
                    break;
                case 2:
                    alertState = 3;
                    NotfiyOthers(lastSpotted.position);
                    break;
                default:
                    lastSpotted.SendMessage("Alerted");
                    if (Vector3.Distance(transform.position, lastSpotted.position) < 8)
                    {
                        if (shootingCoroutine == null)
                            shootingCoroutine = StartCoroutine(ShootingCoroutine());

                        //actions.Aiming();
                        //agent.isStopped = true;

                        //Shoot at every so many seconds
                        //actions.Attack();
                    } else
                    {
                        if (shootingCoroutine != null)
                        {
                            StopCoroutine(shootingCoroutine);
                            shootingCoroutine = null;
                        }
                        agent.isStopped = false;
                        animator.SetBool("Aiming", false);
                    }
                    //engage
                    NotfiyOthers(fow.viewTarget.position);
                    break;
            }
        }
        else
        {
            playerSpotted = false;
            agent.isStopped = false;
            animator.SetBool("Aiming", false);
            if (shootingCoroutine != null)
            {
                StopCoroutine(shootingCoroutine);
                shootingCoroutine = null;
            }

            switch (alertState)
            {
                case 1:
                    agent.stoppingDistance = 0;
                    if (agent.remainingDistance <= agent.stoppingDistance || !agent.hasPath)
                    {
                        if (lookCoroutine == null)
                            lookCoroutine = StartCoroutine(LookCoroutine());
                    }

                    break;
                case 2:
                    agent.stoppingDistance = 0;
                    arrived = false;
                    //look around the area, then resume patrol
                    if (searchCoroutine == null)
                        searchCoroutine = StartCoroutine(SearchCoroutine());
                    break;
                case 3:
                    //continue to destination, look around, then switch to searching state
                    if (lastSpotted != null)
                    {
                        lastSpotted.SendMessage("Alerted");
                    }
                    
                    if (arrived)
                    {
                        agent.stoppingDistance = 5;
                    } else
                    {
                        agent.stoppingDistance = 1;
                    }
                    if (agent.remainingDistance <= agent.stoppingDistance || !agent.hasPath)
                    {
                        NotifyArrived();
                        if (lookCoroutine == null)
                            lookCoroutine = StartCoroutine(LookCoroutine());
                    }
                    break;
                default:
                    agent.stoppingDistance = 0;
                    patrol.Patrol();
                    StartCoroutine(GetPath());
                    break;
            }
        }
    }

    /// <summary>
    /// Called when an enemy is at alertState 3, does nothing when a hacked enemy is spotted by another enemy
    /// </summary>
    void Alerted()
    {

    }

    void NotfiyOthers(Vector3 pos)
    {
        foreach (Transform enemy in otherEnemies)
        {
            enemy.SendMessage("Alert", pos);
        }
    }

    void Alert(Vector3 pos)
    {
        if (!killed && !hacked) //&& not sleep
        {  
            alertState = 3;
            agent.SetDestination(pos);
        }
    }

    void Caution(Vector3 pos)
    {
        if (alertState == 0)
        {
            if (cautionCoroutine == null)
                cautionCoroutine = StartCoroutine(CautionCoroutine());
        }
        
        agent.SetDestination(pos);
    }

    void Arrived()
    {
        arrived = true;
    }

    void NotifyArrived()
    {
        arrived = true;
        foreach (Transform enemy in otherEnemies)
        {
            enemy.SendMessage("Arrived");
        }
    }


    Vector3 gunHeight = new Vector3(0, 1.4f, 0);
    private float movementShotSpreadCoefficient = 0.03f;
    private float stationaryShotSpread = 0.05f;
    private void Shoot()
    {
        if (Physics.Raycast(transform.position + gunHeight, transform.forward, out RaycastHit hit, 100, targetLayersMask))
        {
            if (hit.transform.gameObject.layer != enemyMask)
            {
                animator.SetTrigger("Attack");
                float shotSpread = characterController.velocity.magnitude * movementShotSpreadCoefficient + stationaryShotSpread;
                if (Physics.Raycast(transform.position + gunHeight, transform.TransformDirection(new Vector3((1 - 2 * Random.value) * shotSpread, (1 - 2 * Random.value) * shotSpread, 1)), out hit, 100, targetLayersMask))
                {
                    if (hit.transform.name.Equals("Player"))
                    {
                        Debug.Log("hit");
                    }
                    
                    //TODO: Send damage to player or enemy
                }
            }
        }
    }

    IEnumerator ShootingCoroutine()
    {
        agent.isStopped = true;
        animator.SetBool("Aiming", true);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Aiming"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        Shoot();
        yield return new WaitForSeconds(0.5f);
        Shoot();
        yield return new WaitForSeconds(0.5f);
        Shoot();
        yield return new WaitForSeconds(1f);

        shootingCoroutine = null;
    }

    IEnumerator RotationCoroutine(Quaternion lookTowards)
    {
        while (Mathf.Abs(Quaternion.Dot(transform.rotation, lookTowards)) < 0.9999999f)
        {
            if (playerSpotted)
            {
                if (lookCoroutine != null)
                {
                    StopCoroutine(lookCoroutine);
                    lookCoroutine = null;
                } else if (searchCoroutine != null) 
                {
                    StopCoroutine(searchCoroutine);
                    searchCoroutine = null;
                }
                yield break;
            }
            // Rotate at a consistent speed toward the target.
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                lookTowards,
                turnAngle * Time.deltaTime
            );

            // Wait a frame, then resume.
            yield return null;
        }
    }

    IEnumerator LookCoroutine()
    {
        // Until our current rotation aligns with the target...
        Quaternion forward = transform.rotation;
        Vector3 euler = forward.eulerAngles;
        Quaternion left = Quaternion.Euler(euler.x, euler.y - turnAngle, euler.z);
        Quaternion right = Quaternion.Euler(euler.x, euler.y + turnAngle, euler.z);

        yield return StartCoroutine(RotationCoroutine(right));

        yield return new WaitForSeconds(1);

        yield return StartCoroutine(RotationCoroutine(left));
        
        yield return new WaitForSeconds(1);

        yield return StartCoroutine(RotationCoroutine(forward));

        alertState--;
        
        // Clear the coroutine so the next input starts a fresh one.
        lookCoroutine = null;
    }

    IEnumerator SearchCoroutine()
    {
        // Walks 5 meters forward, or to closest obstacle
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 5.5f, default))
        {
            float dist = Vector3.Distance(transform.position, hit.point);
            if (dist > 0.5f) {
                agent.SetDestination(transform.position + transform.forward * (dist - .5f));
            }
        } else 
        {
            agent.SetDestination(transform.position + transform.forward * 5);
        }
        
        // Waits for agent to arrive at destination
        while (agent.remainingDistance <= agent.stoppingDistance || !agent.hasPath)
        {
            if (playerSpotted)
            {
                searchCoroutine = null;
                yield break;
            }
            yield return null;
        }

        Quaternion forward = transform.rotation;
        Vector3 euler = forward.eulerAngles;
        Quaternion left = Quaternion.Euler(euler.x, euler.y - turnAngle, euler.z);
        Quaternion right = Quaternion.Euler(euler.x, euler.y + turnAngle, euler.z);

        yield return StartCoroutine(RotationCoroutine(right));

        yield return new WaitForSeconds(1);

        yield return StartCoroutine(RotationCoroutine(left));

        yield return new WaitForSeconds(1);
        alertState = 0;

        searchCoroutine = null;
    }

    IEnumerator CautionCoroutine()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(1);
        agent.isStopped = false;
        alertState = 1;

        cautionCoroutine = null;
    }

    IEnumerator DiscoverCoroutine(Vector3 pos)
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(1);
        agent.isStopped = false;
        NotfiyOthers(pos);
        alertState = 3;

        discoverCoroutine = null;
    }

    LineRenderer line;

    IEnumerator GetPath()
    {
        line.SetPosition(0, transform.position); //set the line's origin

        yield return null; //wait for the path to generate

        DrawPath(agent.path);

        //agent.Stop();//add this if you don't want to move the agent
    }

    void DrawPath(NavMeshPath path)
    {
        if (path.corners.Length < 2) //if the path has 1 or no corners, there is no need
            return;

        line.positionCount = path.corners.Length; //set the array of positions to the amount of corners

        for (var i = 1; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]); //go through each corner and set that to the line renderer's position
        }
    }

}
