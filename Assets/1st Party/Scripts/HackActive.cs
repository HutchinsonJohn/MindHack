using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackActive : MonoBehaviour
{

    // Start is called before the first frame update
    private void Start()
    {
        FaceCamera faceCamera = GetComponent<FaceCamera>();
        Transform head = faceCamera.head;
        Vector3 offset = faceCamera.offset;
        transform.position = head.position + offset;
        gameObject.SetActive(false);
    }

}
