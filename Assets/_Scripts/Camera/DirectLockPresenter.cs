using Unity.Cinemachine;
using UnityEngine;

namespace Turtinia.Camera
{
    /// <summary>
    /// Simplest presenter: the lock CinemachineCamera is enabled while locked (give it a higher Priority than the
    /// freelook camera) and the Brain blends between the two. Use this OR StateDrivenLockPresenter.
    /// </summary>
    public class DirectLockPresenter : LockCameraPresenter
    {
        [Tooltip("CinemachineCamera used while locked. Its Tracking Target is the LockPivot.")]
        [SerializeField] CinemachineCamera lockCamera;

        protected override void OnEnable()
        {
            lockCamera.gameObject.SetActive(false);
            base.OnEnable();
        }

        protected override void Apply(LockMode mode) =>
            lockCamera.gameObject.SetActive(mode != LockMode.Free);
    }
}
