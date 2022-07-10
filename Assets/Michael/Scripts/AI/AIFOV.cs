using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFOV : MonoBehaviour
{
    AI ai;

    public Transform player;
    public float FOV = 45;
    public float Range = 5;

    Vector3 targetDirection;
    float Angle;
    float Distance;

    public LayerMask layermask;

    void Start()
    {
        ai = GetComponent<AI>();
    }

    void Update()
    {
        targetDirection = player.transform.position - this.transform.position;
        Distance = targetDirection.magnitude;

        Angle = Vector3.Angle(transform.forward, targetDirection);

        if (Mathf.Abs(Angle) < FOV / 2 && Distance < Range && !ai.isChasing())
        {
            if (!Physics.Raycast(this.transform.position, targetDirection, Distance, layermask))
            {
                Debug.DrawRay(this.transform.position, targetDirection.normalized * Distance, Color.yellow);

                ai.spottingPlayer();

                // The only thing the raycast WONT collide with is Character layer, so make layermask ABSOLUTELY EVERYTHING ELSE
            }
        }
    }

    public bool RaycastHit()
    {
        if (!Physics.Raycast(this.transform.position, targetDirection, Distance, layermask))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
