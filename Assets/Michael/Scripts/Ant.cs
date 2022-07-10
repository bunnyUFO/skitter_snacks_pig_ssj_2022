using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    [Header("Body Positioning")]
    [LabelOverride( "Body Y Offset" )]
    [SerializeField] float offsetY = 0.2f;
    [LabelOverride( "Rotation Curve" )]
    [SerializeField] AnimationCurve sensitivityCurve;
    [SerializeField] private List<ProceduralAnimationScript> legs;
    private Rigidbody _rigidbody;
    private bool _grounded = false;

    private void Awake()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // update all leg positions
        foreach(ProceduralAnimationScript leg in legs)
        {
            leg.UpdatePosition(Time.deltaTime);
        }

        CalculateOrientation();
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

    private void CalculateOrientation() {
        Vector3 up = Vector3.zero;
        float avgSurfaceDist = 0;

        _grounded = false;

        Vector3 point, a, b, c;

        // cross product of adjacent leg pairs to calculate average up
        for (int i = 0; i < legs.Count; i++)
        {
            ProceduralAnimationScript legPair = i == 0 ? legs[legs.Count - 1] : legs[i - 1];
            point = legs[i].GetOldPosition();
            avgSurfaceDist += transform.InverseTransformPoint(point).y;
            a = (transform.position - point).normalized;
            b = (legPair.GetOldPosition() - point).normalized;
            c = Vector3.Cross(a, b);
            up += c * sensitivityCurve.Evaluate(c.magnitude) + (legs[i].stepNormal == Vector3.zero ? transform.forward : legs[i].stepNormal);
            _grounded |= legs[i].IsGrounded();
            
            Debug.DrawRay(point, c, Color.yellow, 0);
            Debug.DrawRay(point, legs[i].stepNormal, Color.magenta, 0);
            
        }
        up /= legs.Count;
        avgSurfaceDist /= legs.Count;
        
        Debug.DrawRay(transform.position, up, Color.green, 0);

        // Asigns Up Using Vector3.ProjectOnPlane To Preserve Forward Orientation
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, up), up), 22.5f * Time.deltaTime);
        // Asigns Up Using Vector3.ProjectOnPlane To Preserve Forward Orientation
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, up), up), 22.5f * Time.deltaTime);
        if (_grounded) {
            transform.Translate(0, -(-avgSurfaceDist + -offsetY) * 0.5f, 0, Space.Self);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.color = Color.red;
    }
}