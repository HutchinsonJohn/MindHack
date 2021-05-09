using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    public LayerMask lookPlane;

    private CharacterController controller;
    private CharacterController characterController;
    private CharacterController hackController;
    private PlayerController playerController;
    private Actions actions;
    private Animator playerAnimator;
    private Camera viewCamera;
    private Transform camTransform;
    private FieldOfView fow;
    private Transform transformTarget; //Transform the camera should follow, either hacked enemy or player
    private Transform hackedTarget;
    private Animator animatorTarget;
    private GameObject[] enemies;

    // Variables
    private float leftRightInput, forwardBackwardInput;
    private float distance = 9f; //camera distance from player, more accurately its height times sin(cameraAngle), when not blocked by an object
    private float moveSpeed = 4f;
    private float cameraAngle = 45f;
    public float roomLeftLimit = .5f;
    public float roomRightLimit = 24.5f;
    private bool hacked;
    private float maxHackDuration = 10f;
    private bool aiming;
    public bool alerted;
    public bool rifleEquipped;
    Vector3 gunHeight = new Vector3(0, 1.4f, 0);
    public Vector3 lookAt;
    public float hackMeter;

    public int killedEnemies = 0;
    public int sleptEnemies = 0;
    public int hackedEnemies = 0;

    private int health = 5; //5 is max health
    private float regenCooldown = 0f;
    private bool isDying;
    public GameObject gameOverCanvas;
    public Image healthMeter;

    public LayerMask targetLayersMask;
    private LayerMask enemyMask;
    public LayerMask obstacleMask;

    // Start is called before the first frame update
    void Start()
    {
        viewCamera = Camera.main;
        camTransform = viewCamera.transform;
        fow = GetComponent<FieldOfView>();
        characterController = GetComponent<CharacterController>();
        controller = characterController;
        transformTarget = transform;
        actions = GetComponent<Actions>();
        playerAnimator = GetComponent<Animator>();
        animatorTarget = playerAnimator;
        playerController = GetComponent<PlayerController>();
        rifleEquipped = PlayerPrefs.GetInt("RifleEquipped") == 1;
        if (rifleEquipped)
        {
            playerController.SetArsenal("AK-74M");
        }
        else
        {
            playerController.SetArsenal("Pistol");
        }
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyMask = LayerMask.NameToLayer("Enemy");
        ThirdPersonCamera();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (isDying)
        {
            return;
        }

        if (PauseMenu.GameIsPaused)
        {
            animatorTarget.SetFloat("Speed", 0f);
            return;
        }

        if (health < 5)
        {
            if (regenCooldown <= 0f)
            {
                health++;
                healthMeter.fillAmount = 0.25f + health * 0.15f;
                regenCooldown = 2f;
            }
            else
            {
                regenCooldown -= Time.deltaTime;
            }
        }

        // Toggles aim stance
        if (Input.GetButtonDown("Fire2"))
        {
            if (aiming)
            {
                animatorTarget.SetBool("Aiming", false);
                aiming = false;
            }
            else
            {
                animatorTarget.SetBool("Aiming", true);
                animatorTarget.SetFloat("Speed", 0f);
                aiming = true;
            }
        }

        if (Input.GetButtonDown("Switch") && !hacked)
        {
            if (rifleEquipped)
            {
                playerController.SetArsenal("Pistol");
                rifleEquipped = false;
            }
            else
            {
                playerController.SetArsenal("AK-74M");
                rifleEquipped = true;
            }
            aiming = false;
        }

        if (!aiming)
        {
            Movement();
            animatorTarget.SetFloat("Speed", controller.velocity.magnitude);
            Look();
        }
        else
        {
            Look();

            if (Input.GetButtonDown("Fire1") && animatorTarget.GetCurrentAnimatorStateInfo(0).IsName("Aiming"))
            {
                animatorTarget.SetTrigger("Attack");

                if (hacked)
                {
                    transformTarget.gameObject.layer = LayerMask.NameToLayer("Player");
                }

                if (Physics.Raycast(transformTarget.position + gunHeight, transformTarget.forward, out RaycastHit hit, 100, targetLayersMask))
                {

                    if (hit.transform.gameObject.layer == enemyMask)
                    {
                        if (rifleEquipped || hacked)
                        {
                            hit.transform.SendMessage("Killed");
                            killedEnemies++;
                        }
                        else
                        {
                            if (alerted)
                            {
                                hit.transform.SendMessage("Damage");
                            } else
                            {
                                hit.transform.SendMessage("Slept");
                                sleptEnemies++;
                            }
                        }
                    }
                }

                if (rifleEquipped || hacked)
                {
                    foreach (GameObject enemy in enemies)
                    {
                        enemy.SendMessage("Caution", transformTarget.position);
                    }
                }
            }
        }
        ThirdPersonCamera();
        fow.FindTarget();

        if (!alerted)
        {
            Hack();
        } else if (hacked)
        {
            EndHack();
        }
        
        alerted = false;
    }

    private float turnSpeed = 480f;
    private void Movement()
    {
        leftRightInput = Input.GetAxisRaw("Horizontal");
        forwardBackwardInput = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(leftRightInput, 0, forwardBackwardInput).normalized;
        controller.SimpleMove(moveDirection * moveSpeed);
        if (moveDirection.magnitude > 0)
        {
            Quaternion look = Quaternion.LookRotation(moveDirection);
            transformTarget.rotation = Quaternion.RotateTowards(
                transformTarget.rotation,
                look,
                turnSpeed * Time.deltaTime
            );
        }
    }

    private void Look()
    {
        Ray mousePos = viewCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mousePos, out RaycastHit hit, lookPlane);
        lookAt = new Vector3(hit.point.x, transformTarget.position.y, hit.point.z);
        //Debug.DrawLine(transform.position, lookAt, Color.red);
        if (aiming)
        {
            transformTarget.LookAt(lookAt);
        }
    }

    private void ThirdPersonCamera()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(cameraAngle, 0, 0);
        Vector3 pos = transformTarget.position + rotation * dir;
        if (Physics.Raycast(transformTarget.position, rotation * dir, out RaycastHit hit, distance + .5f, obstacleMask))
        {
            pos.z = hit.point.z + .25f;
            float x = Mathf.Sqrt(Mathf.Pow(distance, 2) - Mathf.Pow(pos.z - transformTarget.position.z, 2)) + transformTarget.position.y;
            pos.y = x;
        }

        pos.x = Mathf.Max(roomLeftLimit, Mathf.Min(pos.x, roomRightLimit));

        Vector3 smooth = Vector3.Lerp(camTransform.position, pos, 5 * Time.deltaTime);
        smooth.x = pos.x;
        camTransform.position = smooth;

        camTransform.LookAt(transformTarget);
    }

    private void Alerted()
    {
        alerted = true;
    }

    private void Hack()
    {
        bool hackHeld = Input.GetButton("Hack");
        bool hackDown = Input.GetButtonDown("Hack");

        if (hacked)
        {
            if (hackDown)
            {
                EndHack();
            }
            return;
        }

        // TODO: change to hold until circle meter full to hack, .5 second to hack and unhack
        bool targetFound = fow.FindIndirectTarget(lookAt);
        if (targetFound)
        {
            if (hackMeter > 0.5f)
            {
                hackMeter = 0;

                animatorTarget.SetFloat("Speed", 0f);
                animatorTarget.SetBool("Squat", true);

                hackedTarget = fow.hackTarget;
                transformTarget = hackedTarget;
                hackedTarget.SendMessage("Hacked");

                //could make this one line
                hackController = hackedTarget.GetComponent<CharacterController>();
                controller = hackController;

                animatorTarget = hackedTarget.GetComponent<Animator>();

                aiming = false;
                hacked = true;
                hackedEnemies++;
                StartCoroutine(HackedCoroutine());
            }
            else
            {
                if (hackHeld)
                {
                    hackMeter += Time.deltaTime;
                }
                else
                {
                    hackMeter = 0;
                }
                fow.hackTarget.GetComponent<EnemyAI>().SendMessage("Hackable");
            }
        } else
        {
            hackMeter = 0;
        }
    }

    private void EndHack()
    {
        //Die (potential TODO: change to ragdoll)
        if (animatorTarget.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            animatorTarget.Play("Idle", 0);
        else
            animatorTarget.SetTrigger("Death");
        // TODO: change enemy layer

        controller = characterController;
        hacked = false;
        hackedTarget.SendMessage("MindHacked");
        transformTarget = transform;
        animatorTarget = playerAnimator;
        animatorTarget.SetBool("Squat", false);
        aiming = false;
    }

    private void Hit()
    {
        if (!isDying)
        {
            if (hacked)
            {
                EndHack();
            } else
            {
                health--;
                regenCooldown = 5f;
                actions.Damage();
                healthMeter.fillAmount = 0.25f + health * 0.15f;
            }
            if (health < 1)
            {
                isDying = true;
                playerAnimator.SetTrigger("Death");
                Invoke(nameof(GameOver), 3.5f);
            }
        }
    }

    private void GameOver()
    {
        gameOverCanvas.SetActive(true);
    }

    IEnumerator HackedCoroutine()
    {
        float hackDuration = 0;
        while (hackDuration < maxHackDuration)
        {
            if (!hacked)
            {
                yield break;
            }
            hackDuration += Time.deltaTime;
            //update hack time remaining meter
            yield return null;
        }
        EndHack();
    }

}
