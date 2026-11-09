using Turtinia.Combat;
using UnityEngine;

namespace Turtinia.Debug
{
    public class TestTarget : ResourcePool, IDamageable
    {
        public bool TakeDamage(float damage)
        {
            UpdateResource(-damage);
            if (Current <= 0f)
            {
                Destroy(gameObject);
            }
            return true;
        }
    }
}