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
        [SerializeField] private ResourceChangeEventChannel _changed;

        private float _current;
        private float _maxBonus;
        private float _regenResumesAt;

        public ResourceType Type => _type;
        public float Current => _current;
        public float Max => _baseMax + _maxBonus;

        protected virtual void Awake() => _current = Max;
        protected virtual void Start() => Raise();

        private void Update()
        {
            if (_regenPerSecond <= 0f || _current >= Max || Time.time < _regenResumesAt) return;
            Change(_regenPerSecond * Time.deltaTime);
        }

        public bool CanAfford(float amount) => _current >= amount;

        public bool TryConsume(float amount)
        {
            if (!CanAfford(amount)) return false;
            Change(-amount);
            _regenResumesAt = Time.time + _regenDelay;
            return true;
        }

        public void Restore(float amount) => Change(amount);

        // TODO: Call this method for the relic that increases max mana.
        public void SetMaxBonus(float bonus)
        {
            _maxBonus = bonus;
            Change(0f);
        }

        protected void Change(float delta)
        {
            _current = Mathf.Clamp(_current + delta, 0f, Max);
            Raise();
        }

        private void Raise()
        {
            if (_changed)
            {
                _changed.Invoke(new ResourceChange { Current = _current, Max = Max });
            }
        }
    }
}