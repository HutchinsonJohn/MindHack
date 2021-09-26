using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles a special door so that it may be opened by a specific enemy
/// </summary>
public class SpecialDoorTrigger : MonoBehaviour
{

    public EnemyAI enemyAI;

    public Transform doorTrigger;
    public Transform leftDoor;
    public Transform rightDoor;
    private float doorOpenDistanceSqr = 25f;
    private float openDistance;
    private Vector3 leftStart;
    private Vector3 rightStart;
    private Vector3 leftEnd;
    private Vector3 rightEnd;
    private Vector3 distanceToMove = new(0, 0, 2);

    // Start is called before the first frame update
    void Start()
    {
        leftStart = leftDoor.localPosition;
        rightStart = rightDoor.localPosition;
        leftEnd = leftStart + distanceToMove;
        rightEnd = rightStart - distanceToMove;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: If all enemies are defeated either open door, or if door is closed, keycard is not attached to player or enemy, and the player and keycard are on opposite sides of the door, remind the player they can restart
        // In general a more elegant solution should be implemented
        // Last condition exists so that the enemy may return to the room
        if (Vector3.SqrMagnitude(doorTrigger.localPosition - transform.localPosition) < doorOpenDistanceSqr || enemyAI.alertState > 0 || (!enemyAI.hacked && !enemyAI.killed && (enemyAI.agent.remainingDistance > enemyAI.agent.stoppingDistance || enemyAI.agent.hasPath)))
        {
            if (openDistance < 1f)
            {
                openDistance += Time.deltaTime;
                Mathf.Min(openDistance, 1f);
                leftDoor.localPosition = Vector3.Lerp(leftStart, leftEnd, openDistance);
                rightDoor.localPosition = Vector3.Lerp(rightStart, rightEnd, openDistance);
            }
        }
        else if (openDistance > 0f)
        {
            openDistance -= Time.deltaTime;
            Mathf.Max(openDistance, 0f);
            leftDoor.localPosition = Vector3.Lerp(leftStart, leftEnd, openDistance);
            rightDoor.localPosition = Vector3.Lerp(rightStart, rightEnd, openDistance);
        }
    }
}
