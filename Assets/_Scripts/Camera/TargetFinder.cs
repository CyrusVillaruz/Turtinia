using System.Collections.Generic;
using Turtinia.Combat;
using Unity.Cinemachine;
using UnityEngine;

namespace Turtinia.Camera
{
    public enum LockMode
    {
        FreeLook,
        SoftLock,
        HardLock
    }

    public class TargetFinder : MonoBehaviour
    {
        private static readonly List<Transform> _targetPool = new();

        [Header("Camera Settings")]
        [SerializeField] private CinemachineCamera _playerCamera;
        [SerializeField] private CinemachineCamera _targetCamera;
        [SerializeField] private CombatState _combatState;
        [SerializeField] private float _cameraLookSpeedThreshold = 5.0f;

        [Header("Manual Target Switching")]
        [Tooltip("Restricts valid targets to the specified angle.")]
        [SerializeField, Range(10f, 89f)] private float _selectionConeAngle = 60f;
        [SerializeField] private float _selectCooldown = 0.5f;

        [Header("Soft lock")]
        [SerializeField] private float _softLockRetargetInterval = 0.25f;
        [Tooltip("Indicates how close a new enemy must be than current one before soft lock switches to prevent switching between two enemies at similar distances.")]
        [SerializeField] private float _softLockSwitchMargin = 1.5f;

        public List<Transform> TargetPoolView;

        public Transform CurrentTarget => _currentTarget;
        public LockMode Mode => _mode;

        private Transform _currentTarget;
        private LockMode _mode = LockMode.FreeLook;
        private float _nextSelectTime;
        private float _nextSoftLockCheck;

        private PlayerActions _playerActions;
        private UnityEngine.Camera _camera;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => _targetPool.Clear();

        private void Awake()
        {
            _playerActions = new PlayerActions();
            TargetPoolView = _targetPool;
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
            _camera = UnityEngine.Camera.main;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            ClearLock();
        }

        private void Update()
        {
            bool hasCombatState = _combatState;
            bool inCombat = hasCombatState && _combatState.IsInCombat;

            if (_mode != LockMode.FreeLook && (!_currentTarget || !_currentTarget.gameObject.activeInHierarchy))
            {
                ClearLock();
            }

            if (hasCombatState && !inCombat && _mode != LockMode.FreeLook)
            {
                ClearLock();
            }

            if (_playerActions.Camera.TargetLock.WasPressedThisFrame())
            {
                if (_mode == LockMode.HardLock)
                {
                    ClearLock();
                }
                else if (inCombat || !hasCombatState)
                {
                    HardLock();
                }
            }

            if (_mode == LockMode.HardLock)
            {
                HandleTargetSwitching();
            }
            else if (inCombat)
            {
                UpdateSoftLock();
            }
        }


        private void HardLock()
        {
            Transform target = _mode == LockMode.SoftLock ? _currentTarget : NearestToScreenCenter();

            if (target)
            {
                ApplyTarget(target, LockMode.HardLock);
            }
        }

        private void ApplyTarget(Transform target, LockMode mode)
        {
            _currentTarget = target;
            _mode = mode;
            _targetCamera.LookAt = target;
            _targetCamera.gameObject.SetActive(true);
            _playerCamera.gameObject.SetActive(false);
        }

        private void ClearLock()
        {
            if (_mode != LockMode.FreeLook)
            {
                SyncFreeLookToCurrentView();
            }
            
            _currentTarget = null;
            _mode = LockMode.FreeLook;
            _targetCamera.LookAt = null;
            _targetCamera.gameObject.SetActive(false);
            _playerCamera.gameObject.SetActive(true);
        }
        
        private void SyncFreeLookToCurrentView()
        {
            if (!_camera) return;

            Vector3 euler = _camera.transform.eulerAngles;
            float yaw = Mathf.DeltaAngle(0f, euler.y);
            float pitch = Mathf.DeltaAngle(0f, euler.x);

            if (_playerCamera.TryGetComponent(out CinemachinePanTilt panTilt))
            {
                panTilt.PanAxis.Value = yaw;
                panTilt.TiltAxis.Value = pitch;
            }
            else if (_playerCamera.TryGetComponent(out CinemachineOrbitalFollow orbital))
            {
                orbital.HorizontalAxis.Value = yaw;
            }
        }

