using UnityEngine;

namespace Turtinia.Camera
{
    /// <summary>
    /// Put this on every enemy and boss the camera is allowed to lock onto
    /// (the collider can be on a child, as long as it is on the "Enemy" layer).
    /// </summary>
    public class LockOnTarget : MonoBehaviour
    {
        [Tooltip("Where the camera and auto-aim look, roughly chest height. Defaults to this transform.")]
        [SerializeField] Transform aimPoint;

        [Tooltip("Metres of distance this target 'wins' by when the camera picks one. " +
                 "Give bosses about 10 so summons do not steal the lock.")]
        public float priorityBonus;

        /// <summary>Set to false from your health script when the enemy dies.</summary>
        public bool Alive { get; set; } = true;

        public Transform AimPoint => aimPoint != null ? aimPoint : transform;
        public bool IsValid => Alive && isActiveAndEnabled;
    }
}
