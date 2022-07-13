using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ManualLinkv2 : MonoBehaviour
{
    NavMeshAgent agent;
    Rigidbody _rigidBody;
    float distance;

    bool saved = false;

    Vector3 direction;
    Vector3 rotationTarget;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.nextOffMeshLinkData.startPos != null)
        {
            distance = (agent.nextOffMeshLinkData.startPos - this.transform.position).magnitude;

            if (distance < 2.0f && saved == false)
            {
                Debug.Log("I'm in range of the navLink, my distance is: " + distance);
                //forwardVector = this.transform.forward;
                //distanceBetweenNavlinkPoints = agent.nextOffMeshLinkData.endPos - agent.nextOffMeshLinkData.startPos;

                rotationTarget = new Vector3(agent.nextOffMeshLinkData.endPos.x, this.transform.position.y, agent.nextOffMeshLinkData.endPos.z);
                direction = (rotationTarget - transform.position).normalized;
                // Change direction.y to 0 look appropriately on completely flat surface
                saved = true;
                agent.updateRotation = false;
            }
        }

        if (agent.isOnOffMeshLink || !agent.isOnNavMesh)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 15 * Time.deltaTime);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 360);
            Debug.Log("I'm on a NavMeshLink, or I'm not on a NavMesh");
            agent.updatePosition = false;
            agent.updateUpAxis = false;

            _rigidBody.velocity = this.transform.forward * 3.0f;
        }
    }
}
