using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;
    public LayerMask lookPlane;

    private Camera viewCamera;
    private Transform camTransform;
    private FieldOfView fow;

    // Variables
    private float leftRightInput, forwardBackwardInput;
    private float distance = 7f; //camera distance from player, more accurately its height times sin(cameraAngle)
    private float moveSpeed = 5f;
    private float cameraAngle = 45f;
    private float roomBottomLimit = -19.5f;

    // Start is called before the first frame update
    void Start()
    {
        viewCamera = Camera.main;
        camTransform = viewCamera.transform;
        fow = GetComponent<FieldOfView>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Look();
        ThirdPersonCamera();
        fow.FindTarget();
    }

    private void Movement()
    {
        leftRightInput = Input.GetAxisRaw("Horizontal");
        forwardBackwardInput = Input.GetAxisRaw("Vertical");
        controller.Move(new Vector3(leftRightInput, 0, forwardBackwardInput).normalized * moveSpeed * Time.deltaTime);
    }

    private void Look()
    {
        Ray mousePos = viewCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mousePos, out RaycastHit hit, lookPlane);
        Vector3 lookAt = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        //Debug.DrawLine(transform.position, lookAt, Color.red);
        transform.LookAt(lookAt);
    }

    private void ThirdPersonCamera()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(cameraAngle, 0, 0);
        Vector3 pos = transform.position + rotation * dir;
        pos.z = Mathf.Max(pos.z, roomBottomLimit);
        camTransform.position = pos;
        camTransform.LookAt(transform);
    }

}
