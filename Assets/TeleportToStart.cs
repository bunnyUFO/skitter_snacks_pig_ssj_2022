using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeleportToStart : MonoBehaviour
{
    Vector3 startPos;
    float timer = 0.25f;
    NavMeshAgent agent;
    bool warping = true;
    AI ai;

    // Start is called before the first frame update
    void Start()
    {
        startPos = this.transform.position;
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<AI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (warping)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                WarpToPosition();
            }
        }
    }

    void WarpToPosition()
    {
        warping = false;
        agent.Warp(startPos);
        Debug.Log("Warping");
        agent.SetDestination(ai.Patrol_Points[0].transform.position);
    }
}
