using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Opens and closes door with respect to proximity of a given doorTrigger
/// </summary>
public class DoorTrigger : MonoBehaviour
{

    public Transform doorTrigger;
    public Transform leftDoor;
    public Transform rightDoor;
    public PlayerMovement player;
    private float doorOpenDistanceSqr = 25f;
    private float openDistance;
    private Vector3 leftStart;
    private Vector3 rightStart;
    private Vector3 leftEnd;
    private Vector3 rightEnd;
    private Vector3 distanceToMove = new Vector3(0, 0, 2);
    private bool alerted;

    // Start is called before the first frame update
    void Start()
    {
        leftStart = leftDoor.localPosition;
        rightStart = rightDoor.localPosition;
        leftEnd = leftStart + distanceToMove;
        rightEnd = rightStart - distanceToMove;
        alerted = false;
    }

    // Update is called once per frame
    void Update()
    {
        alerted = player.alerted;
        if (Vector3.SqrMagnitude(doorTrigger.localPosition - transform.localPosition) < doorOpenDistanceSqr && !alerted)
        {
            if (openDistance < 1f)
            {
                openDistance += Time.deltaTime;
                Mathf.Min(openDistance, 1f);
                leftDoor.localPosition = Vector3.Lerp(leftStart, leftEnd, openDistance);
                rightDoor.localPosition = Vector3.Lerp(rightStart, rightEnd, openDistance);
            }
        } else if (openDistance > 0f) {
            openDistance -= Time.deltaTime;
            Mathf.Max(openDistance, 0f);
            leftDoor.localPosition = Vector3.Lerp(leftStart, leftEnd, openDistance);
            rightDoor.localPosition = Vector3.Lerp(rightStart, rightEnd, openDistance);
        }
    }
}
