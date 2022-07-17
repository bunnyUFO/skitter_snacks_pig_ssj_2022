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
        [SerializeField]
        private Spider spider;
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
        private float jumpChargeTime = 6f;        
        [SerializeField] 
        private float jumpAngle = 45f;
        [SerializeField] 
        private Projection projection;

        [Header("Camera Configurations")]
        [SerializeField] 
        private CinemachineFreeLook thirddPersonView;
        [SerializeField] 
        private CinemachineFreeLook jumpView;
        [SerializeField] 
        private Camera thirddPersonCamera;
        [SerializeField] 
        private Camera jumpCamera;
        [SerializeField] 
        private float jumpCameraOffsetX = 15f;

        private Spider _spider;
        private Transform _spiderTransform;
        public float jumpCharge;
        private Vector3 _oldUp, _oldForward;
        private Vector3 _previousInputDirection, _inputDirection, _jumpVelocity;
        private Rigidbody _rigidbody;
        private StarterAssetsInputs _input;
        private float _jumpChargeTimeDelta;

        private void Awake()
        {
            _previousInputDirection =_inputDirection = Vector3.zero;
            _jumpChargeTimeDelta = 0f;
        }

        private void Start()
        {
            _spiderTransform = spider.transform;
            _rigidbody = spider.GetComponent<Rigidbody>();
            _input = GetComponent<StarterAssetsInputs>();
        }

        private void Update()
        {
            if (!_rigidbody.useGravity)
            {
                Rotate(Time.deltaTime);
                Move();;
            }
            StaggerLegs();
            Jump(Time.deltaTime);
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
                    thirddPersonView.enabled = false;
                    thirddPersonCamera.enabled = false;
                    jumpCamera.enabled = true;
                    jumpView.enabled = true;
                    jumpView.m_YAxis.Value = 0.6f;
                    jumpView.m_XAxis.Value = spider.transform.rotation.eulerAngles.y + jumpCameraOffsetX;
                }

                projection.EnableProjection(true);
                spider.ChargingJump(true);
                _jumpChargeTimeDelta += deltaTime;
                jumpCharge = Math.Min(1, _jumpChargeTimeDelta/ jumpChargeTime);
                
                float jumpPower = maxJumpPower * jumpCharge;
                _jumpVelocity = (_spiderTransform.forward +  _spiderTransform.up).normalized * jumpPower;
                projection.SimulateTrajectory(spider, 
                    _spiderTransform.position, 
                    _jumpVelocity);
            }

            if (Keyboard.current.spaceKey.wasReleasedThisFrame)
            {
                thirddPersonView.enabled = true;
                thirddPersonCamera.enabled = true;
                jumpCamera.enabled = false;
                jumpView.enabled = false;
                thirddPersonView.m_YAxis.Value = 0f;
                thirddPersonView.m_XAxis.Value = 0f;
                _jumpChargeTimeDelta = 0f;
                spider.Jump(_jumpVelocity);
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
            

            Vector3 targetDirection = _spiderTransform.forward * _inputDirection.z + _spiderTransform.right * _inputDirection.x;

            _rigidbody.velocity = (targetDirection.normalized) * moveSpeed;
        }

        private void  StaggerLegs()
        {
            if (stoppedMoving() || StoppedRotating())
            {
                spider.StaggerLegs(true);
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
            //was causing bugs with camera
            /*
            if (Keyboard.current.spaceKey.isPressed && jumpCharge == 0f)
            {
                _spiderTransform.rotation = Quaternion.AngleAxis(-jumpAngle, _spiderTransform.right) * _spiderTransform.rotation;
            }
            */
            


            Vector3 up =_spiderTransform.up;
            
            float rotateSpeed = Keyboard.current.spaceKey.isPressed ? jumpViewRotationSpeed : rotationSpeed;
            if( Keyboard.current.qKey.isPressed || Mouse.current.leftButton.isPressed)
            {
                _spiderTransform.rotation = Quaternion.AngleAxis(-rotateSpeed * deltaTime, up) * _spiderTransform.rotation;
            }
            if (Keyboard.current.eKey.isPressed || Mouse.current.rightButton.isPressed)
            {
                _spiderTransform.rotation = Quaternion.AngleAxis(rotateSpeed * deltaTime, up) * _spiderTransform.rotation;
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