using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{

    public Transform lookAt;
    public Transform camTransform;

    public Camera cam;

    private float distance = 7f;
    //private float height;
    private float currentX = 0f;
    private float currentY = 0f;
    private float sensitivityX = 4f;
    private float sensititvyY = 1f;

    // Start is called before the first frame update
    void Start()
    {
        camTransform = transform;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(45, 0, 0);
        Vector3 pos = lookAt.position + rotation * dir;
        pos.z = Mathf.Max(pos.z, -19.5f);
        camTransform.position = pos;
        camTransform.LookAt(lookAt.position);
    }
}
