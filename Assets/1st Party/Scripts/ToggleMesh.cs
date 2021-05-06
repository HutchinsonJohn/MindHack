using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMesh : MonoBehaviour
{

    public GameObject box;
    private MeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = box.GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            mesh.enabled = false;
        }
    }
}
