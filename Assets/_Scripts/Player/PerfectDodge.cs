using System;
using Turtinia.Combat;
using Turtinia.Events;
using Turtinia.Interfaces;
using UnityEngine;

namespace Turtinia.Player
{
    public class PerfectDodge : MonoBehaviour
    {
        [SerializeField] private float _window = 0.15f;
        [SerializeField, Range(0f, 1f)] private float _refundFraction = 1f;
        
        // TODO: Create perfect dodge channel for VFX when performing a perfect dodge.
        // [SerializeField] private EventChannel _onPerfectDodge;

        private PlayerDodge _dodge;
        private PlayerHealth _health;
        private ResourceHub _hub;
        private bool _refunded;
        
        // Local version of event for InvulVisual to update material on perfect dodge.
        public event Action OnPerfectDodge;

        private void Awake()
        {
            _dodge = GetComponent<PlayerDodge>();
            _health = GetComponent<PlayerHealth>();
            _hub = GetComponent<ResourceHub>();
        }

        private void OnEnable()
        {
            _dodge.OnDodgeStarted += OnDodgeStarted;
            _health.OnHitAvoided += OnHitAvoided;
        }

        private void OnDisable()
        {
            _dodge.OnDodgeStarted -= OnDodgeStarted;
            _health.OnHitAvoided -= OnHitAvoided;
        }

        private void OnDodgeStarted() => _refunded = false;

        private void OnHitAvoided()
        {
            if (_refunded || !_dodge.IsDodging) return;
            if (Time.time - _dodge.DodgeStartTime > _window) return;

            _refunded = true;
            foreach (ResourceCost cost in _dodge.Costs)
            {
                if (_hub.TryGet(cost.Type, out IResource pool))
                {
                    pool.Restore(cost.Amount * _refundFraction);
                }
            }
            
            OnPerfectDodge?.Invoke();
            // if (_onPerfectDodge != null)
            // {
            //     _onPerfectDodge.Invoke(new Empty());
            // }
        }
    }
}