using System;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerInput))]
    public class SpiderController : MonoBehaviour
    {
        [Header("Spider Movement")]
        [SerializeField] 
        private float moveSpeed = 2.0f;
        [SerializeField] 
        private float rotationSpeed = 90f;
        [SerializeField] 
        private float jumpViewRotationSpeed = 25f;
        
        [SerializeField]
        [Header("Spider Jump")]
        private float maxJumpPower =10f;
        [SerializeField] 
        private float jumpChargeTime =6f;
        [SerializeField] 
        private Projection projection;

        [Header("Camera")]
        [SerializeField] 
        private CinemachineFreeLook cinemachineFreeLook;
        [SerializeField] 
        private float jumpCameraOffsetX = 15f;

        public float jumpCharge;
        private Spider _spider;
        private Vector3 _previousInputDirection, _inputDirection, _jumpVelocity;
        private Rigidbody _rigidbody;
        private StarterAssetsInputs _input;
        private float _jumpChargeTimeDelta;

        private void Awake()
        {
            _spider = transform.GetComponent<Spider>();
            _previousInputDirection =_inputDirection = Vector3.zero;
            _jumpChargeTimeDelta = 0f;
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _input = GetComponent<StarterAssetsInputs>();
        }

        private void Update()
        {
            StaggerLegs();
            Jump(Time.deltaTime);
            if (!_rigidbody.useGravity)
            {
                Rotate(Time.deltaTime);
                Move();;
            }
        }

        
        
        private void Jump(float deltaTime)
        {
            if (!_rigidbody.useGravity)
            {
                projection.EnableProjection(false);
            }
            
            if (Keyboard.current.spaceKey.isPressed)
            {
                if (jumpCharge == 0f)
                {
                    cinemachineFreeLook.m_YAxis.Value = 0.6f;
                    cinemachineFreeLook.m_XAxis.Value = _spider.transform.rotation.eulerAngles.y + jumpCameraOffsetX;
                }

                projection.EnableProjection(true);
                _spider.ChargingJump(true);
                _jumpChargeTimeDelta += deltaTime;
                jumpCharge = Math.Min(1, _jumpChargeTimeDelta/ jumpChargeTime);

                float jumpPower = maxJumpPower * jumpCharge;
                _jumpVelocity = (_spider.transform.up + _spider.transform.forward).normalized * jumpPower;
                projection.SimulateTrajectory(_spider, 
                    _spider.transform.position, 
                    _jumpVelocity);
            }

            if (Keyboard.current.spaceKey.wasReleasedThisFrame)
            {
                _jumpChargeTimeDelta = 0f;
                _spider.Jump(_jumpVelocity);
                jumpCharge = 0f;
            }
        }

        private void Move()
        {

            if (Keyboard.current.spaceKey.isPressed)
            {
                _rigidbody.velocity = Vector3.zero;
                return;
            }
            
            _inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            

            Vector3 targetDirection = transform.forward * _inputDirection.z + transform.right * _inputDirection.x;

            _rigidbody.velocity = (targetDirection.normalized) * moveSpeed;
        }

        private void  StaggerLegs()
        {
            if (stoppedMoving() || StoppedRotating())
            {
                _spider.StaggerLegs(true);
            }
            
            _previousInputDirection = _inputDirection;
        }

        private bool stoppedMoving()
        {
            if (_previousInputDirection.magnitude != 0 && _inputDirection.magnitude == 0)
            {
                return true;
            }

            return false;
        }

        private void Rotate(float deltaTime)
        {

            float rotateSpeed = Keyboard.current.spaceKey.isPressed ? jumpViewRotationSpeed : rotationSpeed;
            if( Keyboard.current.qKey.isPressed || Mouse.current.leftButton.isPressed){
                transform.Rotate(0f, -rotateSpeed * deltaTime, 0f);
            }
            if (Keyboard.current.eKey.isPressed || Mouse.current.rightButton.isPressed)
            {
                transform.Rotate(0f, rotateSpeed * deltaTime, 0f);
            }
        }

        private bool RotationKeysPressed()
        {
            return Keyboard.current.qKey.isPressed || Mouse.current.leftButton.isPressed;
        }

        private bool RotationKeysReleased()
        {
            return Keyboard.current.qKey.wasReleasedThisFrame || Mouse.current.leftButton.wasReleasedThisFrame ||
                   Keyboard.current.eKey.isPressed || Mouse.current.rightButton.isPressed;
        }

        private bool StoppedRotating()
        {
            if (_rigidbody.velocity.magnitude == 0 && RotationKeysReleased() && !RotationKeysPressed())
            {
                return true;
            }

            return false;
        }
    }
}