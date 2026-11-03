using System;
using UnityEngine;

namespace Turtinia.Camera
{
    /// <summary>
    /// Moves an empty "LockPivot" object to the player and turns it to face the locked target.
    /// The lock camera (any kind) uses the pivot as its Tracking Target. Camera-agnostic.
    /// </summary>
    [DefaultExecutionOrder(50)]
    public class LockPivotDriver : MonoBehaviour
    {
        [SerializeField] LockOnTargeting targeting;
        [Tooltip("Empty object at the scene root, NOT a child of the player.")]
        [SerializeField] Transform pivot;

        [Header("Placeholders, tune in the Milestone 1 prototype")]
        [SerializeField] float pivotHeight = 1.5f;
        [SerializeField] float pitch = 15f;
        [SerializeField] float yawSmoothTime = 0.12f;

        public float Yaw { get; private set; }
        public float Pitch => pitch;

        float yawVelocity;
        LockMode previous = LockMode.Free;
        private UnityEngine.Camera mainCam;

        private void Start()
        {
            mainCam = UnityEngine.Camera.main;
        }

        void OnEnable() => targeting.OnModeChanged += OnMode;
        void OnDisable() => targeting.OnModeChanged -= OnMode;

        void OnMode(LockMode mode)
        {
            // Entering a lock: start from the direction the player is already looking, so nothing whips around.
            if (previous == LockMode.Free && mode != LockMode.Free)
            {
                var cam = mainCam;
                if (cam != null) Yaw = cam.transform.eulerAngles.y;
                yawVelocity = 0f;
            }
            previous = mode;
        }

        void LateUpdate()
        {
            if (!targeting.HasTarget || pivot == null) return;

            Transform player = targeting.Player;
            Vector3 to = targeting.TargetAimPosition - player.position;
            to.y = 0f;

            if (to.sqrMagnitude > 0.01f)
            {
                float desired = Mathf.Atan2(to.x, to.z) * Mathf.Rad2Deg;
                Yaw = Mathf.SmoothDampAngle(Yaw, desired, ref yawVelocity, yawSmoothTime);
            }

            pivot.SetPositionAndRotation(player.position + Vector3.up * pivotHeight,
                                         Quaternion.Euler(pitch, Yaw, 0f));
        }
    }
}
