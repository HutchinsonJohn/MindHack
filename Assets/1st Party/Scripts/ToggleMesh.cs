using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to make the Mindhacking device invisible when picked up
/// </summary>
public class ToggleMesh : MonoBehaviour
{

    public MeshRenderer mesh;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            mesh.enabled = false;
        }
    }
}
