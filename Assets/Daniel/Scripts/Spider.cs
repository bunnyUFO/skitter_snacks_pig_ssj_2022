using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    [Header("Body Positioning")] [LabelOverride("Body Y Offset")] [SerializeField]
    float offsetY = 0.2f;

    [LabelOverride("Rotation Speed Curve")] [SerializeField]
    AnimationCurve sensitivityCurve;

    [SerializeField]
    List<ProceduralAnimationScript> legs;
    private bool _grounded = false;

    void Update()
    {
        foreach (ProceduralAnimationScript leg in legs)
        {
            leg.UpdatePosition(Time.deltaTime);
        }

        CalculateOrientation();
    }

    private void CalculateOrientation()
    {
        Vector3 up = Vector3.zero;
        float avgSurfaceDist = 0;

        _grounded = false;

        Vector3 point, a, b, c;

        for (int i = 0; i < legs.Count; i++)
        {
            // Calculate adjacent leg distance from body
            ProceduralAnimationScript legPair = i == 0 ? legs[legs.Count - 1] : legs[i - 1];
            point = legs[i].GetOldPosition();
            avgSurfaceDist += transform.InverseTransformPoint(point).y;
            a = (transform.position - point).normalized;
            b = (legPair.GetOldPosition() - point).normalized;
            
            // Calculate product of adjacent leg distance from body
            c = Vector3.Cross(a, b);
            
            // Use lge surface normal vectors and leg distance cross products to calculate body up vector
            up += c * sensitivityCurve.Evaluate(c.magnitude) +
                  (legs[i].stepNormal == Vector3.zero ? transform.forward : legs[i].stepNormal);
            _grounded |= legs[i].IsGrounded();

            Debug.DrawRay(point, c, Color.yellow, 0);
            Debug.DrawRay(point, legs[i].stepNormal, Color.magenta, 0);
        }

        // Scale up vector and surface vertical distance
        up /= legs.Count;
        avgSurfaceDist /= legs.Count;
        Debug.DrawRay(transform.position, up, Color.green, 0);

        // Rotate body to match up vector
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, up), up), 22.5f * Time.deltaTime);
        
        // I grounded move body to Y offset
        if (_grounded)
        {
            transform.Translate(0, -(-avgSurfaceDist + -offsetY) * 0.5f, 0, Space.Self);
        }
    }
}