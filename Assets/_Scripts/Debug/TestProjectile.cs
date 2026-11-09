using Turtinia.Combat;
using Turtinia.Interfaces;
using UnityEngine;

namespace Turtinia.Debug
{
    public class TestProjectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _damage = 1f;
        [SerializeField] private float _knockbackDistance = 1f;
        [SerializeField] private float _lifetime = 5f;

        private void Start()
        {
            Destroy(gameObject, _lifetime);
        }

        private void Update()
        {
            transform.position += transform.forward * (_speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable == null) return;

            if (damageable.TakeDamage(_damage))
            {
                other.GetComponent<IKnockbackable>()?.Knockback(transform.forward, _knockbackDistance);
            }
            Destroy(gameObject);
        }
    }
}