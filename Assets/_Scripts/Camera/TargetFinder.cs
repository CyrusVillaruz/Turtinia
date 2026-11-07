using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Turtinia.Camera
{
    public class TargetFinder : MonoBehaviour
    {
        private static List<Transform> _targetPool;
        private static readonly int _lockedOn = Animator.StringToHash("lockedOn");

        [SerializeField] private CinemachineCamera _playerCamera;
        [SerializeField] private CinemachineCamera _targetCamera;
        [SerializeField] private Animator _targetAnimator;
        [SerializeField] private float _cameraLookSpeedThreshold = 2.0f;

        public List<Transform> TargetPoolView;
        private Transform _currentTarget;

        public Transform CurrentTarget => _currentTarget;

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

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // Make sure we always start in the free-look state, regardless of
            // which camera happens to be enabled in the scene.
            LockOff();
        }

        private void Update()
        {
            TargetPoolView = _targetPool;

            if (_isLockedOn && !_currentTarget)
            {
                LockOff();
            }

            if (_playerActions.Camera.TargetLock.WasPressedThisFrame())
            {
                if (_isLockedOn) LockOff();
                else LockOn();
            }

            if (_isLockedOn)
            {
                float lookX = _playerActions.Gameplay.Look.ReadValue<Vector2>().x;

                if (lookX > _cameraLookSpeedThreshold)
                {
                    SelectTarget(1);   // target to the right on screen
                }
                else if (lookX <= -_cameraLookSpeedThreshold)
                {
                    SelectTarget(-1);  // target to the left on screen
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
            if (_targetAnimator) _targetAnimator.SetBool(_lockedOn, true);
        }

        private void LockOff()
        {
            _currentTarget = null;
            _targetCamera.LookAt = null;
            _targetCamera.gameObject.SetActive(false);
            _playerCamera.gameObject.SetActive(true);
            _isLockedOn = false;
            if (_targetAnimator) _targetAnimator.SetBool(_lockedOn, false);
        }

        // direction: 1 = next target to the right on screen, -1 = to the left.
        // Screen-space order is computed at selection time, so it's always
        // relative to where the camera is *right now*.
        private void SelectTarget(int direction)
        {
            if (_isOnCooldown || !_currentTarget || _targetPool is not { Count: > 1 }) return;

            UnityEngine.Camera cam = UnityEngine.Camera.main;
            if (!cam) return;

            float currentX = cam.WorldToViewportPoint(_currentTarget.position).x;

            Transform best = null;
            float bestDelta = float.MaxValue;

            foreach (Transform candidate in _targetPool)
            {
                if (candidate == _currentTarget) continue;

                // Positive when the candidate is in the requested direction.
                float delta = (cam.WorldToViewportPoint(candidate.position).x - currentX) * direction;

                if (delta <= 0f || delta >= bestDelta) continue;

                best = candidate;
                bestDelta = delta;
            }

            if (!best) return;

            _currentTarget = best;
            _targetCamera.LookAt = _currentTarget;
            _isOnCooldown = true;
            StartCoroutine(ResetCoolDown());
        }

        private IEnumerator ResetCoolDown()
        {
            yield return new WaitForSeconds(0.5f);
            _isOnCooldown = false;
        }

        // Go through pool and return the target nearest the center of the screen.
        private Transform NearestTarget()
        {
            UnityEngine.Camera cam = UnityEngine.Camera.main;
            if (!cam || _targetPool == null || _targetPool.Count == 0) return null;

            Vector2 center = new Vector2(0.5f, 0.5f);
            Transform nearest = null;
            float shortestDistance = float.MaxValue;

            foreach (Transform t in _targetPool)
            {
                Vector3 viewport = cam.WorldToViewportPoint(t.position);
                float distance = Vector2.Distance(new Vector2(viewport.x, viewport.y), center);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearest = t;
                }
            }

            return nearest;
        }

        public static void AddToPool(Transform target)
        {
            if (_targetPool != null && !_targetPool.Contains(target))
            {
                _targetPool.Add(target);
            }
        }

        public static void RemoveFromPool(Transform target)
        {
            _targetPool?.Remove(target);
        }
    }
}