using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeleportToStart : MonoBehaviour
{
    Vector3 startPos;
    NavMeshAgent agent;
    AI ai;
    SkinnedMeshRenderer skinnedMeshRenderer;
    public string spawnPoint;
    Manager manager;
    bool warping = true;

    // Start is called before the first frame update
    void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        //startPos = this.transform.position;
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<AI>();
        manager = GameObject.Find("EnemyManager").GetComponent<Manager>();
        skinnedMeshRenderer.enabled = false;
    }

    private void Update()
    {
        if (manager.allowWarp && warping)
        {
            WarpToSpawn();
        }
    }

    // Update is called once per frame
    void WarpToPosition()
    {
        agent.Warp(startPos);
        Debug.Log("Warping");
        agent.SetDestination(ai.Patrol_Points[0].transform.position);
        skinnedMeshRenderer.enabled = true;
    }

    void WarpToPatrol()
    {
        ai.tPN = 0;
        agent.Warp(ai.Patrol_Points[0].transform.position);
        Debug.Log("Warping");
        skinnedMeshRenderer.enabled = true;
    }

    public void WarpToSpawn()
    {
        ai.tPN = 0;
        startPos = GameObject.Find(spawnPoint).transform.position;
        agent.enabled = true;
        agent.Warp(startPos);
        agent.SetDestination(ai.Patrol_Points[0].transform.position);
        skinnedMeshRenderer.enabled = true;
        warping = false;
    }
}
