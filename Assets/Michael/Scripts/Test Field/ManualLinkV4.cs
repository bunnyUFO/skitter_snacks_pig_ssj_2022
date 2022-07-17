using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ManualLinkV4 : MonoBehaviour
{
    NavMeshAgent _agent;
    Rigidbody _rigidBody;
    Vector3 direction;
    Vector3 linkEndPoint;
    Vector3 linkStartPoint;
    Vector3 linkMidRotationPoint;
    AI ai;

    public float beginDistance = 2.0f;
    public float midDistance = 1.5f;
    public float endDistance = 1.0f;
    float distanceFromStart;
    float distanceFromMid;
    float distanceFromEnd;

    float distanceFromEndTemp;

    public float endpointThreshold = 0.1f;

    public bool traversing = false;

    public float transitionTimer = 1.0f;
    public float stopTimer = 1.0f;
    float resetStop;

    float transitionTimerReset;

    bool autoTransitioning = false;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rigidBody = GetComponent<Rigidbody>();
        ai = GetComponent<AI>();

        _agent.autoTraverseOffMeshLink = false;

        transitionTimerReset = transitionTimer;

        resetStop = stopTimer;
    }

    void Update()
    {
        if (!ai.isChasing())
        {
            transitionTimer -= Time.deltaTime;

            if (transitionTimer <= 0.0f)
            {
                _agent.autoTraverseOffMeshLink = false;
                autoTransitioning = false;
            }
        }

        if (ai.isChasing())
        {
            autoTransitioning = true;
            if (_agent.isOnOffMeshLink)
            {
                stopTimer -= Time.deltaTime;
            }
            if (stopTimer <= 0)
            {
                _agent.autoTraverseOffMeshLink = true;
                stopTimer = resetStop;
                Debug.Log("Transitioning");
            }
            if (!_agent.isOnOffMeshLink)
            {
                stopTimer = resetStop;
                _agent.autoTraverseOffMeshLink = false;
            }
            transitionTimer = transitionTimerReset;
        }

        if (_agent.nextOffMeshLinkData.startPos != new Vector3(0, 0, 0) && !traversing && !autoTransitioning)
        {

            linkStartPoint = _agent.nextOffMeshLinkData.startPos;
            linkEndPoint = _agent.nextOffMeshLinkData.endPos;

            distanceFromStart = (linkStartPoint - this.transform.position).magnitude;

            linkMidRotationPoint.x = linkStartPoint.x + (linkEndPoint.x - linkStartPoint.x) / 2;
            linkMidRotationPoint.y = linkStartPoint.y + (linkEndPoint.y - linkStartPoint.y) / 2;
            linkMidRotationPoint.z = linkStartPoint.z + (linkEndPoint.z - linkStartPoint.z) / 2;

            if (distanceFromStart < beginDistance && !ai.isChasing() && transitionTimer <= 0.0f)
            {
                Debug.Log("Am in distance of navmesh");
                //_agent.updatePosition = false;
                _agent.updateRotation = false;
                _agent.updateUpAxis = false;

                traversing = true;
            }
        }

        if (traversing)
        {
            //Debug.Log("Is Traversing");
            //Debug.Log("Distance from mid: " + distanceFromMid);
            //Debug.Log("Distance from end: " + distanceFromEnd);

            _rigidBody.velocity = this.transform.forward * 3.0f;

            direction = (linkMidRotationPoint - transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 12 * Time.deltaTime);

            distanceFromMid = (linkMidRotationPoint - this.transform.position).magnitude;
            distanceFromEnd = (linkEndPoint - this.transform.position).magnitude;
            //Debug.Log("Distance from Mid" + distanceFromMid);
            //Debug.Log("Distance from End" + distanceFromEnd);

            if (distanceFromMid < midDistance)
            {
                linkMidRotationPoint = linkEndPoint;
            }
            if (distanceFromEnd < endDistance)
            {
                //Debug.Log("Has Reached End");
                _rigidBody.velocity = this.transform.forward * 0;

                //_agent.updatePosition = true;
                _agent.updateRotation = true;
                _agent.updateUpAxis = true;

                _agent.CompleteOffMeshLink();

                traversing = false;
            }
        }



        /*
        // Obtain the triangular midpoint of a navlink
        if (_agent.nextOffMeshLinkData.startPos != new Vector3(0, 0, 0) && !traversing)
        {
            linkStartPoint = _agent.nextOffMeshLinkData.startPos;
            linkEndPoint = _agent.nextOffMeshLinkData.endPos;

            distanceFromStart = (linkStartPoint - this.transform.position).magnitude;

            linkMidRotationPoint.x = linkStartPoint.x + (linkEndPoint.x - linkStartPoint.x) / 2;
            linkMidRotationPoint.y = linkStartPoint.y + (linkEndPoint.y - linkStartPoint.y) / 2;
            linkMidRotationPoint.z = linkStartPoint.z + (linkEndPoint.z - linkStartPoint.z) / 2;

            Debug.Log("Distance from start: " + distanceFromStart);

            // Rotate the spider towards the correct rotation point if his distance from the start is close by
            if (distanceFromStart < 2f)
            {
                _agent.updatePosition = false;
                _agent.updateRotation = false;
                _agent.updateUpAxis = false;

                traversing = true;
            }

            // Going Down
            if (linkStartPoint.y > linkEndPoint.y)
            {
                // If also on X axis
                if (linkStartPoint.x == linkEndPoint.x)
                {
                    if (linkStartPoint.z < linkEndPoint.z) // Going Up on X Axis
                    {
                        //linkRotationPoint = new Vector3(linkStartPoint.x, linkEndPoint.y, linkStartPoint.z + 0.2f);
                        linkRotationPoint = new Vector3(linkStartPoint.x, linkEndPoint.y, linkStartPoint.z + 0.2f);
                    }
                    else // Going Down on X Axis
                    {
                        //linkRotationPoint = new Vector3(linkStartPoint.x, linkEndPoint.y, linkStartPoint.z - 0.2f);
                        linkRotationPoint = new Vector3(linkStartPoint.x, linkEndPoint.y, linkStartPoint.z - 0.2f);
                    }
                }

                // If also on Z axis
                if (linkStartPoint.z == linkEndPoint.z)
                {
                    if (linkStartPoint.x < linkEndPoint.x) // Going Up on Z Axis
                    {
                        //linkRotationPoint = new Vector3(linkStartPoint.x + 0.2f, linkEndPoint.y, linkStartPoint.z);
                        linkRotationPoint = new Vector3(linkStartPoint.x + 0.2f, linkEndPoint.y, linkStartPoint.z);
                    }
                    else // Going Down on Z Axis
                    {
                        //linkRotationPoint = new Vector3(linkStartPoint.x - 0.2f, linkEndPoint.y, linkStartPoint.z);
                        linkRotationPoint = new Vector3(linkStartPoint.x - 0.2f, linkEndPoint.y, linkStartPoint.z);
                    }
                }
            }

            // Going Up
            if (linkStartPoint.y < linkEndPoint.y)
            {
                // If also on X axis
                if (linkStartPoint.x == linkEndPoint.x)
                {
                    if (linkStartPoint.z < linkEndPoint.z) // Going Up on X Axis
                    {
                        //linkRotationPoint = new Vector3(linkStartPoint.x, linkEndPoint.y + 0.2f, linkStartPoint.z);
                        linkRotationPoint = new Vector3(linkStartPoint.x, linkStartPoint.y + 0.2f, linkEndPoint.z);
                    }
                    else // Going Down on X Axis
                    {
                        //linkRotationPoint = new Vector3(linkStartPoint.x, linkEndPoint.y + 0.2f, linkStartPoint.z);
                        linkRotationPoint = new Vector3(linkStartPoint.x, linkStartPoint.y + 0.2f, linkEndPoint.z);
                    }
                }

                // If also on Z axis
                if (linkStartPoint.z == linkEndPoint.z)
                {
                    if (linkStartPoint.x < linkEndPoint.x) // Going Up on Z Axis
                    {
                        //linkRotationPoint = new Vector3(linkStartPoint.x, linkEndPoint.y + 0.2f, linkStartPoint.z);
                        linkRotationPoint = new Vector3(linkEndPoint.x, linkStartPoint.y + 0.2f, linkEndPoint.z);
                    }
                    else // Going Down on Z Axis
                    {
                        //linkRotationPoint = new Vector3(linkStartPoint.x, linkEndPoint.y + 0.2f, linkStartPoint.z);
                        linkRotationPoint = new Vector3(linkEndPoint.x, linkStartPoint.y + 0.2f, linkEndPoint.z);
                    }
                }
            }
        }

        if (traversing)
        {
            Debug.Log("Is Traversing");

            _rigidBody.velocity = this.transform.forward * 3.0f;

            direction = (linkRotationPoint - transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 8 * Time.deltaTime);

            distanceFromMid = (linkRotationPoint - this.transform.position).magnitude;
            distanceFromEnd = (linkEndPoint - this.transform.position).magnitude;
            Debug.Log("Distance from Mid" + distanceFromMid);
            Debug.Log("Distance from End" + distanceFromEnd);

            if (distanceFromMid < 1.0f)
            {
                Debug.Log("Has Reached Mid");
                linkRotationPoint = linkEndPoint;
            }
            if (distanceFromEnd < 0.75f)
            {
                Debug.Log("Has Reached End");
                _rigidBody.velocity = this.transform.forward * 0;

                _agent.updatePosition = true;
                _agent.updateRotation = true;
                _agent.updateUpAxis = true;

                _agent.CompleteOffMeshLink();

                traversing = false;
            }
        }
        */
    }
        

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(new Vector3(_agent.currentOffMeshLinkData.end))
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(linkEndPoint, 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(linkStartPoint, 0.2f);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(linkRotationPoint, 0.2f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(linkMidRotationPoint, 0.2f);
    }
}
