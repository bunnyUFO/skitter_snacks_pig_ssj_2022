using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public List<GameObject> Patrol_Points = new List<GameObject>();
    public State _state;
    public Transform player;

    NavMeshAgent agent;
    Vector3 v_targetVector;

    public float pointRange = 1.0f;
    public float timer;

    float reset;
    float range;
    int tPN = 0;

    void Start()
    {
        reset = timer;
        agent = GetComponent<NavMeshAgent>();
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

                v_targetVector = Patrol_Points[tPN].transform.position - this.transform.position;

                if (v_targetVector.sqrMagnitude < pointRange)
                {
                    UpdateTarget();
                }
                
                break;

            case State.Chasing:

                timer -= Time.deltaTime;

                v_targetVector = player.position - this.transform.position;

                agent.destination = player.transform.position;

                if (timer <= 0.0f && v_targetVector.magnitude > range)
                {
                    _state = State.Returning;
                    timer = reset;
                }

                break;

            case State.Returning:

                v_targetVector = Patrol_Points[tPN].transform.position - this.transform.position;
                agent.destination = Patrol_Points[tPN].transform.position;

                if (v_targetVector.magnitude < pointRange)
                {
                    UpdateTarget();
                    _state = State.Patroling;
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

    public void FoundPlayer(float spotting_distance)
    {
        _state = State.Chasing;

        timer = reset;
        range = spotting_distance;
    }
}
