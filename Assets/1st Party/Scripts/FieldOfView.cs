using UnityEngine;

/// <summary>
/// Contains functions for finding targets in objects in field of view
/// </summary>
public class FieldOfView : MonoBehaviour
{

    public float viewRadius;
    public float hackRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public Transform viewTarget;
    [HideInInspector]
    public Transform hackTarget;

    /// <summary>
    /// Find target in fov
    /// </summary>
    /// <returns></returns>
    public bool FindTarget()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        viewTarget = null;
        float lowestAngle = viewAngle;
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float angleBetweenTargetAndLook = Vector3.Angle(transform.forward, dirToTarget);
            // TODO: Send alert if player is in fov regardless of lowest angle 
            if (angleBetweenTargetAndLook < viewAngle / 2 && angleBetweenTargetAndLook < lowestAngle)
            {
                float disToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, disToTarget, obstacleMask))
                {
                    viewTarget = target;
                    lowestAngle = angleBetweenTargetAndLook;
                }
            }
        }
        return (viewTarget != null);
    }

    /// <summary>
    /// Finds target in fov ignoring walls
    /// </summary>
    /// <param name="lookAt"></param>
    /// <returns></returns>
    public bool FindIndirectTarget(Vector3 lookAt)
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, hackRadius, targetMask);
        hackTarget = null;
        float lowestAngle = viewAngle;
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float angleBetweenTargetAndLook = Vector3.Angle(lookAt - transform.position, dirToTarget);
            if (angleBetweenTargetAndLook < viewAngle / 2 && angleBetweenTargetAndLook < lowestAngle)
            {
                hackTarget = target;
                lowestAngle = angleBetweenTargetAndLook;
            }
        }
        return (hackTarget != null);
    }

    /// <summary>
    /// Debug for displaying fov cone
    /// </summary>
    /// <param name="angleInDegrees"></param>
    /// <param name="angleIsGlobal"></param>
    /// <returns></returns>
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

}
