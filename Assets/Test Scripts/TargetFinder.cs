using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using System.Collections;

public class TargetFinder : MonoBehaviour
{
    private static List<Transform> _targetPool;
    private static DistanceClass _dc;
    private static readonly int _lockedOn = Animator.StringToHash("lockedOn");

    private Transform _currentTarget;
    [SerializeField] private CinemachineCamera _playerCamera;
    [SerializeField] private CinemachineCamera _targetCamera;
    [SerializeField] private Animator _targetAnimator;
    [SerializeField] private float _cameraLookSpeedThreshold = 2.0f;
    
    public List<Transform> TargetPoolView;

    private bool _isLockedOn;
    private bool _isOnCooldown;
    
    private PlayerActions _playerActions;

    private void Awake()
    {
        _playerActions = new PlayerActions();
    }

    private void OnEnable()
    {
        _playerActions.Enable();
    }

    private void OnDisable()
    {
        _playerActions.Disable();
    }

    private void Start()
    {
        _targetPool ??= new List<Transform>();
        _dc ??= new DistanceClass();
    }

    private void Update()
    {
        TargetPoolView = _targetPool; 
        if (_isLockedOn && !_currentTarget) {
            LockOff();
        }

        if (_playerActions.Camera.TargetLock.WasPressedThisFrame())
        {
            if (_isLockedOn) LockOff();
            else LockOn();
        }

        if (_isLockedOn) {
            if (_playerActions.Gameplay.Look.ReadValue<Vector2>().x > _cameraLookSpeedThreshold)
            {
                // Select the target on the right.
                SelectTarget(1);
            }
            else if (_playerActions.Gameplay.Look.ReadValue<Vector2>().x <= -_cameraLookSpeedThreshold)
            {
                // Select target on left.
                SelectTarget(-1);
            }
        }
    }

    private void LockOn() 
    {
        _currentTarget = NearestTarget();

        if (!_currentTarget) return;
        
        _targetCamera.LookAt = _currentTarget;
        _targetCamera.gameObject.SetActive(true);
        _playerCamera.gameObject.SetActive(false);
        _isLockedOn = true;
        _targetAnimator.SetBool(_lockedOn, true);

    }

    private void LockOff() 
    {
        _currentTarget = null;
        _targetCamera.LookAt = null;
        _targetCamera.gameObject.SetActive(false);
        _playerCamera.gameObject.SetActive(true);
        _isLockedOn = false;
        _targetAnimator.SetBool(_lockedOn, false);
    }

    //next is either 1 or -1 > represents moving to the left or right of our pool
    private void SelectTarget(int next)
    {
        if (_isOnCooldown || _targetPool is not { Count: > 1 }) return;
        
        System.Predicate<Transform> predicate = FindTransform;
        int currentIndex = _targetPool.FindIndex(predicate);
        int nextIndex = currentIndex + next; // 0, 1, 2.

        if (nextIndex < 0 || nextIndex >= _targetPool.Count) return;
        
        _currentTarget = _targetPool[nextIndex];
        _targetCamera.LookAt = _currentTarget;
        _isOnCooldown = true;
        StartCoroutine(nameof(ResetCoolDown));
    }

    private bool FindTransform(Transform transform) { 
        return transform.Equals(_currentTarget);
    }

    IEnumerator ResetCoolDown() {
        yield return new WaitForSeconds(0.5f);
        _isOnCooldown = false;
    }

    // Go through pool and return the target nearest the center of the screen.
    private Transform NearestTarget() 
    {
        if (_targetPool != null && _targetPool.Count > 0) 
        {
            Vector3 center = new Vector3(0.5f, 0.5f, 0.0f);
            Camera cam = Camera.main;

            int minIndex = 0;
            float shortestDistance = 1000.0f;

            for (int i = 0; i < _targetPool.Count; i++)
            {
                if (cam != null)
                {
                    Vector3 targetViewport = cam.WorldToViewportPoint(_targetPool[i].position);
                    targetViewport -= Vector3.forward * targetViewport.z;

                    float targetDistanceFromCenter = Vector3.Distance(targetViewport, center);

                    if (targetDistanceFromCenter < shortestDistance)
                    {
                        shortestDistance = targetDistanceFromCenter;
                        minIndex = i;
                    }
                }
            }

            return _targetPool[minIndex];
        } 

        return null;
    }

    public static void AddToPool(Transform target) {
        if (_targetPool != null && !_targetPool.Contains(target)) {
            _targetPool.Add(target);
            _targetPool.Sort(_dc);
        }
    }

    public static void RemoveFromPool(Transform target) {
        if (_targetPool != null && _targetPool.Contains(target))
        {
            _targetPool.Remove(target);
            _targetPool.Sort(_dc);
        }
    }
}

public class DistanceClass : IComparer<Transform> {
    public int Compare(Transform x, Transform y) {

        Camera cam = Camera.main;

        Vector3 xViewport = cam.WorldToViewportPoint(x.position);
        Vector3 yViewport = cam.WorldToViewportPoint(y.position);

        if (xViewport.x < yViewport.x)
        {
            return -1;
        }
        else if (xViewport.x == yViewport.x)
        {
            return 0;
        }
        else {
            return 1;
        }

    }
}
