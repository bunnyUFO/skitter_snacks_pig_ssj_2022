using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(Rigidbody))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class SpiderController : MonoBehaviour
    {
        [Header("Player")] [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        private Spider _spider;
        private Vector3 _previousInputDirection, _inputDirection;


#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Rigidbody _rigidbody;
        private StarterAssetsInputs _input;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				    return false;
#endif
            }
        }

        private void Awake()
        {
            _spider = transform.GetComponent<Spider>();
            _previousInputDirection =_inputDirection = Vector3.zero;
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
			    Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
        }

        private void Update()
        {
            Move(Time.deltaTime);
        }

        private void Move(float time)
        {
            _inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (RotationKeysPressed())
            {
                transform.Rotate(0f, -360f * time, 0f);
            }

            if (Keyboard.current.eKey.isPressed || Mouse.current.rightButton.isPressed)
            {
                transform.Rotate(0f, 360 * time, 0f);
            }

            Vector3 targetDirection = transform.forward * _inputDirection.z + transform.right * _inputDirection.x;
            _rigidbody.velocity = (targetDirection.normalized) * MoveSpeed;

            if (stoppedMoving() || stoppedRotating())
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

        private bool RotationKeysPressed()
        {
            return Keyboard.current.qKey.isPressed || Mouse.current.leftButton.isPressed;
        }

        private bool RotationKeysReleased()
        {
            return Keyboard.current.qKey.wasReleasedThisFrame || Mouse.current.leftButton.wasReleasedThisFrame ||
                   Keyboard.current.eKey.isPressed || Mouse.current.rightButton.isPressed;
        }

        private bool stoppedRotating()
        {
            if (_rigidbody.velocity.magnitude == 0 && RotationKeysReleased() && !RotationKeysPressed())
            {
                return true;
            }

            return false;
        }
    }
}