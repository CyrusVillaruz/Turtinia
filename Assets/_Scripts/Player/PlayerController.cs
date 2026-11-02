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
            Vector3 movement = new Vector3(_moveInput.x,  0, _moveInput.y);
            _characterController.Move(movement * (_movementSpeed * Time.deltaTime));
            
            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }
}
