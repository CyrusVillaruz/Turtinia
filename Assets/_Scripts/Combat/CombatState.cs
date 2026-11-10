using System;
using Turtinia.Camera;
using UnityEngine;

namespace Turtinia.Combat
{
    public class CombatState : MonoBehaviour
    {
        [SerializeField] private float _outOfCombatDelay = 5f;
        [SerializeField] private float _checkInterval = 0.1f;

        public bool IsInCombat { get; private set; }
        public event Action<bool> OnCombatStateChanged;

        private float _lastSeenTime;
        private float _nextCheckTime;

        private void Update()
        {
            if (Time.time < _nextCheckTime) return;
            _nextCheckTime = Time.time + _checkInterval;

            if (AnyEnemySeesPlayer())
            {
                _lastSeenTime = Time.time;
                SetInCombat(true);
            }
            else if (IsInCombat && Time.time - _lastSeenTime >= _outOfCombatDelay)
            {
                SetInCombat(false);
            }
        }
        
        // TODO: Call when enemy sees or chases the player. For now, check through distance to player.
        private bool AnyEnemySeesPlayer()
        {
            Vector3 playerPosition = transform.position;

            foreach (Target target in Target.AllTargets)
            {
                if (!target.IsEnemy) continue;

                float range = target.SightRange;
                if ((target.transform.position - playerPosition).sqrMagnitude <= range * range)
                {
                    return true;
                }
            }

            return false;
        }

        private void SetInCombat(bool value)
        {
            if (IsInCombat == value) return;

            IsInCombat = value;
            OnCombatStateChanged?.Invoke(value);
        }
    }
}
