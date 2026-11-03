using UnityEngine;

namespace Turtinia.Camera
{
    /// <summary>
    /// Presenter for a Cinemachine State-Driven Camera. Sets the integer parameter "LockMode" on the dummy
    /// Animator: 0 = Free, 1 = Soft, 2 = Pinned. Map the states to cameras (and blends) on the
    /// State-Driven Camera itself.
    /// </summary>
    public class StateDrivenLockPresenter : LockCameraPresenter
    {
        [SerializeField] Animator cameraStates;
        [SerializeField] string parameterName = "LockMode";

        int hash;

        protected override void OnEnable()
        {
            hash = Animator.StringToHash(parameterName);
            base.OnEnable();
        }

        protected override void Apply(LockMode mode)
        {
            if (cameraStates != null) cameraStates.SetInteger(hash, (int)mode);
        }
    }
}
