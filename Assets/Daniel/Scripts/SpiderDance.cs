using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpiderDance
    : MonoBehaviour
{
    [Header("Body Positioning")] [LabelOverride("Y Offset")] [SerializeField]
    float yOffset =0.2f;
    [SerializeField]
    float xOffset = 0.4f;
    [SerializeField]
    float frequency = 1;
    [SerializeField]
    AnimationCurve sensitivityCurve;

    [SerializeField] List<ProceduralAnimationScript> legs;
    [SerializeField] List<ProceduralAnimationScript> arms;
    private float _timeElapsed;
    private Vector3 _orinalPosition;

    void Start()
    {
        _orinalPosition = transform.position;
    }

    void Update()
    {
        _timeElapsed += Time.deltaTime;
        foreach (ProceduralAnimationScript leg in legs)
        {
            leg.UpdatePosition(Time.deltaTime);
        }
        Move();
    }

    private void Move()
    {
        float offsetX = sensitivityCurve.Evaluate(Mathf.Sin(frequency * _timeElapsed * Mathf.PI) * xOffset);
        float offsetY = sensitivityCurve.Evaluate(Mathf.Sin(frequency * 2 * _timeElapsed * Mathf.PI) * yOffset);
        transform.position = _orinalPosition + new Vector3(offsetX, offsetY, 0);
    }
    
}