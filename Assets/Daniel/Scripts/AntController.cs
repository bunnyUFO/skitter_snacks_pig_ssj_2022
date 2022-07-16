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
    public class AntController : MonoBehaviour
    {
        [Header("Ant Movement")]
        [SerializeField] 
        private float moveSpeed = 2.0f;
        [SerializeField] 
        private float rotationSpeed = 90f;

        private Vector3 _inputDirection;
        private Rigidbody _rigidbody;
        private StarterAssetsInputs _input;
        
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _input = GetComponent<StarterAssetsInputs>();
        }

        private void Update()
        {
            Rotate(Time.deltaTime);
            Move();
        }

        private void Move()
        {
            _inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            print(_inputDirection);
            Vector3 targetDirection = transform.forward * _inputDirection.z + transform.right * _inputDirection.x;
            _rigidbody.velocity = (targetDirection.normalized) * moveSpeed;
        }

        private void Rotate(float deltaTime)
        {
            Vector3 up = transform.up;
            ;
            if( Keyboard.current.qKey.isPressed || Mouse.current.leftButton.isPressed)
            {
                transform.rotation = Quaternion.AngleAxis(-rotationSpeed * deltaTime, up) * transform.rotation;
            }
            if (Keyboard.current.eKey.isPressed || Mouse.current.rightButton.isPressed)
            {
                transform.rotation = Quaternion.AngleAxis(rotationSpeed * deltaTime, up) * transform.rotation;
            }
        }
    }
}