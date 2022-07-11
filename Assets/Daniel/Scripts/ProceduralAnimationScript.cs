using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProceduralAnimationScript : MonoBehaviour
{
    [Header ("Step Configurations")]
    [SerializeField] bool stagger = false;
    [SerializeField] float stepDistance = 0.2f;
    [SerializeField] float stepHeight = 0.2f;
    [SerializeField] float stepDuration = 0.2f;
    [SerializeField] List<ProceduralAnimationScript> LegConstraints = null;
    
    [Header ("Shared Raycast Configurations")]
    [SerializeField] Transform body;
    [SerializeField] LayerMask raycastLayer = default;
    
    [Header ("forward Raycast Configurations")]
    [SerializeField] float forwardRayCastDistance = 2f;
    [SerializeField] float forwardRayCastRadius = 2f;
    [SerializeField] float forwardRayCastOffsetZ = 1f;
    [SerializeField] float forwardRayCastOffsetY = 1f;
    [SerializeField] bool debugBodyForward = false;

    [Header ("Leg Down Raycast Configurations")]
    [SerializeField] float downRayCastDistance = 1f;
    [SerializeField] float downRayCastRadius = 1f;
    [SerializeField] float downRayCastOffsetY = 1f;
    [SerializeField] bool debugLegDown = false;
    
    [Header ("Body Down Raycast Configurations")]
    [SerializeField] float centerRayCastDistance = 2f;
    [SerializeField] float centerRayCastRadius = 2f;
    [SerializeField] bool debugBodyDown = false;

    public Vector3 stepNormal;
    private Transform rayCastSource;
    private Vector3 _oldPosition, _currentPosition, _targetPosition, _raycastPosition,_bodyPosition;
    private float lerp, lerpTime;
    private bool _canMove, _previouslyGrounded, _grounded = false;

    //Set initial IK target position to ground at z offset from source
    private void Awake()
    {
        rayCastSource = transform.parent.transform.Find("raycast_source").transform;
        lerp = 1;
        lerpTime = stepDuration;
        _oldPosition = _currentPosition = _targetPosition = transform.position;
        _previouslyGrounded = _grounded;
        _raycastPosition = rayCastSource.position;
        _canMove = true;

        Vector3 legDownRaySource = _raycastPosition + transform.up * downRayCastOffsetY;
        if (stagger)
        {
            staggerLeg();
        }

        if (stagger && Physics.SphereCast(legDownRaySource , downRayCastRadius, -transform.up.normalized, out RaycastHit downHit, downRayCastDistance, raycastLayer.value)) {
            _oldPosition = _currentPosition =_targetPosition = downHit.point + transform.forward*stepDistance/2;
        }
    }
    
    public void UpdatePosition(float deltaTime)
    {
        transform.position = _currentPosition;
        _raycastPosition = rayCastSource.position;
        _bodyPosition = body.position;
        updateCanMove();
        
        Vector3 legDownRaySource = _raycastPosition + transform.up * downRayCastOffsetY;
        
        Vector3 bodyDownBodySource = _raycastPosition;
        Vector3 bodyDownCastDirection = (rayCastSource.right*(-rayCastSource.localPosition.x)).normalized;
        Vector3 bodyForwardBodySource = _bodyPosition + transform.up*forwardRayCastOffsetY + transform.forward*(rayCastSource.localPosition.z - forwardRayCastOffsetZ);
        
        _grounded = lerp < 1;
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
            _grounded = _currentPosition.y - forwardHit.point.y < forwardRayCastOffsetY;

            if (Vector3.Distance(_currentPosition, _targetPosition) > stepDistance && lerp >= 1 && !otherLegsMoving())
            {
                lerpTime = 0f;
            }
        }
        else if (Physics.SphereCast(legDownRaySource , downRayCastRadius, -transform.up.normalized, out RaycastHit downHit, downRayCastDistance, raycastLayer.value)) {
            _targetPosition = downHit.point;
            stepNormal = downHit.normal;
            _grounded = _currentPosition.y - downHit.point.y < downRayCastOffsetY;
            

            if (Vector3.Distance(_currentPosition, _targetPosition) > stepDistance && lerp >= 1 && !otherLegsMoving())
            {
                lerpTime = 0f;
            }
        }
        else if  (Physics.SphereCast(bodyDownBodySource , centerRayCastRadius, bodyDownCastDirection.normalized, out RaycastHit centerHit, centerRayCastDistance, raycastLayer.value))
        {
            _targetPosition = centerHit.point;
            stepNormal = centerHit.normal;
            _grounded = true;

            if (Vector3.Distance(_currentPosition, _targetPosition) > stepDistance && lerp >= 1 && !otherLegsMoving())
            {
                lerpTime = 0f;
            }
        }

        lerp = lerpTime/stepDuration;
        
        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(_oldPosition, _targetPosition, lerp);
            tempPosition += transform.up * Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            
            _currentPosition = tempPosition;
            lerpTime += deltaTime;
        }
        else
        {
            // if (_grounded)
            if (true)
            {
                _oldPosition = _currentPosition;
            }
            // else
            // {
            //     transform.position = _oldPosition = _currentPosition = rayCastSource.position + transform.forward * legStaggerOffset;
            // }
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
        
        Transform castTransform = transform.parent.transform.Find("raycast_source").transform;
        Vector3 castSource = castTransform.position;
        
        Vector3 castTransformRight = transform.parent.transform.Find("raycast_source").transform.right;

        if (debugLegDown)
        {
            Vector3 source = castSource + transform.up * downRayCastOffsetY;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(source, downRayCastRadius);
            Gizmos.DrawLine(source, source + -transform.up.normalized*downRayCastDistance);
            Gizmos.DrawWireSphere(source + -transform.up.normalized*downRayCastDistance, downRayCastRadius);
        }

        if (debugBodyDown)
        {
            Vector3 source = castSource;
            Vector3 bodyDownCastDirection = (castTransformRight*(-castTransform.localPosition.x)).normalized;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(source, centerRayCastRadius);
            Gizmos.DrawLine(source, source + bodyDownCastDirection.normalized*centerRayCastDistance);
            Gizmos.DrawWireSphere(source + bodyDownCastDirection.normalized*centerRayCastDistance, centerRayCastRadius);
        }
        
        if (debugBodyForward)
        {
            Vector3 source = body.position + transform.up*forwardRayCastOffsetY + transform.forward*(castTransform.localPosition.z - forwardRayCastOffsetZ);
            Gizmos.DrawWireSphere(source, forwardRayCastRadius);
            Gizmos.DrawLine(source, source + transform.forward.normalized*forwardRayCastDistance);
            Gizmos.DrawWireSphere(source + transform.forward.normalized*forwardRayCastDistance, forwardRayCastRadius);
        }
    }
    public bool IsGrounded()
    {
        return _grounded;
    }
    
    public bool IsMoving()
    {
        return lerp < 1;
    }

    private bool otherLegsMoving()
    {
        return LegConstraints.TrueForAll(leg => leg.IsMoving());
    }
    
    private void updateCanMove()
    {
        if (otherLegsMoving())
        {
            _canMove = false;
        }
        else
        {
            _canMove = true;
        }
    }


    public void staggerLeg()
    {
        if (!stagger) { return; }

        Vector3 legDownRaySource = _raycastPosition + transform.up * downRayCastOffsetY;
        
        if (Physics.SphereCast(legDownRaySource , downRayCastRadius, -transform.up.normalized, out RaycastHit downHit, downRayCastDistance, raycastLayer.value)) {
            _currentPosition = downHit.point + transform.forward*stepDistance/2;
        }
    }
}