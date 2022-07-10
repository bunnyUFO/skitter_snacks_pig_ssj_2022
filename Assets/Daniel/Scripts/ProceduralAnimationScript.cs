using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProceduralAnimationScript : MonoBehaviour
{
    [Header ("Step Configurations")]
    [SerializeField] float offsetZ = 0;
    [SerializeField] float stepDistance = 0.2f;
    [SerializeField] float stepHeight = 0.2f;
    [SerializeField] float stepDuration = 0.2f;
    [SerializeField] ProceduralAnimationScript oppositeLeg = null;
    
    [Header ("Shared Configurations")]
    [SerializeField] Transform body;
    [SerializeField] float offsetY = 1f;
    [SerializeField] LayerMask raycastLayer = default;
    
    [Header ("forward Raycast Configurations")]
    [SerializeField] float forwardRayCastDistance = 2f;
    [SerializeField] float forwardRayCastRadius = 2f;
    [SerializeField] bool debugBodyForward = false;

    [Header ("Leg Down Raycast Configurations")]
    [SerializeField] float downRayCastDistance = 1f;
    [SerializeField] float downRayCastRadius = 1f;
    [SerializeField] bool debugLegDown = false;
    
    [Header ("Body Down Raycast Configurations")]
    [SerializeField] float centerRayCastDistance = 2f;
    [SerializeField] float centerRayCastRadius = 2f;
    [SerializeField] bool debugBodyDown = false;

    public Vector3 stepNormal;
    private Transform rayCastSource;
    private Vector3 _oldPosition, _currentPosition, _targetPosition, _raycastPosition,_bodyPosition;
    private float lerp, lerpTime;

    //Set initial IK target position to ground at z offset from source
    private void Awake()
    {
        lerp = 1;
        lerpTime = stepDuration;
        _oldPosition = _currentPosition = _targetPosition = transform.position;
        
        rayCastSource = transform.parent.transform.Find("raycast_source").transform;
        Vector3 initalRayCastSource = transform.position + transform.up.normalized*offsetY;
        Ray ray = new Ray(initalRayCastSource, -transform.up.normalized);
        
        if (Physics.Raycast(ray, out RaycastHit info, downRayCastDistance, raycastLayer.value))
        {
            _oldPosition = _currentPosition =_targetPosition = info.point + transform.forward*offsetZ;
        }
    }
    
    public void UpdatePosition(float deltaTime)
    {
        transform.position = _currentPosition;
        _raycastPosition = rayCastSource.position;
        _bodyPosition = body.position;
        
        Vector3 legDownRaySource = _raycastPosition + transform.up * offsetY;
        
        Vector3 bodyDownBodySource = _raycastPosition;
        Vector3 bodyDownCastDirection = new Vector3(_bodyPosition.x, _bodyPosition.y, _raycastPosition.z) - _raycastPosition;

        Vector3 bodyForwardBodySource = _bodyPosition + transform.up * offsetY/2 + transform.forward*(_raycastPosition.z - _bodyPosition.z);
        /*
         * Ray cast and set target move position
         * Ray cast forward from body first
         * down from legs if no hit
         * down from body if no hit
        */
        if  (Physics.SphereCast(bodyForwardBodySource , forwardRayCastRadius, transform.forward.normalized, out RaycastHit forwardHit, forwardRayCastDistance, raycastLayer.value))
        {
            _targetPosition = forwardHit.point;
            stepNormal = forwardHit.normal;

            if (Vector3.Distance(_currentPosition, _targetPosition) > stepDistance && lerp >= 1 && oppositeLeg.IsGrounded())
            {
                lerpTime = 0f;
            }
        }
        else if (Physics.SphereCast(legDownRaySource , downRayCastRadius, -transform.up.normalized, out RaycastHit downHit, downRayCastDistance, raycastLayer.value)) {
            _targetPosition = downHit.point;
            stepNormal = downHit.normal;

            if (Vector3.Distance(_currentPosition, _targetPosition) > stepDistance && lerp >= 1 && oppositeLeg.IsGrounded())
            {
                lerpTime = 0f;
            }
        }
        else if  (Physics.SphereCast(bodyDownBodySource , centerRayCastRadius, bodyDownCastDirection.normalized, out RaycastHit centerHit, centerRayCastDistance, raycastLayer.value))
        {
            _targetPosition = centerHit.point;
            stepNormal = centerHit.normal;

            if (Vector3.Distance(_currentPosition, _targetPosition) > stepDistance && lerp >= 1 && oppositeLeg.IsGrounded())
            {
                lerpTime = 0f;
            }
        }

        lerp = lerpTime/stepDuration;
        
        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(_oldPosition, _targetPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            
            _currentPosition = tempPosition;
            lerpTime += deltaTime;
        }
        else
        {
            _oldPosition = _currentPosition;
        }
    }

    public Vector3 GetOldPosition()
    {
        return _oldPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_targetPosition, 0.02f);
        Gizmos.DrawLine(_oldPosition, _targetPosition);
        
        Vector3 castSource = transform.parent.transform.Find("raycast_source").transform.position;

        if (debugLegDown)
        {
            Vector3 source = castSource + transform.up * offsetY;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(source, downRayCastRadius);
            Gizmos.DrawLine(source, source + -transform.up.normalized*downRayCastDistance);
            Gizmos.DrawWireSphere(source + -transform.up.normalized*downRayCastDistance, downRayCastRadius);
        }

        if (debugBodyDown)
        {
            Vector3 source = castSource;
            Vector3 bodyDownCastDirection = new Vector3(body.position.x, body.position.y, source.z) - source;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(source, centerRayCastRadius);
            Gizmos.DrawLine(source, source + bodyDownCastDirection.normalized*centerRayCastDistance);
            Gizmos.DrawWireSphere(source + bodyDownCastDirection.normalized*centerRayCastDistance, centerRayCastRadius);
        }
        
        if (debugBodyForward)
        {
            Vector3 source = body.position + transform.up *(offsetY/2);
            Gizmos.DrawWireSphere(source, forwardRayCastRadius);
            Gizmos.DrawLine(source, source + transform.forward.normalized*forwardRayCastDistance);
            Gizmos.DrawWireSphere(source + transform.forward.normalized*forwardRayCastDistance, forwardRayCastRadius);
        }
    }
    public bool IsGrounded()
    {
        return lerp >= 1;
    }
}