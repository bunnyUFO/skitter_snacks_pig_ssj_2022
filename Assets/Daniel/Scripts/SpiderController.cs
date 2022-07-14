using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerInput))]
    public class SpiderController : MonoBehaviour
    {
        [Header("Player")] [Tooltip("Move speed of the character in m/s")]
        [SerializeField] float moveSpeed = 2.0f;
        [SerializeField] float maxJumpPower =400f;
        [SerializeField] float jumpChargeTime =3f;
        [SerializeField] Camera mainCamera, jumpCamera;
        [SerializeField] CinemachineFreeLook cineMachineFreeLook;

        public float jumpCharge;
        private Spider _spider;
        private Vector3 _previousInputDirection, _inputDirection;
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
            Jump(Time.deltaTime);
            Rotate(Time.deltaTime);
            Move();
            StaggerLegs();
        }

        private void Jump(float deltaTime)
        {
            if (_rigidbody.useGravity)
            {
                return;
            }

            if (Keyboard.current.spaceKey.isPressed)
            {
                _jumpChargeTimeDelta += deltaTime;
                jumpCharge = Math.Min(1, _jumpChargeTimeDelta/ jumpChargeTime);
                cineMachineFreeLook.enabled = false;
                jumpCamera.enabled = true;
                mainCamera.enabled = false;
            }

            if (Keyboard.current.spaceKey.wasReleasedThisFrame)
            {
                _jumpChargeTimeDelta = 0f;
                cineMachineFreeLook.enabled = true;
                cineMachineFreeLook.m_YAxis.Value = 0.8f;
                cineMachineFreeLook.m_XAxis.Value = _spider.transform.rotation.eulerAngles.y;
                jumpCamera.enabled = false;
                mainCamera.enabled = true;
                _rigidbody.useGravity = true;
                float jumpPower = maxJumpPower * jumpCharge;
                _rigidbody.AddForce((_spider.transform.up + _spider.transform.forward).normalized*jumpPower);
                jumpCharge = 0f;
            }
        }

        private void Move()
        {
            if (_rigidbody.useGravity)
            {
                return;
            }

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
            if( Keyboard.current.qKey.isPressed || Mouse.current.leftButton.isPressed){
                transform.Rotate(0f, -180f * deltaTime, 0f);
            }
            if (Keyboard.current.eKey.isPressed || Mouse.current.rightButton.isPressed)
            {
                transform.Rotate(0f, 180 * deltaTime, 0f);
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