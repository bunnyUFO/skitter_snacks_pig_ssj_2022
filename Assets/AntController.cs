using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AntController : MonoBehaviour
{
    Rigidbody _rigidbody;
    Vector3 targetDirection;

    NavMeshAgent agent;

    bool transitioning = false;
    public float timer = 3.0f;
    float reset;
    float distance;
    public float linkDistance = 1.5f;

    // Need to turn off Navmesh Agent, then Disable kinematics, then Move Forward X amount of time/until at the end point, then Renable Kinematics and Navmesh Agent and end the navlink

    public float transitionSpeed = 1.0f;
    float transitionSpeedReset;

    void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;
        reset = timer;
        transitionSpeedReset = transitionSpeed;
    }

    void Update()
    {

        if (agent.isOnOffMeshLink && transitioning == false)
        {
            //Debug.Log("At navmesh link, coords are x:" + this.transform.position.x + " y:" + this.transform.position.y + " z:" + this.transform.position.z);
            //Debug.Log("Navmesh link position is x:" + agent.currentOffMeshLinkData.startPos.x + " y:" + agent.currentOffMeshLinkData.startPos.y + " z:" + agent.currentOffMeshLinkData.startPos.z);
            distance = (agent.currentOffMeshLinkData.startPos - this.transform.position).magnitude;
            //Debug.Log("The distance is: " + distance);

            if (distance < linkDistance)
            {
                debugRemoveControl();
                transitioning = true;
                Debug.Log("Agent losing control");
            }
        }
        
        if (transitioning)
        {
            enableWalk();
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                transitioning = false;

                debugGainControl();

                agent.CompleteOffMeshLink();

                Debug.Log("Agent taking Control");
              
                timer = reset;
                disableWalk();
            }
        }
    }

    void enableWalk()
    {
        transitionSpeed = transitionSpeedReset;
        targetDirection = transform.forward;
        _rigidbody.velocity = targetDirection * transitionSpeed;
        Debug.Log("Walking");
    }

    void disableWalk()
    {
        transitionSpeed = 0;
        targetDirection = transform.forward;
        _rigidbody.velocity = targetDirection * transitionSpeed;
    }

    void debugRemoveControl()
    {
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        agent.updatePosition = false;
        Debug.Log("Lost Control");
    }

    void debugGainControl()
    {
        agent.updateUpAxis = true;
        agent.updateRotation = true;
        agent.updatePosition = true;
        Debug.Log("Took Control");
    }
}
