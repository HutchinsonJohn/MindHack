using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles player movement, camera, shooting, and hacking
/// </summary>
public class PlayerMovement : MonoBehaviour
{

    private CharacterController controller;
    private CharacterController characterController;
    private CharacterController hackController;
    private PlayerController playerController;
    private Actions actions;
    private Animator playerAnimator;
    private Camera viewCamera;
    private Transform camTransform;
    private FieldOfView fow;
    /// <summary>
    /// Transform the camera should follow, either hacked enemy or player
    /// </summary>
    private Transform transformTarget;
    private Transform hackedTarget;
    private Animator animatorTarget;
    private GameObject[] enemies;

    // Camera
    /// <summary>
    /// camera distance from player, more accurately its height times sin(cameraAngle), when not blocked by an object
    /// </summary>
    private float distance = 10f;
    private float cameraAngle = 45f;
    public Vector3 lookAt;

    // Movement
    private float leftRightInput, forwardBackwardInput;
    private float moveSpeed = 4f;
    private float turnSpeed = 480f;

    // Detection
    public bool alerted;
    public bool wasAlerted;
    public bool wasDetected;

    // Weapons
    private bool aiming;
    public bool rifleEquipped;
    private Vector3 gunHeight = new(0, 1.4f, 0);

    // Hacking
    private bool hacked;
    public float hackMeter;
    private float maxHackDuration = 15f;

    // Health
    // TODO: maxHealth
    public bool isDying;
    private int health = 5; //5 is max health
    private float regenCooldown = 0f;

    // Room
    public float roomLeftLimit = .5f;
    public float roomRightLimit = 24.5f;

    // Defeated Enemies
    public int killedEnemies = 0;
    public int sleptEnemies = 0;
    public int hackedEnemies = 0;

    // LayerMasks
    public LayerMask targetLayersMask;
    public LayerMask obstacleMask;
    public LayerMask lookPlane;

    // UI
    public GameObject hackBar;
    public Image hackBarImage;
    public GameObject gameOverCanvas;
    public Image healthMeter;

    // Sounds
    public AudioSource akShot;
    public AudioSource tranqShot;

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
        ThirdPersonCamera();
        BGM.Instance.Play();
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

        // TODO: Cleanup shooting behavior or move to another class
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
                    transformTarget.tag = "Player";
                    transformTarget.gameObject.layer = LayerMask.NameToLayer("Player");
                }

                if (Physics.Raycast(transformTarget.position + gunHeight, transformTarget.forward, out RaycastHit hit, 100, targetLayersMask))
                {

                    if (hit.transform.CompareTag("Enemy"))
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
                            }
                            else
                            {
                                hit.transform.SendMessage("Slept");
                                sleptEnemies++;
                            }
                        }
                    }
                }

                if (rifleEquipped || hacked)
                {
                    akShot.Play();
                    foreach (GameObject enemy in enemies)
                    {
                        enemy.SendMessage("Caution", transformTarget.position);
                    }
                }
                else
                {
                    tranqShot.Play();
                }
            }
        }
        ThirdPersonCamera();
        fow.FindTarget();

        if (!alerted)
        {
            Hack();
            if (wasAlerted)
            {
                BGM.Instance.Unseen();
            }
            wasAlerted = false;
        }
        else
        {
            if (hacked)
            {
                EndHack();
            }
            if (!wasAlerted)
            {
                BGM.Instance.Spotted();
            }
            wasAlerted = true;
        }

        alerted = false;
    }

    /// <summary>
    /// Handles player movement and rotation smoothing
    /// </summary>
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

    /// <summary>
    /// Handles targeting and rotation while aiming
    /// </summary>
    private void Look()
    {
        Ray mousePos = viewCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mousePos, out RaycastHit hit, 10000f, lookPlane);
        lookAt = new Vector3(hit.point.x, transformTarget.position.y, hit.point.z);
        //Debug.DrawLine(transform.position, lookAt, Color.red);
        if (aiming)
        {
            transformTarget.LookAt(lookAt);
        }
    }

    /// <summary>
    /// Handles camera that follows the player
    /// </summary>
    private void ThirdPersonCamera()
    {
        Vector3 dir = new(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(cameraAngle, 0, 0);
        Vector3 pos = transformTarget.position + rotation * dir;
        if (Physics.Raycast(transformTarget.position, rotation * dir, out RaycastHit hit, distance + .5f, obstacleMask))
        {
            pos.z = hit.point.z + .25f;
            float x = Mathf.Sqrt(Mathf.Pow(distance + .5f, 2) - Mathf.Pow(hit.point.z - transformTarget.position.z, 2)) + transformTarget.position.y;
            pos.y = x;
        }

        pos.x = Mathf.Max(roomLeftLimit, Mathf.Min(pos.x, roomRightLimit));

        Vector3 smooth = Vector3.Lerp(camTransform.position, pos, 5 * Time.deltaTime);
        smooth.x = pos.x;
        camTransform.position = smooth;

        camTransform.LookAt(transformTarget);
    }

    /// <summary>
    /// Sets alerted and wasDetected to true
    /// </summary>
    private void Alerted()
    {
        alerted = true;
        wasDetected = true;
    }

    /// <summary>
    /// Handles hacking behavior
    /// </summary>
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

        bool targetFound = fow.FindIndirectTarget(lookAt);
        // TODO: Technically the player can transfer hack progress to another enemy if they are both in range and player rotates from one to the other while hacking.  Something should prevent this
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
        }
        else
        {
            hackMeter = 0;
        }
    }

    /// <summary>
    /// Cancels the hack
    /// </summary>
    private void EndHack()
    {
        hackBar.SetActive(false);
        controller = characterController;
        hacked = false;
        hackedTarget.SendMessage("MindHacked");
        transformTarget = transform;
        animatorTarget = playerAnimator;
        animatorTarget.SetBool("Squat", false);
        aiming = false;
        hackedEnemies++;
    }

    private void Hit()
    {
        if (!isDying)
        {
            if (hacked)
            {
                controller = characterController;
                hacked = false;
                hackedTarget.SendMessage("Killed");
                transformTarget = transform;
                animatorTarget = playerAnimator;
                animatorTarget.SetBool("Squat", false);
                aiming = false;
                killedEnemies++;
            }
            else
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

    /// <summary>
    /// Sets the game over screen to active
    /// </summary>
    private void GameOver()
    {
        gameOverCanvas.SetActive(true);
    }

    /// <summary>
    /// Updates the hackBar
    /// </summary>
    /// <returns></returns>
    IEnumerator HackedCoroutine()
    {
        hackBar.SetActive(true);
        float hackDuration = 0;
        while (hackDuration < maxHackDuration)
        {
            hackBarImage.fillAmount = Mathf.Lerp(.075f, 1, ((maxHackDuration - hackDuration) / maxHackDuration));
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
