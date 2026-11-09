using UnityEngine;

namespace Turtinia.Interfaces
{
    public interface IKnockbackable
    {
        void Knockback(Vector3 direction, float distance);
    }
}