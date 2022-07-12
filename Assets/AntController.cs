using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AntController : MonoBehaviour
{
    Rigidbody _rigidbody;
    Vector3 targetDirection;

    NavMeshAgent agent;

    // Need to turn off Navmesh Agent, then Disable kinematics, then Move Forward X amount of time/until at the end point, then Renable Kinematics and Navmesh Agent and end the navlink

    public float transitionSpeed = 3.0f;

    void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;
    }

    void Update()
    {
        
        //if (agent.isOnOffMeshLink)
        //{
            agent.updatePosition = false;
            agent.updateRotation = false;
            targetDirection = transform.forward;
            _rigidbody.velocity = targetDirection * transitionSpeed;

        // Get on Y axis to determine which direction it is going in, if omni directional will need to redo, then Complete Link
            if (this.transform.position == agent.currentOffMeshLinkData.endPos)
            {

            }
        //}
    }
}
