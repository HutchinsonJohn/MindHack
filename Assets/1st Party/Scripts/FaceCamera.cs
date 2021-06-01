using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Orients hack circle, sleep text, and hacked text to face camera relative to given head transform
/// </summary>
public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;
    public Transform head;
    public Vector3 offset;

    // Start is called before the first frame update
    private void Start()
    {
        mainCamera = Camera.main;
    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        transform.position = head.position + offset;
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }
}
