using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spider : MonoBehaviour
{
    [Header("Body Positioning")] [LabelOverride("Body Y Offset")] [SerializeField]
    float offsetY = 0.2f;

    [LabelOverride("Rotation Speed Curve")] [SerializeField]
    AnimationCurve sensitivityCurve;

    [SerializeField] List<ProceduralAnimationScript> legs;
    [SerializeField] bool debug = false;
    private Rigidbody _rigidbody;
    private bool _stagger = false;

    private void Awake()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
    }

    void Update()
    {
        foreach (ProceduralAnimationScript leg in legs)
        {
            leg.UpdatePosition(Time.deltaTime);
        }
        StaggerLegs();
        Move();
    }
    
    private void Move() {
        if (legs.TrueForAll(leg => !leg.Grounded()))
        {
            _rigidbody.useGravity = true;
        }
        
        CalculateOrientation();
    }

    private void CalculateOrientation()
    {
        
        //will need to raycast to rotate towards closest surface
        //or predict point of impact and rotate when close

        Vector3 up = Vector3.zero;
        float avgSurfaceDist = 0;

        Vector3 a, b, c, oldPoint, targetPoint;

        for (int i = 0; i < legs.Count; i++)
        {
            // Calculate adjacent leg distance from body
            ProceduralAnimationScript legPair = i == 0 ? legs[legs.Count - 1] : legs[i - 1];
            oldPoint = legs[i].GetOldPosition();
            targetPoint = legs[i].GetTargetPosition();
            avgSurfaceDist += transform.InverseTransformPoint(targetPoint).y;
            a = (transform.position - oldPoint).normalized;
            b = (legPair.GetOldPosition() - oldPoint).normalized;

            // Calculate product of adjacent leg distance from body
            c = Vector3.Cross(a, b);

            // Use leg surface normal vectors and leg distance cross products to calculate body up vector
            // up += (legs[i].stepNormal == Vector3.zero ? transform.forward : legs[i].stepNormal);
            up += c * sensitivityCurve.Evaluate(c.magnitude) +
                  (legs[i].stepNormal == Vector3.zero ? transform.up : legs[i].stepNormal);

            if (debug)
            {
                Debug.DrawRay(oldPoint, c, Color.yellow, 0);
                Debug.DrawRay(oldPoint, legs[i].stepNormal, Color.magenta, 0);
            }
        }

        up /= legs.Count;
        avgSurfaceDist /= legs.Count;
        if (debug)
        {
            Debug.DrawRay(transform.position, up, Color.green, 0);
        }

        if (!_rigidbody.useGravity)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, up), up), 22.5f * Time.deltaTime);
            transform.Translate(0, -(-avgSurfaceDist + -offsetY) * 0.5f, 0, Space.Self);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up), Vector3.up), 22.5f * Time.deltaTime);
        }
    }

    public void StaggerLegs(bool stagger)
    {
        _stagger = stagger;
    }

    public void StaggerLegs()
    {
        if (_stagger && legs.TrueForAll(leg => !leg.IsMoving()))
        {
            _stagger = false;
            for (int i = 0; i < legs.Count; i++)
            {
                float delay = (((i / 4) + (i % 2)) * legs[i].stepDuration) / 2f;
                legs[i].Stagger(delay);
            }
        }
    }
    public void OnCollisionEnter(Collision other)
    {
        _rigidbody.useGravity = false;
        StaggerLegs(true);
    }
    
}