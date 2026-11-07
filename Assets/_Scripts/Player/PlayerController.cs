using UnityEngine;
using UnityEngine.InputSystem;

namespace Turtinia.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _movementSpeed = 5.0f;
        [SerializeField] private float _jumpHeight = 1.0f;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private bool _shouldFaceMoveDirection = false;
        [SerializeField] private float _rotationSpeed = 10.0f;
        
        private CharacterController _characterController;
        private Vector3 _moveInput;
        private Vector3 _moveVelocity;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            _moveVelocity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
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
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);
            }
            
            _moveVelocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_moveVelocity * Time.deltaTime);
        }
    }
}
