using UnityEngine;
using UnityEngine.InputSystem;

namespace Turtinia.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _rotationSpeed = 10.0f;
        
        [Header("Movement")]
        [SerializeField] private float _movementSpeed = 5.0f;
        public bool IsMovementLocked { get; set; }
        public Vector3 MoveDirection { get; private set; }
        
        [Header("Jump")]
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _jumpHeight = 1.0f;
        [SerializeField] private float _shortJumpHeight = 0.4f;
        [SerializeField] private float _coyoteTime = 0.1f;
        [SerializeField] private float _jumpBufferTime = 0.1f;
        [SerializeField] private float _fallGravityMultiplier = 1.5f;
        
        [Tooltip("Apply small downward velocity to keep controller grounded.")]
        [SerializeField] private float _groundedStickVelocity = -2.0f;

        private CharacterController _characterController;
        private Vector2 _moveInput;
        private float _verticalVelocity;

        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private bool _jumpHeld;
        private bool _isJumping;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void OnValidate()
        {
            _shortJumpHeight = Mathf.Min(_shortJumpHeight, _jumpHeight);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _jumpHeld = true;
                _jumpBufferTimer = Mathf.Max(_jumpBufferTime, Time.deltaTime);
            }
            else if (context.canceled)
            {
                _jumpHeld = false;
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = forward * _moveInput.y + right * _moveInput.x;
            MoveDirection = moveDirection;
            
            if (!IsMovementLocked && moveDirection.sqrMagnitude > 0.001f)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, _rotationSpeed * dt);
            }

            UpdateVerticalVelocity(dt);

            Vector3 velocity = IsMovementLocked ? Vector3.zero : moveDirection * _movementSpeed;
            velocity.y = _verticalVelocity;
            _characterController.Move(velocity * dt);

            if ((_characterController.collisionFlags & CollisionFlags.Above) != 0 && _verticalVelocity > 0f)
            {
                _verticalVelocity = 0f;
            }
        }

        private void UpdateVerticalVelocity(float dt)
        {
            if (_characterController.isGrounded)
            {
                _coyoteTimer = _coyoteTime;
                _isJumping = false;

                if (_verticalVelocity < 0f)
                {
                    _verticalVelocity = _groundedStickVelocity;
                }
            }
            else
            {
                _coyoteTimer -= dt;
            }

            if (_jumpBufferTimer > 0f && _coyoteTimer > 0f)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                _isJumping = true;
                _jumpBufferTimer = 0f;
                _coyoteTimer = 0f;
            }
            
            if (_isJumping && !_jumpHeld)
            {
                float shortJumpVelocity = Mathf.Sqrt(_shortJumpHeight * -2f * _gravity);
                if (_verticalVelocity > shortJumpVelocity) _verticalVelocity = shortJumpVelocity;
            }

            float gravityScale = _verticalVelocity < 0f ? _fallGravityMultiplier : 1f;
            _verticalVelocity += _gravity * gravityScale * dt;

            _jumpBufferTimer -= dt;
        }
    }
}