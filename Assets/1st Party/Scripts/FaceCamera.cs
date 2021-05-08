using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera mainCamera;
    public Transform head;
    public Vector3 offset;

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        transform.position = head.position + offset;
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }
}
