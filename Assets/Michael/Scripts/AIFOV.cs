using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFOV : MonoBehaviour
{
    public AI ai;
    public Transform player;
    public float FOV = 45;
    public float Range = 5;

    Vector3 targetDirection;
    float Angle;
    float Distance;
    bool Spotted = false;

    public Material green;
    public Material red;

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

        if (Mathf.Abs(Angle) < FOV / 2 && Distance < Range)
        {
            if (!Physics.Raycast(this.transform.position, targetDirection, Distance, layermask) && Spotted == false)
            {
                Debug.DrawRay(this.transform.position, targetDirection.normalized * Distance, Color.yellow);
                GetComponent<MeshRenderer>().material = green;
                if (Distance <= Range/2 && Distance > Range/4)
                {

                }
                else if (Distance <= Range/4)
                {

                }
                else
                {

                }
                // ai.FoundPlayer(Range);
                // The only thing the raycast WONT collide with is Character layer, so make layermask ABSOLUTELY EVERYTHING ELSE
            }
            else
            {
                GetComponent<MeshRenderer>().material = red;
            }
        }
        else
        {
            GetComponent<MeshRenderer>().material = red;
        }
    }
}
