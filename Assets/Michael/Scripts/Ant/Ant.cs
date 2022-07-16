using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ant : MonoBehaviour
{
    [Header("Body Positioning")] [LabelOverride("Body Y Offset")] [SerializeField]
    float offsetY = 0.1f;
    float offsetToleranceY = 0.02f;
    float offsetDurationY = 0.2f;

    [LabelOverride("Rotation Speed Curve")] [SerializeField]
    AnimationCurve sensitivityCurve;

    [SerializeField] List<AntLeg> legs;
    [SerializeField] bool debug = false;

    void Update()
    {
        foreach (AntLeg leg in legs)
        {
            leg.UpdatePosition(Time.deltaTime);
        }
        Move();
    }
    
    private void Move() {
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
            AntLeg legPair = i == 0 ? legs[legs.Count - 1] : legs[i - 1];
            oldPoint = legs[i].GetOldPosition();
            targetPoint = legs[i].GetTargetPosition();
            avgSurfaceDist += transform.InverseTransformPoint(targetPoint).y;
            a = (transform.position - oldPoint).normalized;
            b = (legPair.GetOldPosition() - oldPoint).normalized;
            c = Vector3.Cross(a, b);
            
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
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, up), up), 22.5f * Time.deltaTime);
        float yTranslateDistance = -(-avgSurfaceDist + -offsetY) * 0.5f;
        if (Math.Abs(yTranslateDistance) > offsetToleranceY)
        {
            float yScaleFactor = Time.deltaTime / offsetToleranceY;
            transform.Translate(0, -(-avgSurfaceDist + -offsetY) * 0.5f * yScaleFactor, 0, Space.Self);
        }
    }
}