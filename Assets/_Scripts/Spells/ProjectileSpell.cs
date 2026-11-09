using UnityEngine;

namespace Turtinia.Spells
{
    [CreateAssetMenu(menuName = "Spells/Projectile Spell")]
    public class ProjectileSpell : SpellDefinition
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private float _damage = 5f;
        [SerializeField] private float _speed = 20f;
        [SerializeField] private float _lifetime = 3f;

        public override void Cast(SpellCastContext context)
        {
            // TODO: Add projectile to projectile object pool for reuse.
            Projectile projectile = Instantiate(_projectilePrefab, context.Origin, Quaternion.LookRotation(context.Direction));
            projectile.Launch(_speed, _damage, _lifetime, context.Caster);
        }
    }
}