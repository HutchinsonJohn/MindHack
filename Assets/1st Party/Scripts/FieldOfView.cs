﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public float viewRadius;
    public float hackRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public Transform viewTarget;
    public Transform hackTarget;

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

    public bool FindIndirectTarget()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, hackRadius, targetMask);
        hackTarget = null;
        float lowestAngle = viewAngle;
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float angleBetweenTargetAndLook = Vector3.Angle(transform.forward, dirToTarget);
            if (angleBetweenTargetAndLook < viewAngle / 2 && angleBetweenTargetAndLook < lowestAngle)
            {
                hackTarget = target;
                lowestAngle = angleBetweenTargetAndLook;
            }
        }
        return (hackTarget != null);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

}
