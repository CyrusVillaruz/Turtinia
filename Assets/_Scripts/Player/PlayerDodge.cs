using System;
using System.Collections;
using Turtinia.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Turtinia.Player
{
    public class PlayerDodge : MonoBehaviour
    {
        [SerializeField] private ResourceCost[] _resourceCosts;
        [SerializeField] private float _distance = 3f;
        [SerializeField] private float _iFrameDuration = 0.15f;
        [SerializeField] private float _cooldown = 0.2f;

        private CharacterController _controller;
        private PlayerController _playerController;
        private PlayerHealth _health;
        private ResourceHub _hub;
        private float _nextDodgeTime;

        public event Action OnDodgeStarted;
        public bool IsDodging { get; private set; }
        public float DodgeStartTime { get; private set; }
        public ResourceCost[] Costs => _resourceCosts;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _playerController = GetComponent<PlayerController>();
            _health = GetComponent<PlayerHealth>();
            _hub = GetComponent<ResourceHub>();
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                TryDodge();
            }
        }

        private void TryDodge()
        {
            if (IsDodging || Time.time < _nextDodgeTime) return;
            if (!_hub.TryConsume(_resourceCosts)) return;

            Vector3 direction = _playerController.MoveDirection;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f)
            {
                direction = transform.forward;
            }

            DodgeStartTime = Time.time;
            _health.GrantInvulnerability(_iFrameDuration);
            OnDodgeStarted?.Invoke();
            StartCoroutine(DodgeRoutine(direction.normalized));
        }

        private IEnumerator DodgeRoutine(Vector3 direction)
        {
            IsDodging = true;
            _playerController.IsMovementLocked = true;
            transform.rotation = Quaternion.LookRotation(direction);

            Vector3 total = direction * _distance;
            float elapsed = 0f;
            while (elapsed < _iFrameDuration)
            {
                float dt = Mathf.Min(Time.deltaTime, _iFrameDuration - elapsed);
                _controller.Move(total * (dt / _iFrameDuration));
                elapsed += dt;
                yield return null;
            }

            EndDodge();
        }

        private void EndDodge()
        {
            IsDodging = false;
            _playerController.IsMovementLocked = false;
            _nextDodgeTime = Time.time + _cooldown;
        }

        private void OnDisable()
        {
            if (IsDodging)
            {
                EndDodge();
            }
        }
    }
}