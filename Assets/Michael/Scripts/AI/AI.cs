using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public List<GameObject> Patrol_Points = new List<GameObject>();
    public State _state;
    public Transform player;
    public Transform rotationTarget;
    public Material green;
    public Material red;
    public Material blue;

    public Material closeRange;
    public Material mediumRange;
    public Material farRange;

    Spotbar spotbar;
    NavMeshAgent agent;
    Vector3 v_targetVector;
    Vector3 v_phermoneVector;
    AIFOV aifov;
    Phermone phermone;

    public float pointRange = 1.0f;
    public float timer = 5.0f;
    public float phermoneTimer = 3.0f;

    public float reset;
    public float phermoneTimerReset;
    public float range;
    int tPN = 0;

    void Start()
    {
        reset = timer;
        phermoneTimerReset = phermoneTimer;

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

                GetComponent<MeshRenderer>().material = red;

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
                // use rotationTarget.position to work for making a specific location that the enemy should look towards

                v_targetVector = player.position - this.transform.position;

                Vector3 turnTowardNavSteeringTarget = agent.steeringTarget;

                Vector3 direction = (turnTowardNavSteeringTarget - transform.position).normalized;
                // Change direction.y to 0 look appropriately on completely flat surface
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
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
                }

                break;

            case State.Phermone:

                v_targetVector = v_phermoneVector - this.transform.position;

                if (v_targetVector.sqrMagnitude < pointRange)
                {
                    phermoneTimer -= Time.deltaTime;
                    if (phermoneTimer <= 0)
                    {
                        _state = State.Returning;
                        phermoneTimer = phermoneTimerReset;
                    }
                }

                spotbar.maxSpot();
                spotbar.reveal(false);

                GetComponent<MeshRenderer>().material = blue;
                agent.destination = v_phermoneVector;

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
        Phermone,
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
        if (v_targetVector.magnitude <= range/4)
        {
            spotbar.increaseSpot(3);
            GetComponent<MeshRenderer>().material = closeRange;
        }
        else if (v_targetVector.magnitude <= range/2 && v_targetVector.magnitude > range/4)
        {
            spotbar.increaseSpot(2);
            GetComponent<MeshRenderer>().material = mediumRange;
        }
        else if (v_targetVector.magnitude > range/2)
        {
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

    private void OnTriggerStay(Collider touchedObject)
    {
        if (touchedObject.gameObject.CompareTag("Phermone") && _state != State.Chasing && _state != State.Returning)
        {
            v_phermoneVector = touchedObject.transform.position;
            _state = State.Phermone;
        }
    }
}
