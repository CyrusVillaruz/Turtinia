using Turtinia.Combat;
using UnityEngine;

namespace Turtinia.Spells
{
    public class Projectile : MonoBehaviour
    {
        private float _speed;
        private float _damage;
        private float _despawnAt;
        private Transform _owner;

        public void Launch(float speed, float damage, float lifetime, Transform owner)
        {
            _speed = speed;
            _damage = damage;
            _despawnAt = Time.time + lifetime;
            _owner = owner;
        }

        private void Update()
        {
            if (Time.time >= _despawnAt)
            {
                Despawn();
                return;
            }
            transform.position += transform.forward * (_speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger) return;
            if (_owner != null && other.transform.IsChildOf(_owner)) return;

            other.GetComponentInParent<IDamageable>()?.TakeDamage(_damage);
            Despawn();
        }

        // TODO: Add projectile to a projectile object pool for performance.
        private void Despawn()
        {
            Destroy(gameObject);
        }
    } 
}