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
    private float engageDistance = 6f;
    private bool playerSpotted;
    private float turnAngle = 75f;
    public bool hacked; //Currently controlled by player
    public bool killed;
    private bool arrived;
    private CharacterController characterController;

    public GameObject sleepText;
    public GameObject mindHackedText;

    Coroutine lookCoroutine;
    Coroutine searchCoroutine;
    Coroutine cautionCoroutine;
    Coroutine discoverCoroutine;
    Coroutine shootingCoroutine;

    private GameObject[] allEnemies;

    private Transform lastSpotted;

    private LayerMask enemyMask;
    public LayerMask targetLayersMask;

    private bool hackable;
    private bool wasHackable;
    public GameObject hackCanvas;

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
        allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    /// <summary>
    /// While being controlled by the player
    /// </summary>
    void Hacked()
    {
        hacked = true;
        StopAllCoroutines();
        agent.enabled = false;
    }

    /// <summary>
    /// After player is done controlling enemy
    /// </summary>
    void MindHacked()
    {
        Invoke(nameof(MindHackedText), 3.5f);
        Killed();
    }

    void MindHackedText()
    {
        mindHackedText.SetActive(true);
    }

    /// <summary>
    /// If player shoots with pistol
    /// </summary>
    void Slept()
    {
        Invoke(nameof(SleptText), 3.5f);
        Killed();
    }

    void SleptText()
    {
        sleepText.SetActive(true);
    }

    /// <summary>
    /// If player shoots with AK
    /// </summary>
    void Killed()
    {
        killed = true;
        StopAllCoroutines();
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        agent.enabled = false;
        actions.Death();
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

    public void Hackable()
    {
        hackable = true;
    }

    private void LateUpdate()
    {
        if (hackable)
        {
            if (!wasHackable)
            {
                hackCanvas.SetActive(true);
            }
            wasHackable = true;
            
        } else
        {
            if (wasHackable)
            {
                hackCanvas.SetActive(false);
            }
            wasHackable = false;
        }
        hackable = false;
    }

    /// <summary>
    /// Handles enemy behavior according to alert state and if the player is in view
    /// </summary>
    private void EnemyBehavior()
    {
        if (fow.FindTarget())
        {
            playerSpotted = true;
            lastSpotted = fow.viewTarget;
            agent.SetDestination(lastSpotted.position);
            transform.LookAt(lastSpotted.position);
            agent.stoppingDistance = 5;
            arrived = false;
            patrol.OffPath();
            //StartCoroutine(GetPath()); //Debug
            switch (alertState)
            {
                case 0:
                    if (Vector3.Distance(transform.position, lastSpotted.position) > engageDistance)
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
                    if (Vector3.Distance(transform.position, lastSpotted.position) > engageDistance)
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
                    NotifyOthers(lastSpotted.position);
                    break;
                default:
                    lastSpotted.SendMessage("Alerted");
                    if (Vector3.Distance(transform.position, lastSpotted.position) < engageDistance)
                    {
                        if (shootingCoroutine == null && discoverCoroutine == null)
                            shootingCoroutine = StartCoroutine(ShootingCoroutine());

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
                    NotifyOthers(fow.viewTarget.position);
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
                    agent.stoppingDistance = 1;
                    if (agent.remainingDistance <= agent.stoppingDistance || !agent.hasPath)
                    {
                        if (lookCoroutine == null)
                        {
                            if (searchCoroutine != null)
                            {
                                StopCoroutine(searchCoroutine);
                                searchCoroutine = null;
                            }
                            lookCoroutine = StartCoroutine(LookCoroutine());
                        }
                            
                    }

                    break;
                case 2:
                    agent.stoppingDistance = 1;
                    arrived = false;
                    //look around the area, then resume patrol
                    if (searchCoroutine == null)
                    {
                        if (lookCoroutine != null)
                        {
                            StopCoroutine(lookCoroutine);
                            lookCoroutine = null;
                        }
                        searchCoroutine = StartCoroutine(SearchCoroutine());
                    }
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
                        {
                            if (searchCoroutine != null)
                            {
                                StopCoroutine(searchCoroutine);
                                searchCoroutine = null;
                            }
                            lookCoroutine = StartCoroutine(LookCoroutine());
                        }
                    }
                    break;
                default:
                    agent.stoppingDistance = 0;
                    patrol.Patrol();
                    break;
            }
        }
        StartCoroutine(GetPath()); //Debug
    }

    /// <summary>
    /// Called when an enemy is at alertState 3, does nothing when a hacked enemy is spotted by another enemy
    /// </summary>
    void Alerted()
    {

    }

    /// <summary>
    /// Alerts all other enemies of the players location
    /// </summary>
    /// <param name="pos">Position of the player</param>
    void NotifyOthers(Vector3 pos)
    {
        foreach (GameObject enemy in allEnemies)
        {
            enemy.SendMessage("Alert", pos);
        }
    }

    /// <summary>
    /// Called by other enemies when the player is spotted in alertState 3, sets destination to player and alertState = 3
    /// </summary>
    /// <param name="pos">Position of the player</param>
    void Alert(Vector3 pos)
    {
        if (!killed && !hacked) //&& not sleep
        {
            if (lookCoroutine != null)
            {
                StopCoroutine(lookCoroutine);
                lookCoroutine = null;
            }
            else if (searchCoroutine != null)
            {
                StopCoroutine(searchCoroutine);
                searchCoroutine = null;
            }
            if (alertState < 2)
            {
                if (discoverCoroutine == null)
                    discoverCoroutine = StartCoroutine(DiscoverCoroutine(pos));
            }
            else
            {
                alertState = 3;
            }
            agent.SetDestination(pos);
            if (agent.remainingDistance <= agent.stoppingDistance || !agent.hasPath)
            {
                transform.LookAt(pos);
            }
        }
    }

    /// <summary>
    /// Called if the player shoots an unsuppressed weapon, sets destination to player and starts caution routine if alertState == 0
    /// </summary>
    /// <param name="pos">Position of the player</param>
    void Caution(Vector3 pos)
    {
        if (!killed && !hacked) //&& not sleep
        {
            if (alertState == 0)
            {
                if (cautionCoroutine == null)
                    cautionCoroutine = StartCoroutine(CautionCoroutine());
            }

            agent.SetDestination(pos);
        }
    }

    /// <summary>
    /// Sets arrived = true
    /// </summary>
    void Arrived()
    {
        arrived = true;
    }

    /// <summary>
    /// Sets arrived to true for all enemies
    /// </summary>
    void NotifyArrived()
    {
        foreach (GameObject enemy in allEnemies)
        {
            enemy.SendMessage("Arrived");
        }
    }


    Vector3 gunHeight = new Vector3(0, 1.4f, 0);
    private float movementShotSpreadCoefficient = 0.03f;
    private float stationaryShotSpread = 0.05f;
    /// <summary>
    /// Handles individual shots and hit registration
    /// </summary>
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
                        hit.transform.SendMessage("Hit");
                    }
                    
                    //TODO: Send damage to player or enemy
                }
            }
        }
    }

    /// <summary>
    /// Handles enemy shooting pattern
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Rotates towards lookTowards over time
    /// </summary>
    /// <param name="lookTowards">The desired final look direction</param>
    /// <returns></returns>
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

    /// <summary>
    /// Handles enemy look in place behavior
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Handles enemy move and look behavior
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Handles enemy pause on caution behavior
    /// </summary>
    /// <returns></returns>
    IEnumerator CautionCoroutine()
    {
        agent.isStopped = true;
        alertState = 1;
        yield return new WaitForSeconds(1);
        agent.isStopped = false;

        cautionCoroutine = null;
    }

    /// <summary>
    /// Handles enemy discovery pause behavior
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    IEnumerator DiscoverCoroutine(Vector3 pos)
    {
        agent.isStopped = true;
        alertState = 3;
        yield return new WaitForSeconds(1);
        agent.isStopped = false;

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
