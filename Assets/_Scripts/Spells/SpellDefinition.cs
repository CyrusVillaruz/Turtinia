using Turtinia.Combat;
using UnityEngine;

namespace Turtinia.Spells
{
    public abstract class SpellDefinition : ScriptableObject
    {
        [SerializeField] private ResourceCost[] _costs;
        [SerializeField] private float _cooldown = 0.5f;

        public ResourceCost[] Costs => _costs;
        public float Cooldown => _cooldown;

        public abstract void Cast(SpellCastContext context);
    }
}