using UnityEngine;

public class Target : MonoBehaviour
{
    private Collider _collider;
    private Camera _camera;

    private Plane[] _frustumPlanes;
    private bool _isVisible;
    private bool _isInPool;
    
    private void Start()
    {
        _collider = GetComponent<Collider>();
        _camera = Camera.main;
    }

    private void Update()
    {
        _frustumPlanes = GeometryUtility.CalculateFrustumPlanes(_camera);
        _isVisible = GeometryUtility.TestPlanesAABB(_frustumPlanes, _collider.bounds);

        if (_isVisible && !_isInPool)
        {
            TargetFinder.AddToPool(this.transform);
            _isInPool = true;
        }
        else
        {
            //either we're not visible - or we are in the pool
            //remove ourselves from the target pool
            if (_isVisible) return;
            TargetFinder.RemoveFromPool(this.transform);
            _isInPool = false;


        }
    }
    
    private void OnDisable()
    {
        TargetFinder.RemoveFromPool(this.transform);
    }

    private void OnDestroy()
    {
        TargetFinder.RemoveFromPool(this.transform);
    }
}
