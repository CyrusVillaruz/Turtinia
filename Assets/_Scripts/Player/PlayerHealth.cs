using Turtinia.Combat;
using Turtinia.Events;
using UnityEngine;

namespace Turtinia.Player
{
    public class PlayerHealth : ResourcePool, IDamageable
    {
        [SerializeField] private EventChannel _onPlayerDeath;
        [SerializeField] private FloatEventChannel _invulnerabilityStarted;
        [SerializeField] private float _iFrameDuration = 3.0f;
        
        private float _invulnerableUntil;
        
        public bool TakeDamage(float damage)
        {
            if (Current <= 0f || Time.time < _invulnerableUntil) return false;
            _invulnerableUntil = Time.time + _iFrameDuration;

            Change(-damage);
            _invulnerabilityStarted.Invoke(_iFrameDuration);

            if (Current <= 0f)
            {
                UnityEngine.Debug.Log("Player dead");
                _onPlayerDeath.Invoke(new Empty());
            }
            return true;
        }
    }
}