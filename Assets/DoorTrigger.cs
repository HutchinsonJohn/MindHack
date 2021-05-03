using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{

    public Transform doorTrigger;
    public Transform leftDoor;
    public Transform rightDoor;
    private float doorOpenDistanceSqr = 25f;
    private float openDistance;
    private Vector3 leftStart;
    private Vector3 rightStart;
    private Vector3 leftEnd;
    private Vector3 rightEnd;
    private Vector3 distanceToMove = new Vector3(0, 0, 2);

    // Start is called before the first frame update
    void Start()
    {
        leftStart = leftDoor.position;
        rightStart = rightDoor.position;
        leftEnd = leftStart + distanceToMove;
        rightEnd = rightStart - distanceToMove;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.SqrMagnitude(doorTrigger.position - transform.position) < doorOpenDistanceSqr)
        {
            if (openDistance < 1f)
            {
                openDistance += Time.deltaTime;
                Mathf.Min(openDistance, 1f);
                leftDoor.position = Vector3.Lerp(leftStart, leftEnd, openDistance);
                rightDoor.position = Vector3.Lerp(rightStart, rightEnd, openDistance);
            }
        } else if (openDistance > 0f) {
            openDistance -= Time.deltaTime;
            Mathf.Max(openDistance, 0f);
            leftDoor.position = Vector3.Lerp(leftStart, leftEnd, openDistance);
            rightDoor.position = Vector3.Lerp(rightStart, rightEnd, openDistance);
        }
    }
}
