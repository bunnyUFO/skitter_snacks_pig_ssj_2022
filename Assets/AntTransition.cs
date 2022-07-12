using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AntTransition : MonoBehaviour
{
    NavMeshAgent agent;
    Rigidbody _rigidBody;
    Vector3 properForward;

    public float timer = 3.0f;
    float reset;
    bool linking = false;
    bool repeat = true;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _rigidBody = GetComponent<Rigidbody>();
        reset = timer;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isOnOffMeshLink && !linking && repeat)
        {
            
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            
            linking = true;
            repeat = true;
            // Set Repeat to False to prevent repeat
            Debug.Log("Linking Begin");
        }


        if (linking)
        {
            timer -= Time.deltaTime;
            properForward = GameObject.Find("MoveTarget").transform.position;
            properForward = properForward - this.transform.position;
            // Write something here to update movement of Agent
            //agent.transform.position = Vector3.MoveTowards(transform.position, properForward, 2.0f);
            _rigidBody.velocity = properForward.normalized * 3.0f;

            if (timer <= 0.0f)
            {
                
                agent.updatePosition = true;
                agent.updateRotation = true;
                agent.updateUpAxis = true;

                _rigidBody.velocity = properForward.normalized * 0.0f;

                // Return the original target of the Agent

                timer = reset;
                linking = false;

                agent.CompleteOffMeshLink();
                Debug.Log("Linking End");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.forward);
    }
}
