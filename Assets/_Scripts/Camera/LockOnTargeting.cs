using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Turtinia.Camera
{
    public enum LockMode
    {
        Free,    // freelook, no target
        Soft,    // in combat: auto-faces the best target (GDD soft-lock)
        Pinned   // player pressed Lock On: target held until released, dead or out of range
    }

    /// <summary>
    /// Decides WHAT is targeted and WHEN. Contains no camera code at all.
    /// Presenters (DirectLockPresenter, StateDrivenLockPresenter) react to OnModeChanged.
    /// </summary>
    public class LockOnTargeting : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Transform player;
        [SerializeField] InputActionReference lockOnAction;

        [Header("Targeting")]
        [SerializeField] LayerMask enemyMask;
        [Tooltip("Optional: walls and obstacles that block locking. 0 = skip line-of-sight checks.")]
        [SerializeField] LayerMask obstacleMask;
        [Tooltip("Height of the line-of-sight ray origin above the player's feet.")]
        [SerializeField] float eyeHeight = 1.5f;
        [Tooltip("GDD: enemy sight range, 20 m. Also decides when you are in combat.")]
        [SerializeField] float acquireRange = 20f;
        [Tooltip("A held target is dropped beyond this distance (larger than acquireRange to avoid flicker).")]
        [SerializeField] float breakRange = 28f;
        [Tooltip("GDD: out of combat after 5 s with no enemy in range.")]
        [SerializeField] float combatExitDelay = 5f;
        [Tooltip("GDD behaviour. Off = lock only when the player presses the button.")]
        [SerializeField] bool autoLockInCombat = true;
        [Tooltip("In Soft mode a new target must be this many metres better to take over.")]
        [SerializeField] float switchMargin = 3f;
        [SerializeField] float minSwitchInterval = 0.5f;
        [SerializeField] float scanInterval = 0.1f;

        public event Action<LockMode> OnModeChanged;
        public event Action<LockOnTarget> OnTargetChanged;   // null when released

        public LockMode Mode { get; private set; } = LockMode.Free;
        public LockOnTarget Target { get; private set; }
        public bool InCombat { get; private set; }
        public bool HasTarget => Target != null && Mode != LockMode.Free;
        public Vector3 TargetAimPosition => Target != null ? Target.AimPoint.position : Vector3.zero;
        public Transform Player => player;

        readonly Collider[] buffer = new Collider[32];
        LockOnTarget candidate;
        float candidateScore;
        float lastInRangeTime = -999f;
        float lastSwitchTime;
        float scanTimer;

        void OnEnable()
        {
            lockOnAction.action.performed += OnLockOnPressed;
            lockOnAction.action.Enable();
        }

        void OnDisable()
        {
            lockOnAction.action.performed -= OnLockOnPressed;
        }

        void Update()
        {
            scanTimer -= Time.deltaTime;
            if (scanTimer > 0f) return;
            scanTimer = scanInterval;

            Scan();
            UpdateMode();
        }

        void OnLockOnPressed(InputAction.CallbackContext _)
        {
            switch (Mode)
            {
                case LockMode.Free:
                    Scan(); // never act on stale data
                    if (candidate != null) SetTarget(candidate, LockMode.Pinned);
                    break;
                case LockMode.Soft:
                    SetMode(LockMode.Pinned);
                    break;
                case LockMode.Pinned:
                    if (autoLockInCombat && InCombat) SetMode(LockMode.Soft);
                    else Unlock();
                    break;
            }
        }

        float Score(LockOnTarget t) =>
            Vector3.Distance(player.position, t.AimPoint.position) - t.priorityBonus;

        bool StillValid(LockOnTarget t) =>
            t != null && t.IsValid &&
            Vector3.Distance(player.position, t.AimPoint.position) <= breakRange;

        void Scan()
        {
            int n = Physics.OverlapSphereNonAlloc(player.position, acquireRange, buffer,
                                                  enemyMask, QueryTriggerInteraction.Ignore);
            Vector3 eye = player.position + Vector3.up * eyeHeight;
            LockOnTarget best = null;
            float bestScore = float.MaxValue;
            bool anyInRange = false;

            for (int i = 0; i < n; i++)
            {
                var t = buffer[i].GetComponentInParent<LockOnTarget>();
                if (t == null || !t.IsValid) continue;

                anyInRange = true; // combat ignores walls; only locking needs line of sight

                if (obstacleMask.value != 0 && Physics.Linecast(eye, t.AimPoint.position, obstacleMask))
                    continue;

                float s = Score(t);
                if (s < bestScore) { best = t; bestScore = s; }
            }

            candidate = best;
            candidateScore = bestScore;

            if (anyInRange) lastInRangeTime = Time.time;
            InCombat = Time.time - lastInRangeTime < combatExitDelay;
        }

        void UpdateMode()
        {
            switch (Mode)
            {
                case LockMode.Free:
                    if (autoLockInCombat && InCombat && candidate != null)
                        SetTarget(candidate, LockMode.Soft);
                    break;

                case LockMode.Soft:
                    if (!InCombat) { Unlock(); break; }

                    if (!StillValid(Target))
                    {
                        if (candidate != null) SetTarget(candidate, LockMode.Soft);
                        else Unlock();
                    }
                    else if (candidate != null && candidate != Target &&
                             candidateScore < Score(Target) - switchMargin &&
                             Time.time - lastSwitchTime > minSwitchInterval)
                    {
                        SetTarget(candidate, LockMode.Soft); // hysteresis: switch only on a clear win
                    }
                    break;

                case LockMode.Pinned:
                    if (!StillValid(Target))
                    {
                        if (autoLockInCombat && InCombat && candidate != null)
                            SetTarget(candidate, LockMode.Soft);
                        else
                            Unlock();
                    }
                    break;
            }
        }

        void SetTarget(LockOnTarget t, LockMode newMode)
        {
            if (t != Target)
            {
                Target = t;
                lastSwitchTime = Time.time;
                OnTargetChanged?.Invoke(Target);
            }
            SetMode(newMode);
        }

        void Unlock()
        {
            if (Target != null)
            {
                Target = null;
                OnTargetChanged?.Invoke(null);
            }
            SetMode(LockMode.Free);
        }

        void SetMode(LockMode newMode)
        {
            if (newMode == Mode) return;
            Mode = newMode;
            OnModeChanged?.Invoke(Mode);
        }

        void OnDrawGizmosSelected()
        {
            if (player == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, acquireRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, breakRange);
        }
    }
}
