using Unity.Cinemachine;
using UnityEngine;

namespace Turtinia.Camera
{
    /// <summary>
    /// Base class for "how the camera shows the lock state". Add exactly ONE concrete presenter to the rig.
    /// The base class also hands the view back to the freelook camera when a lock ends, so every
    /// presenter gets a smooth unlock for free.
    /// </summary>
    public abstract class LockCameraPresenter : MonoBehaviour
    {
        [SerializeField] protected LockOnTargeting targeting;
        [SerializeField] protected LockPivotDriver pivotDriver;
        [Tooltip("Optional but recommended. The freelook camera's Orbital Follow, with Binding Mode = World Space.")]
        [SerializeField] protected CinemachineOrbitalFollow freelookOrbit;

        bool wasLocked;

        protected virtual void OnEnable()
        {
            wasLocked = false;
            targeting.OnModeChanged += Handle;
            Handle(targeting.Mode);
        }

        protected virtual void OnDisable() => targeting.OnModeChanged -= Handle;

        void Handle(LockMode mode)
        {
            bool locked = mode != LockMode.Free;

            // Sync BEFORE the camera switches, so freelook starts where the lock camera ended.
            // If the view pops on unlock, offset or negate the yaw here.
            if (wasLocked && !locked && freelookOrbit != null)
            {
                freelookOrbit.HorizontalAxis.Value = Mathf.DeltaAngle(0f, pivotDriver.Yaw);
                freelookOrbit.VerticalAxis.Value = pivotDriver.Pitch;
            }

            wasLocked = locked;
            Apply(mode);
        }

        protected abstract void Apply(LockMode mode);
    }
}
