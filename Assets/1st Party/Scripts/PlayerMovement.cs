using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public LayerMask lookPlane;

    private CharacterController controller;
    private CharacterController characterController;
    private CharacterController hackController;
    private PlayerController playerController;
    private Animator animator;
    private Camera viewCamera;
    private Transform camTransform;
    private FieldOfView fow;
    private Transform transformTarget; //Transform the camera should follow, either hacked enemy or player
    private Transform hackedTarget;

    // Variables
    private float leftRightInput, forwardBackwardInput;
    private float distance = 7f; //camera distance from player, more accurately its height times sin(cameraAngle)
    private float moveSpeed = 5f;
    private float cameraAngle = 45f;
    private float roomBottomLimit = -19.5f;
    private bool hacked;
    private float maxHackDuration = 10f;
    private bool aiming;

    // Start is called before the first frame update
    void Start()
    {
        viewCamera = Camera.main;
        camTransform = viewCamera.transform;
        fow = GetComponent<FieldOfView>();
        characterController = GetComponent<CharacterController>();
        controller = characterController;
        transformTarget = transform;
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        playerController.SetArsenal("AK-74M");
    }

    // Update is called once per frame
    void Update()
    {
        // Toggles aim stance
        if (Input.GetButtonDown("Fire2"))
        {
            if (aiming)
            {
                animator.SetBool("Aiming", false);
                aiming = false;
            } else
            {
                animator.SetFloat("Speed", 0f);
                animator.SetBool("Aiming", true);
                aiming = true;
            }
        }

        if (!aiming)
        {
            Movement();
            animator.SetFloat("Speed", characterController.velocity.magnitude);
        } else
        {
            Look();
            // if weapon is AK, alert all guards
            if (Input.GetButtonDown("Fire1")) {
                animator.SetTrigger("Attack");
            }
        }
        ThirdPersonCamera();
        fow.FindTarget();
        Hack();        
    }

    private float turnSpeed = 480f;
    private void Movement()
    {
        leftRightInput = Input.GetAxisRaw("Horizontal");
        forwardBackwardInput = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(leftRightInput, 0, forwardBackwardInput).normalized;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        if (moveDirection.magnitude > 0)
        {
            Quaternion look = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                look,
                turnSpeed * Time.deltaTime
            );
        }
        
        //transform.forward = new Vector3(leftRightInput, 0, forwardBackwardInput).normalized;
    }

    private void Look()
    {
        Ray mousePos = viewCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mousePos, out RaycastHit hit, lookPlane);
        Vector3 lookAt = new Vector3(hit.point.x, transformTarget.position.y, hit.point.z);
        //Debug.DrawLine(transform.position, lookAt, Color.red);
        transformTarget.LookAt(lookAt);
    }

    private void ThirdPersonCamera()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(cameraAngle, 0, 0);
        Vector3 pos = transformTarget.position + rotation * dir;
        pos.z = Mathf.Max(pos.z, roomBottomLimit);
        camTransform.position = pos;
        camTransform.LookAt(transformTarget);
    }

    private void Hack()
    {
        bool hackHeld = Input.GetButton("Hack");
        bool hackDown = Input.GetButtonDown("Hack");
        //change to hold until circle meter full to hack, .5 second to hack and unhack
        bool targetFound = fow.FindIndirectTarget();
        //draw hack target reticle
        Debug.Log("hackDown: " + hackDown + ", hacked: " + hacked + ", targetFound: " + targetFound);
        if (hackDown && !hacked)
        {
            if (targetFound)
            {
                hackedTarget = fow.hackTarget;
                transformTarget = hackedTarget;
                hackedTarget.SendMessage("Hacked");

                //could make this one line
                hackController = hackedTarget.GetComponent<CharacterController>();
                controller = hackController; 

                hacked = true;
                StartCoroutine(HackedCoroutine());
            }
        } else if (hackDown && hacked)
        {
            EndHack();
        }
        
        
    }

    private void EndHack()
    {
        controller = characterController;
        hacked = false;
        hackedTarget.SendMessage("Killed"); //Change to endhack later
        transformTarget = transform;
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
