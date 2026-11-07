using UnityEngine;

namespace Turtinia.Camera
{
    public class Target : MonoBehaviour
    {
        private Collider _collider;
        private UnityEngine.Camera _camera;

        private readonly Plane[] _frustumPlanes = new Plane[6];
        private bool _isInPool;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _camera = UnityEngine.Camera.main;
        }

        private void Update()
        {
            if (!_camera) return;

            GeometryUtility.CalculateFrustumPlanes(_camera, _frustumPlanes);
            bool isVisible = GeometryUtility.TestPlanesAABB(_frustumPlanes, _collider.bounds);

            if (isVisible == _isInPool) return;

            if (isVisible) TargetFinder.AddToPool(transform);
            else TargetFinder.RemoveFromPool(transform);

            _isInPool = isVisible;
        }

        private void OnDisable()
        {
            TargetFinder.RemoveFromPool(transform);
            _isInPool = false;
        }
    }
}
