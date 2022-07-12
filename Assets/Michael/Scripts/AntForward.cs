using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AntForward : MonoBehaviour
{
    // PROOF no issue when swapping between Rigidbody/Navmesh Agent
    Rigidbody _rigidBody;
    float timer = 3.0f;
    float reset;
    NavMeshAgent agent;
    bool RigidBodyInControl = true;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = this.GetComponent<Rigidbody>();
        agent = this.GetComponent<NavMeshAgent>();
        reset = timer;
        Debug.Log("Rigid Body Taking Over");
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0 && RigidBodyInControl)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            _rigidBody.velocity = (this.transform.forward * 3);
        }
        if (timer <=0 && RigidBodyInControl)
        {
            Debug.Log("Agent Taking Over");
            RigidBodyInControl = false;
            timer = reset;
        }


        if (timer > 0 && !RigidBodyInControl)
        {
            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.updateUpAxis = true;
            _rigidBody.velocity = (this.transform.forward * 0);
        }
        if (timer <= 0 && !RigidBodyInControl)
        {
            Debug.Log("Rigid Body Taking Over");
            RigidBodyInControl = true;
            timer = reset;
        }
    }
}
