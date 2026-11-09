using Turtinia.Combat;
using Turtinia.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Turtinia.Player
{
    public class PlayerSprint : MonoBehaviour
    {
        [SerializeField] private ResourceType _staminaType;
        [SerializeField] private float _speedMultiplier = 1.4f;
        [SerializeField] private float _staminaPerSecond = 10f;
        
        [Tooltip("Prevent weird flickering at low stamina.")]
        [SerializeField] private float _minStaminaToStart = 10f;

        private PlayerController _controller;
        private ResourceHub _hub;
        private IResource _stamina;

        public bool IsSprinting { get; private set; }

        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _hub = GetComponent<ResourceHub>();
        }

        private void Start()
        {
            _hub.TryGet(_staminaType, out _stamina);
        }

        public void OnToggleSprint(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (IsSprinting)
            {
                StopSprint();
            }
            else
            {
                StartSprint();
            }
        }

        private void Update()
        {
            if (!IsSprinting) return;

            bool moving = !_controller.IsMovementLocked && _controller.MoveDirection.sqrMagnitude > 0.001f;
            if (!moving) return;

            if (!_stamina.TryConsume(_staminaPerSecond * Time.deltaTime))
            {
                StopSprint();
            }
        }

        private void StartSprint()
        {
            if (_stamina == null || !_stamina.CanAfford(_minStaminaToStart)) return;
            IsSprinting = true;
            _controller.SpeedMultiplier = _speedMultiplier;
        }

        private void StopSprint()
        {
            IsSprinting = false;
            _controller.SpeedMultiplier = 1f;
        }

        private void OnDisable()
        {
            if (IsSprinting)
            {
                StopSprint();
            }
        }
    }
}