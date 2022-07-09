using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    [LabelOverride( "body vertical offset" )]
    [SerializeField] float offsetY = 0.2f;
    [SerializeField] private List<ProceduralAnimationScript> legs;
    private Rigidbody _rigidbody;
    private Vector3 _bodyTarget,_moveVelocity;

    private void Awake()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
        _moveVelocity = Vector3.zero;
    }

    void Update()
    {
        //update all leg positions
        foreach(ProceduralAnimationScript leg in legs)
        {
            leg.UpdatePosition(Time.deltaTime);
        }

        _moveVelocity = Vector3.zero;
        _moveVelocity = Vector3.forward *1f;
        _bodyTarget = GetMeanLegPosition() + Vector3.up*offsetY;
        setBodyVerticalVelocity();
        

        _rigidbody.velocity = _moveVelocity;
    }
    
    private Vector3 GetMeanLegPosition(){
        if (legs.Count == 0)
            return Vector3.zero;
         
        float x = 0f;
        float y = 0f;
        float z = 0f;
 
        foreach (ProceduralAnimationScript leg in legs)
        {
            Vector3 pos = leg.GetOldPosition();
            x += pos.x;
            y += pos.y;
            z += pos.z;
        }
        return new Vector3(x / legs.Count, y / legs.Count, z / legs.Count);
    }

    private void setBodyVerticalVelocity()
    {
        //if body too far from average leg position add force to center it
        Vector3 centerDistance = GetMeanLegPosition() - transform.position;
        Vector3 legForce = (centerDistance).normalized * 5f;
        if ((centerDistance.y) is > 0.01f or < -0.01f)
        {
            _moveVelocity += Vector3.up * (legForce.y/5);
        }
        if ((centerDistance.x) is > 0.01f or < -0.01f)
        {
            _moveVelocity += Vector3.right * (legForce.x/5);
        }
        if ((centerDistance.z) is > 0.2f or < -0.2f)
        {
            _moveVelocity += Vector3.forward * (legForce.z/5);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_bodyTarget, 0.2f);
    }
}