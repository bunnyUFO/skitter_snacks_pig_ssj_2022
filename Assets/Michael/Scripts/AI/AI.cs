using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public List<GameObject> Patrol_Points = new List<GameObject>();
    public State state;
    public Transform player;

    Spotbar spotbar;
    NavMeshAgent agent;
    Vector3 v_targetVector;
    public Vector3 v_phermoneVector;
    AIFOV aifov;
    Phermone phermone;
    AudioManager audioManager;
    AudioSource audioSource;
    Manager manager;

    public float pointRange = 1.0f;
    public float timer = 5.0f;
    public float phermoneTimer = 3.0f;

    public float reset;
    public float phermoneTimerReset;
    public float range;
    int tPN = 0;

    public bool debug = false;

    public bool playingSong = false;

    void Start()
    {
        reset = timer;
        phermoneTimerReset = phermoneTimer;

        spotbar = GetComponentInChildren<Spotbar>();
        agent = GetComponent<NavMeshAgent>();
        aifov = GetComponent<AIFOV>();
        phermone = GetComponentInChildren<Phermone>();
        audioSource = GetComponent<AudioSource>();

        audioManager = GameObject.Find("SoundManager").GetComponent<AudioManager>();
        manager = GameObject.Find("EnemyManager").GetComponent<Manager>();
        
        range = aifov.Range;

        if (state == State.Patroling)
        {
            agent.destination = Patrol_Points[tPN].transform.position;
        }

        
    }


    void Update()
    {
        // Testing currently played song/sound effect works
        /*
        if (audioManager.currentlyPlayingSong("Idle"))
        {
            playingSong = true;
        }
        else
        {
            playingSong = false;
        }
        */

        switch (state)
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
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        state = State.Returning;
                        timer = reset;

                        manager.changeMusic();
                    }
                }
                else if (!aifov.canSee())
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        state = State.Returning;
                        timer = reset;

                        manager.changeMusic();
                    }
                }
                else
                {
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
                    state = State.Patroling;
                    //spotbar.reveal(false);
                }

                break;

            case State.Spotting:

                manager.changeMusic();

                //spotbar.reveal(true);

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
                    state = State.Chasing;
                    agent.isStopped = false;

                    if (manager.checkPhermones())
                    {
                        phermone.ReleasePhermones();
                    }

                    manager.changeMusic();
                }
                
                if (v_targetVector.magnitude > range || aifov.RaycastHit())
                {
                    state = State.Returning;
                    agent.isStopped = false;

                    manager.changeMusic();
                }

                break;

            case State.Phermone:

                v_targetVector = v_phermoneVector - this.transform.position;

                if (v_targetVector.sqrMagnitude < pointRange)
                {
                    phermoneTimer -= Time.deltaTime;
                    if (phermoneTimer <= 0)
                    {
                        state = State.Returning;
                        phermoneTimer = phermoneTimerReset;
                    }
                }

                spotbar.maxSpot();

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
        }
        else if (v_targetVector.magnitude <= range/2 && v_targetVector.magnitude > range/4)
        {
            spotbar.increaseSpot(2);
        }
        else if (v_targetVector.magnitude > range/2)
        {
            spotbar.increaseSpot(1);
        }
    }

    public void spottingPlayer()
    {
        state = State.Spotting;
    }

    public bool isChasing()
    {
        if (state == State.Chasing)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isSpotting()
    {
        if (state == State.Spotting)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isPhermoned()
    {
        if (state == State.Phermone)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Color colour = Color.red;

    private void OnDrawGizmos()
    {
        if (debug)
        {
            for (int i = 0; i < Patrol_Points.Count; i++)
            {
                Gizmos.color = colour;

                Gizmos.DrawSphere(Patrol_Points[i].transform.position, 0.2f);

                if (i > 0)
                {
                    Gizmos.DrawLine(Patrol_Points[i - 1].transform.position, Patrol_Points[i].transform.position);
                }
                if (i == 0)
                {
                    Gizmos.DrawLine(Patrol_Points[i].transform.position, Patrol_Points[Patrol_Points.Count - 1].transform.position);
                }
            }
        }
    }
}
