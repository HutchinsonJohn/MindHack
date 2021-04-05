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
    private float distance = 7f;

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
        controller.Move(new Vector3(leftRightInput, 0, forwardBackwardInput).normalized * 5 * Time.deltaTime);
    }

    private void Look()
    {
        //Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
        //mousePos.y = 0;
        //Debug.DrawLine(transform.position, mousePos + Vector3.up * transform.position.y, Color.red);
        //transform.LookAt(mousePos + Vector3.up * transform.position.y);

        Ray mousePos = viewCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(mousePos, out hit, lookPlane);
        Vector3 lookAt = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        Debug.DrawLine(transform.position, lookAt, Color.red);
        transform.LookAt(lookAt);
    }

    private void ThirdPersonCamera()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(45, 0, 0);
        Vector3 pos = transform.position + rotation * dir;
        pos.z = Mathf.Max(pos.z, -19.5f);
        camTransform.position = pos;
        camTransform.LookAt(transform);
    }

}
