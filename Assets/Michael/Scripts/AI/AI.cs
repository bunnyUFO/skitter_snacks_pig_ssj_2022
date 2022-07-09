using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public List<GameObject> Patrol_Points = new List<GameObject>();
    public State _state;
    public Transform player;
    public Material green;
    public Material red;

    public Material closeRange;
    public Material mediumRange;
    public Material farRange;

    Spotbar spotbar;
    NavMeshAgent agent;
    Vector3 v_targetVector;
    AIFOV aifov;
    Phermone phermone;

    public float pointRange = 1.0f;
    public float timer = 5.0f;

    float reset;
    float range;
    int tPN = 0;

    void Start()
    {
        reset = timer;
        spotbar = GetComponentInChildren<Spotbar>();
        agent = GetComponent<NavMeshAgent>();
        aifov = GetComponent<AIFOV>();
        phermone = GetComponentInChildren<Phermone>();
        
        range = aifov.Range;

        if (_state == State.Patroling)
        {
            agent.destination = Patrol_Points[tPN].transform.position;
        }
    }


    void Update()
    {
        switch (_state)
        {
            case State.Patroling:

                spotbar.decreaseSpot();

                v_targetVector = Patrol_Points[tPN].transform.position - this.transform.position;

                if (v_targetVector.sqrMagnitude < pointRange)
                {
                    UpdateTarget();
                }
                
                break;

            case State.Chasing:

                v_targetVector = player.position - this.transform.position;
                agent.destination = player.position;

                if (v_targetVector.magnitude > range)
                {
                    GetComponent<MeshRenderer>().material = red;
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        _state = State.Returning;
                        timer = reset;
                    }
                }
                else
                {
                    GetComponent<MeshRenderer>().material = green;
                    timer = reset;
                }

                break;

            case State.Returning:

                spotbar.decreaseSpot();

                v_targetVector = Patrol_Points[tPN].transform.position - this.transform.position;
                agent.destination = Patrol_Points[tPN].transform.position;

                if (v_targetVector.magnitude < pointRange)
                {
                    UpdateTarget();
                    _state = State.Patroling;
                    spotbar.reveal(false);
                }

                break;

            case State.Spotting:

                spotbar.reveal(true);

                agent.isStopped = true;

                agent.destination = player.position;

                v_targetVector = player.position - this.transform.position;

                Vector3 turnTowardNavSteeringTarget = agent.steeringTarget;

                Vector3 direction = (turnTowardNavSteeringTarget - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);

                IncreaseSpottingValue();

                if (spotbar.spotted())
                {
                    _state = State.Chasing;
                    agent.isStopped = false;
                    phermone.ReleasePhermones();
                }
                
                if (v_targetVector.magnitude > range)
                {
                    _state = State.Returning;
                    agent.isStopped = false;
                    GetComponent<MeshRenderer>().material = red;
                }    

                break;

            case State.Stationary:
                break;
        }
    }

    public enum State
    {
        Patroling,
        Chasing,
        Returning,
        Spotting,
        Stationary
    }

    void UpdateTarget()
    {
        if (tPN >= Patrol_Points.Count - 1)
        {
            tPN = 0;
        }
        else
        {
            tPN++;
        }
        agent.destination = Patrol_Points[tPN].transform.position;
    }

    void IncreaseSpottingValue()
    {
        //Debug.Log("Distance is " + v_targetVector.magnitude);
        if (v_targetVector.magnitude <= range/4)
        {
            //Debug.Log("In short range");
            spotbar.increaseSpot(3);
            GetComponent<MeshRenderer>().material = closeRange;
        }
        else if (v_targetVector.magnitude <= range/2 && v_targetVector.magnitude > range/4)
        {
            //Debug.Log("In medium range");
            spotbar.increaseSpot(2);
            GetComponent<MeshRenderer>().material = mediumRange;
        }
        else if (v_targetVector.magnitude > range/2)
        {
            //Debug.Log("In far range");
            spotbar.increaseSpot(1);
            GetComponent<MeshRenderer>().material = farRange;
        }
    }

    public void spottingPlayer()
    {
        _state = State.Spotting;
    }

    public bool isChasing()
    {
        if (_state == State.Chasing)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
