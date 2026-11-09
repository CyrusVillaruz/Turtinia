using System.Collections.Generic;
using Turtinia.Combat;
using Turtinia.Spells;
using Turtinia.Camera;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Turtinia.Player
{
    public class PlayerSpellcaster : MonoBehaviour
    {
        [SerializeField] private SpellDefinition _basicSpell;

        [Header("Aiming")]
        [SerializeField] private LayerMask _aimMask = ~0;
        [SerializeField] private float _maxAimDistance = 100f;
        [SerializeField] private Vector3 _muzzleOffset = new Vector3(0.7f, 0.5f, 0.4f);

        [Tooltip("Also aim at the auto-selected target while in soft lock. Turn off to keep free crosshair aiming until a hard lock.")]
        [SerializeField] private bool _aimAtSoftLockTarget = true;

        private TargetFinder _targetFinder;
        private readonly Dictionary<SpellDefinition, float> _readyAt = new();
        private ResourceHub _hub;
        private UnityEngine.Camera _camera;
        private bool _attackHeld;

        private void Awake()
        {
            _hub = GetComponent<ResourceHub>();
            _camera = UnityEngine.Camera.main;
            _targetFinder = GetComponent<TargetFinder>();
        }

        private void OnDisable() => _attackHeld = false;

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _attackHeld = true;
                TryCast(_basicSpell);
            }
            else if (context.canceled)
            {
                _attackHeld = false;
            }
        }

        private void Update()
        {
            if (_attackHeld)
            {
                TryCast(_basicSpell);
            }
        }

        public bool TryCast(SpellDefinition spell)
        {
            if (!spell) return false;
            if (_readyAt.TryGetValue(spell, out float readyAt) && Time.time < readyAt) return false;
            if (!_hub.TryConsume(spell.Costs)) return false;

            _readyAt[spell] = Time.time + spell.Cooldown;
            spell.Cast(BuildContext());
            return true;
        }

        private SpellCastContext BuildContext()
        {
            Transform cam = _camera.transform;
            Vector3 flatForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 origin = transform.position
                             + cam.right * _muzzleOffset.x
                             + Vector3.up * _muzzleOffset.y
                             + flatForward * _muzzleOffset.z;

            Vector3 aimPoint = HasLockedTarget(out Transform target)
                ? AimPointOf(target)
                : CrosshairAimPoint();

            Vector3 direction = aimPoint - origin;
            if (direction.sqrMagnitude < 0.25f)
            {
                direction = cam.forward;
            }

            return new SpellCastContext(origin, direction.normalized, transform);
        }

        private bool HasLockedTarget(out Transform target)
        {
            target = _targetFinder ? _targetFinder.CurrentTarget : null;
            if (!target) return false;

            return _targetFinder.Mode == LockMode.HardLock
                   || (_aimAtSoftLockTarget && _targetFinder.Mode == LockMode.SoftLock);
        }

        private static Vector3 AimPointOf(Transform target)
        {
            return target.TryGetComponent(out Collider col) ? col.bounds.center : target.position;
        }

        private Vector3 CrosshairAimPoint()
        {
            Transform cam = _camera.transform;
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            Vector3 rayStart = ray.GetPoint(Vector3.Distance(cam.position, transform.position));
            return Physics.Raycast(rayStart, ray.direction, out RaycastHit hit,
                _maxAimDistance, _aimMask, QueryTriggerInteraction.Ignore)
                ? hit.point
                : rayStart + ray.direction * _maxAimDistance;
        }
    }
}