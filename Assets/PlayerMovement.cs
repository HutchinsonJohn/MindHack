using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;

    public Transform playerCam;

    // Variables
    private float leftRightInput, forwardBackwardInput;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        leftRightInput = Input.GetAxisRaw("Horizontal");
        forwardBackwardInput = Input.GetAxisRaw("Vertical");

        controller.Move(new Vector3(leftRightInput, 0, forwardBackwardInput).normalized * 5 * Time.deltaTime);
    }

}
