using System;
using Turtinia.Events;
using Turtinia.Interfaces;
using UnityEngine;

namespace Turtinia.Combat
{
    public class ResourcePool : MonoBehaviour, IResource
    {
        [SerializeField] private ResourceType _type;
        [SerializeField] private float _baseMax = 10f;
        [SerializeField] private float _regenPerSecond;
        [SerializeField] private float _regenDelay = 1f;
        [Tooltip("Leave empty for enemies unless they should share the same resource.")]
        [SerializeField] private ResourceChangeEventChannel _onResourceChangedEventChannel;

        // Local version, used for individual enemies.
        public event Action<Resource> OnValueChanged;
        
        private float _current;
        private float _maxBonus;
        private float _regenResumesAt;

        public ResourceType Type => _type;
        public float Current => _current;
        public float Max => _baseMax + _maxBonus;

        protected virtual void Awake() => _current = Max;
        protected virtual void Start() => RaiseEvent();

        private void Update()
        {
            if (_regenPerSecond <= 0f || _current >= Max || Time.time < _regenResumesAt) return;
            UpdateResource(_regenPerSecond * Time.deltaTime);
        }

        public bool CanAfford(float amount) => _current >= amount;

        public bool TryConsume(float amount)
        {
            if (!CanAfford(amount)) return false;
            UpdateResource(-amount);
            _regenResumesAt = Time.time + _regenDelay;
            return true;
        }

        public void Restore(float amount) => UpdateResource(amount);

        // TODO: Call this method for relics that increases maximum amount of a resource (mana, stamina, health).
        public void SetMaxBonus(float bonus)
        {
            _maxBonus = bonus;
            UpdateResource(0f);
        }

        protected void UpdateResource(float delta)
        {
            _current = Mathf.Clamp(_current + delta, 0f, Max);
            RaiseEvent();
        }

        private void RaiseEvent()
        {
            Resource resource = new Resource { Current = _current, Max = Max };
            
            if (_onResourceChangedEventChannel)
            {
                _onResourceChangedEventChannel.Invoke(resource);
            }
            OnValueChanged?.Invoke(resource);
        }
    }
}