        private void UpdateSoftLock()
        {
            if (_mode == LockMode.SoftLock && Time.time < _nextSoftLockCheck) return;
            
            _nextSoftLockCheck = Time.time + _softLockRetargetInterval;

            Transform nearest = NearestEnemyToPlayer(out float nearestDistance);

            if (!nearest)
            {
                if (_mode == LockMode.SoftLock)
                {
                    ClearLock();
                }
                return;
            }

            if (_mode == LockMode.SoftLock)
            {
                if (nearest == _currentTarget) return;

                float currentDistance = Vector3.Distance(transform.position, _currentTarget.position);
                if (nearestDistance > currentDistance - _softLockSwitchMargin) return;
            }

            ApplyTarget(nearest, LockMode.SoftLock);
        }

        private Transform NearestEnemyToPlayer(out float distance)
        {
            Vector3 origin = transform.position;
            Transform nearest = null;
            float bestSqr = float.MaxValue;

            foreach (Target target in Target.AllTargets)
            {
                if (!target.IsEnemy) continue;

                float sqr = (target.transform.position - origin).sqrMagnitude;
                if (sqr >= bestSqr) continue;

                nearest = target.transform;
                bestSqr = sqr;
            }

            distance = nearest ? Mathf.Sqrt(bestSqr) : 0f;
            return nearest;
        }

        private void HandleTargetSwitching()
        {
            float lookX = _playerActions.Gameplay.Look.ReadValue<Vector2>().x;


            if (lookX > _cameraLookSpeedThreshold)
            {
                // Camera moved right. Select target right of current locked target.
                SelectTarget(1);
            }
            else if (lookX <= -_cameraLookSpeedThreshold)
            {
                // Camera moved left. Select target left of current locked target.
                SelectTarget(-1);
            }
        }
        
        private void SelectTarget(int direction)
        {
            if (Time.time < _nextSelectTime || !_currentTarget || _targetPool.Count < 2) return;

            if (!_camera) return;

            Vector2 origin = ScreenPosition(_camera, _currentTarget, out _);
            float minCos = Mathf.Cos(_selectionConeAngle * Mathf.Deg2Rad);

            Transform best = null;
            float bestDistance = float.MaxValue;

            foreach (Transform candidate in _targetPool)
            {
                if (candidate == _currentTarget) continue;

                Vector2 offset = ScreenPosition(_camera, candidate, out bool inFront) - origin;
                float distance = offset.magnitude;

                if (!inFront || distance < 0.0001f) continue;

                if (offset.x * direction / distance < minCos) continue;

                if (distance < bestDistance)
                {
                    best = candidate;
                    bestDistance = distance;
                }
            }

            if (!best) return;

            _currentTarget = best;
            _targetCamera.LookAt = best;
            _nextSelectTime = Time.time + _selectCooldown;
        }

        private Transform NearestToScreenCenter()
        {
            if (!_camera) return null;

            Vector2 center = new Vector2(0.5f * _camera.aspect, 0.5f);
            Transform nearest = null;
            float shortest = float.MaxValue;

            foreach (Transform t in _targetPool)
            {
                Vector2 position = ScreenPosition(_camera, t, out bool inFront);
                if (!inFront) continue;

                float distance = Vector2.Distance(position, center);
                if (distance < shortest)
                {
                    shortest = distance;
                    nearest = t;
                }
            }

            return nearest;
        }
        
        private static Vector2 ScreenPosition(UnityEngine.Camera cam, Transform t, out bool inFront)
        {
            Vector3 viewport = cam.WorldToViewportPoint(t.position);
            inFront = viewport.z > 0f;
            return new Vector2(viewport.x * cam.aspect, viewport.y);
        }

        public static void AddToPool(Transform target)
        {
            if (!_targetPool.Contains(target))
            {
                _targetPool.Add(target);
            }
        }

        public static void RemoveFromPool(Transform target)
        {
            _targetPool.Remove(target);
        }
    }
}