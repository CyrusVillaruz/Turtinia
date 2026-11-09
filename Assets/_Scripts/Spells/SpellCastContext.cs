using UnityEngine;

namespace Turtinia.Spells
{
    public readonly struct SpellCastContext
    {
        public readonly Vector3 Origin;
        public readonly Vector3 Direction;
        public readonly Transform Caster;

        public SpellCastContext(Vector3 origin, Vector3 direction, Transform caster)
        {
            Origin = origin;
            Direction = direction;
            Caster = caster;
        }
    }
}