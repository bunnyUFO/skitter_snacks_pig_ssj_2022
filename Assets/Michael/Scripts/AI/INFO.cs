using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class INFO : MonoBehaviour
{
    NavMeshAgent agent;
    AI ai;
    AIFOV aifov;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<AI>();
        aifov = GetComponent<AIFOV>();
    }

    public void changeSpeed(float _speed, float _angularSpeed, float _acceleration)
    {
        agent.speed = _speed;
        agent.angularSpeed = _angularSpeed;
        agent.acceleration = _acceleration;
    }

    public void changeAI(float _pointRange, float _returnTimer, float _phermoneTimer)
    {
        ai.pointRange = _pointRange;
        ai.timer = _returnTimer;
        ai.reset = _returnTimer;
        ai.phermoneTimer = _phermoneTimer;
        ai.phermoneTimerReset = _phermoneTimer;
    }

    public void changeFOV(float _fov, float _range, LayerMask _layermask)
    {
        aifov.FOV = _fov;
        aifov.Range = _range;
        ai.range = _range;
        aifov.layermask = _layermask;
    }
}
