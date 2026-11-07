using UnityEngine;

namespace Turtinia.Camera
{
    /// <summary>
    /// Keeps a UI reticle on top of the currently locked target by projecting the
    /// target's world position to the screen every frame. This doesn't depend on
    /// the camera keeping the target perfectly centered.
    /// Put this on any object (e.g. TargetCanvas) and assign the fields.
    /// </summary>
    [DefaultExecutionOrder(300)] // run after CinemachineBrain so we read the final camera pose
    public class TargetReticle : MonoBehaviour
    {
        [SerializeField] private TargetFinder _targetFinder;
        [SerializeField] private RectTransform _reticle;
        [SerializeField] private Vector3 _worldOffset = Vector3.zero; // e.g. (0, 1, 0) to aim at chest height

        private UnityEngine.Camera _camera;
        private Canvas _rootCanvas;
        private RectTransform _parentRect;

        private void Start()
        {
            _camera = UnityEngine.Camera.main;
            _rootCanvas = _reticle.GetComponentInParent<Canvas>().rootCanvas;
            _parentRect = _reticle.parent as RectTransform;
        }

        private void LateUpdate()
        {
            Transform target = _targetFinder.CurrentTarget;

            // No target: leave the reticle where it was so any "lock off"
            // animation can finish playing in place.
            if (!target || !_camera) return;

            Vector3 screen = _camera.WorldToScreenPoint(target.position + _worldOffset);
            if (screen.z <= 0f) return; // behind the camera

            UnityEngine.Camera uiCamera = _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : _rootCanvas.worldCamera;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _parentRect, screen, uiCamera, out Vector2 local))
            {
                _reticle.localPosition = local;
            }
        }
    }
}
