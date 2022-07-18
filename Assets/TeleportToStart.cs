using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeleportToStart : MonoBehaviour
{
    NavMeshAgent agent;
    AI ai;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Manager manager;
    bool warping = true;

    // Start is called before the first frame update
    void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
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

    public void WarpToSpawn()
    {
        ai.tPN = 0;
        agent.enabled = true;
        agent.SetDestination(ai.Patrol_Points[0].transform.position);
        skinnedMeshRenderer.enabled = true;
        warping = false;
    }
}
