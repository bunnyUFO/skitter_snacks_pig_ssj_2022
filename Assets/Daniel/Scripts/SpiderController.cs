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
        public float MoveSpeed = 2.0f;

        private Spider _spider;
        private Vector3 _previousInputDirection, _inputDirection;
        private Rigidbody _rigidbody;
        private StarterAssetsInputs _input;

        private void Awake()
        {
            _spider = transform.GetComponent<Spider>();
            _previousInputDirection =_inputDirection = Vector3.zero;
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _input = GetComponent<StarterAssetsInputs>();
        }

        private void Update()
        {
            Move(Time.deltaTime);
        }

        private void Move(float time)
        {
            if (_rigidbody.useGravity)
            {
                return;
            }

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