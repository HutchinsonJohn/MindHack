using UnityEngine;

/// <summary>
/// Used to properly place the hack circle on the enemys head on the first frame it appears, otherwise it can be seen at another location for one frame
/// </summary>
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
