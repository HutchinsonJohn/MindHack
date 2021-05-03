using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardScript : MonoBehaviour
{

    public AnimationCurve curve;
    public bool isAttachedToEnemy; //Keeps track of whether the keycard is attached to enemy, set to true in editor if enemy should spawn with keycard
    public Transform attachedToEnemy; //Set to enemy that should hold keycard, can be left empty otherwise
    private EnemyAI enemyAI;
    public bool isAttachedToPlayer;
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        enemyAI = attachedToEnemy.GetComponent<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        transform.Rotate(0, Time.deltaTime * 100f, 0);
        if (isAttachedToEnemy)
        {
            if (enemyAI.killed || enemyAI.slept || enemyAI.mindHacked)
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
        if (other.gameObject.tag == "Player")
        {
            isAttachedToPlayer = true;
        }
    }
}
