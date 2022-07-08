using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    [SerializeField] LayerMask raycastLayer = default;
    [SerializeField] float offsetY = 0.2f;
    [SerializeField] private List<ProceduralAnimationScript> legs;
    private Rigidbody _rigidbody;
    private Vector3 _bodyTarget;

    private void Awake()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //update all leg positions
        foreach(ProceduralAnimationScript leg in legs)
        {
            leg.UpdatePosition(Time.deltaTime);
        }
        
        //force to move spider, will be set by controller later
        Vector3 moveForce = Vector3.forward *1f;
        
        //if body too far from average leg position add force to center it
        _bodyTarget = GetMeanLegPosition();
        Vector3 centerDistance = GetMeanLegPosition() - transform.position;
        if ((centerDistance).magnitude > 0.1f)
        {
            Vector3 legForce = (centerDistance).normalized * 5f;
            _rigidbody.AddForce(legForce);
        }
        else
        {
            _rigidbody.AddForce(Vector3.zero);
        }

        _rigidbody.velocity = moveForce;
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
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_bodyTarget, 0.1f);
    }
}