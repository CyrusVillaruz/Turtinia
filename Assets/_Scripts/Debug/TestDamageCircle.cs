using System;
using Turtinia.Combat;
using Turtinia.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace Turtinia.Debug
{
    public class TestDamageCircle : MonoBehaviour
    {
        [SerializeField] private float _damage = 1.0f;
        [SerializeField] private float _knockbackDistance = 1.0f;

        private void OnTriggerStay(Collider other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable == null || !damageable.TakeDamage(_damage)) return;

            Vector3 direction = other.transform.position - transform.position;
            other.GetComponent<IKnockbackable>()?.Knockback(direction, _knockbackDistance);
        }
    }
}