using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keycard rotation and ownership
/// </summary>
public class KeycardScript : MonoBehaviour
{

    public AnimationCurve curve;
    /// <summary>
    /// Keeps track of whether the keycard is attached to enemy, set to true in editor if enemy should spawn with keycard
    /// </summary>
    public bool isAttachedToEnemy;
    /// <summary>
    /// Set to enemy that should hold keycard, can be left empty otherwise
    /// </summary>
    public Transform attachedToEnemy;
    private EnemyAI enemyAI;
    public bool isAttachedToPlayer;
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        if (isAttachedToEnemy)
        {
            enemyAI = attachedToEnemy.GetComponent<EnemyAI>();
        }
    }

    private void LateUpdate()
    {
        // TODO: Allow for hacked enemy to pick up keycard (also consider whether non hacked enemies should pick it up if they run over it)
        transform.Rotate(0, Time.deltaTime * 100f, 0);
        if (isAttachedToEnemy)
        {
            if (enemyAI.killed)
            {
                isAttachedToEnemy = false;
                transform.position = new Vector3(transform.position.x, curve.Evaluate(Time.time % curve.length) / 2 + 1.5f, transform.position.z);
            }
            transform.position = new Vector3(attachedToEnemy.position.x, curve.Evaluate(Time.time % curve.length) / 2 + 3.5f, attachedToEnemy.position.z);
        }
        else if (isAttachedToPlayer)
        {
            transform.position = new Vector3(player.position.x, curve.Evaluate(Time.time % curve.length) / 2 + 3.5f, player.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, curve.Evaluate(Time.time % curve.length) / 2 + 1.5f, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isAttachedToPlayer = true;
        }
    }
}
