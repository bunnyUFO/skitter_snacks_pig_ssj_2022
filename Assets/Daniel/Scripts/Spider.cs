using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class Spider : MonoBehaviour
{
    [Header("Body Positioning")] [LabelOverride("Y Offset")] [SerializeField]
    float yOffset = 0.1f;

    [LabelOverride("Y Offset Tolerance")] [SerializeField]
    float yOffsetTolerance = 0.02f;

    [LabelOverride("Y Offset Duration")] [SerializeField]
    float yOffsetDuration = 0.5f;

    [LabelOverride("Rotation Speed Curve")] [SerializeField]
    AnimationCurve sensitivityCurve;

    [SerializeField] List<ProceduralAnimationScript> legs;
    [SerializeField] bool debug = false;
    private Rigidbody _rigidbody;
    private bool _stagger = false;
    private bool _chargingJump = false;
    private float _deltaTime, _timeInJump;

    private void Awake()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
    }

    void Update()
    {
        _deltaTime = Time.deltaTime;
        foreach (ProceduralAnimationScript leg in legs)
        {
            leg.UpdatePosition(Time.deltaTime);
        }

        StaggerLegs();
        Move();
    }

    private void Move()
    {
        if (legs.TrueForAll(leg => !leg.Grounded()))
        {
            _rigidbody.useGravity = true;
        }

        CalculateOrientation();
    }
    
    private void Die()
    {
        List<GameObject> respawns = GameObject.FindGameObjectsWithTag("Respawn").ToList();
        float closestDistance = float.MaxValue;
        bool foundRespawn = false;
        Vector3 closestSpawnPosition = transform.position;
        Quaternion closestSpawnRotation = transform.rotation;

        foreach (var respawn in respawns)
        {
            Vector3 respawnPostion = respawn.transform.position;
            float distance = (transform.position - respawnPostion).magnitude;
            
            if (distance < closestDistance)
            {
                foundRespawn = true;
                closestDistance = distance;
                closestSpawnPosition = respawnPostion;
                closestSpawnRotation = respawn.transform.rotation;

            }
        }

        if (foundRespawn)
        {
            transform.position = closestSpawnPosition;
            transform.rotation = closestSpawnRotation;
            
        }

        
        print("Oh no I'm dying!!");
    }

    private void CalculateOrientation()
    {
        if (!_rigidbody.useGravity)
        {
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

            
            if (!_chargingJump)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, up), up), 22.5f * _deltaTime);
                float yTranslateDistance = -(-avgSurfaceDist + -yOffset) * 0.5f;
                if (Math.Abs(yTranslateDistance) > yOffsetTolerance)
                {
                    float yScaleFactor = _deltaTime / yOffsetDuration;
                    transform.Translate(0, -(-avgSurfaceDist + -yOffset) * 0.5f * yScaleFactor, 0, Space.Self);
                }
            }
        }
        else
        {

            if (_rigidbody.velocity.magnitude > 0)
            {
                Vector3.ProjectOnPlane(_rigidbody.velocity, transform.right);
                Quaternion direction = Quaternion.LookRotation(_rigidbody.velocity, transform.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, direction, 20);
            }
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

    public void OnTriggerStay(Collider other)
    {
        _rigidbody.useGravity = false;
        StaggerLegs(true);
    }

    public void ChargingJump(bool charging)
    {
        _chargingJump = charging;
    }
    
    public void Jump(Vector3 velocity)
    {
        _rigidbody.useGravity = true;
        _rigidbody.AddForce(velocity, ForceMode.Impulse);
        _chargingJump = false;
    }
}