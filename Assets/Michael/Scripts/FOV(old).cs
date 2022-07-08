using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour
{
    public Transform player;

    public float viewRadius;

    // Clamps angle to between 0 and 360
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public Material green;
    public Material red;

    float distanceToTarget;

    void Update()
    {
        if((player.transform.position - this.transform.position).normalized.sqrMagnitude < viewRadius)
        {
            findVisableTarget();
        }

        
    }

    void findVisableTarget()
    {
        // Array of colliders for all things overlapping its own collider sphere
        Collider[] targetsHit = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsHit.Length; i++)
        {
            Transform target = targetsHit[i].transform;
            Vector3 dirToTarget = (target.position - this.transform.position).normalized;
            if (Vector3.Angle(this.transform.forward, dirToTarget) < viewAngle / 2)
            {
                distanceToTarget = Vector3.Distance(this.transform.position, target.position);
            }

            // Does a raycast, but only to objects that are obstacles
            if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
            {
                GetComponent<MeshRenderer>().material = green;
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, char Axis, bool angleIsGlobal) 
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
