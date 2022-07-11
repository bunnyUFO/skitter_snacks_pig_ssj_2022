using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntController : MonoBehaviour
{
    Rigidbody _rigidbody;

    Vector3 targetDirection;

    public float MoveSpeed = 2.0f;

    void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
    }

    void Update()
    {
        targetDirection = transform.forward;
        _rigidbody.velocity = targetDirection * MoveSpeed;
    }
}
