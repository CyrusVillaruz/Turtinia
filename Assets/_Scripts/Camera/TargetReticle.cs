using UnityEngine;

namespace Turtinia.Camera
{
    public class TargetReticle : MonoBehaviour
    {
        [SerializeField] private TargetFinder _targetFinder;
        [SerializeField] private RectTransform _crosshair;
        [SerializeField] private RectTransform _lockReticle;
        [SerializeField] private Vector3 _worldOffset = Vector3.zero;
        private UnityEngine.Camera _camera;
        private Canvas _rootCanvas;
        private RectTransform _reticleParent;

        private void Start()
        {
            _camera = UnityEngine.Camera.main;
        }

        private void LateUpdate()
        {
            Transform target = _targetFinder.CurrentTarget;
            bool locked = _targetFinder.Mode != LockMode.FreeLook && target && _camera;

            SetVisible(_crosshair, !locked);

            if (!locked)
            {
                SetVisible(_lockReticle, false);
                return;
            }

            Vector3 screen = _camera.WorldToScreenPoint(target.position + _worldOffset);
            bool inFront = screen.z > 0f;

            SetVisible(_lockReticle, inFront);
            if (!inFront) return;

            _lockReticle.position = new Vector3(screen.x, screen.y, 0f);
        }

        private static void SetVisible(RectTransform element, bool visible)
        {
            if (element.gameObject.activeSelf != visible)
            {
                element.gameObject.SetActive(visible);
            }
        }
    }
}