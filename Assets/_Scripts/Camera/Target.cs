using System.Collections.Generic;
using UnityEngine;

namespace Turtinia.Camera
{
    public class Target : MonoBehaviour
    {
        public static readonly List<Target> AllTargets = new();

        [Tooltip("Set to 0 if this game object is a puzzle object for lock-on targeting.")]
        [SerializeField] private float _sightRange = 20f;

        public float SightRange => _sightRange;
        public bool IsEnemy => _sightRange > 0f;

        private Collider _collider;
        private UnityEngine.Camera _camera;

        private readonly Plane[] _frustumPlanes = new Plane[6];
        private bool _isInPool;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => AllTargets.Clear();

        private void OnEnable()
        {
            AllTargets.Add(this);
        }

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
            AllTargets.Remove(this);
            TargetFinder.RemoveFromPool(transform);
            _isInPool = false;
        }
    }
}