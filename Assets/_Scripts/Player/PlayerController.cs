using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Turtinia.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private float _movementSpeed = 5.0f;
        [SerializeField] private float _jumpHeight = 2.0f;
        [SerializeField] private float _gravity = -9.8f;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private bool _shouldFaceMoveDirection = false;
        
        private CharacterController _characterController;
        private Vector2 _moveInput;
        private Vector3 _velocity;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && _characterController.isGrounded)
            {
                _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }
        }

        private void Update()
        {
            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;

            forward.y = 0;
            right.y = 0;
            
            forward.Normalize();
            right.Normalize();
            
            Vector3 moveDirection = forward * _moveInput.y + right * _moveInput.x;
            _characterController.Move(moveDirection * (_movementSpeed * Time.deltaTime));

            if (_shouldFaceMoveDirection && moveDirection.sqrMagnitude > 0.001f)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }
            
            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }
}
