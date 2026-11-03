using Turtinia.Camera;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Turtinia.Player
{
    /// <summary>
    /// Minimal third-person controller for testing the lock-on camera: camera-relative movement, sprint,
    /// jump, and facing the locked target while locked. No dodge, health or spells yet.
    /// Defaults follow the GDD (2 m/s walk, +40% sprint, 1 m jump). Raise walkSpeed while testing if it feels slow.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class SimplePlayerController : MonoBehaviour
    {
        [Header("Input (drag actions from your PlayerActions asset)")]
        [SerializeField] InputActionReference moveAction;    // Vector2
        [SerializeField] InputActionReference jumpAction;    // Button
        [SerializeField] InputActionReference sprintAction;  // Button (held)

        [Header("Movement")]
        [SerializeField] float walkSpeed = 2f;
        [SerializeField] float sprintMultiplier = 1.4f;
        [SerializeField] float jumpHeight = 1f;
        [SerializeField] float gravity = -20f;
        [SerializeField] float turnSmoothTime = 0.08f;

        [Header("References")]
        [Tooltip("Optional. Used to face the target while locked on.")]
        [SerializeField] LockOnTargeting lockOn;
        [Tooltip("Defaults to the camera tagged MainCamera.")]
        [SerializeField] Transform cameraTransform;
        [SerializeField] bool lockCursor = true;

        CharacterController controller;
        float verticalVelocity;
        float turnVelocity;
        private UnityEngine.Camera mainCam;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        void OnEnable()
        {
            moveAction.action.Enable();
            jumpAction.action.Enable();
            sprintAction.action.Enable();
        }

        void Start()
        {
            if (lockCursor) SetCursorLocked(true);
            mainCam = UnityEngine.Camera.main;
        }

        void Update()
        {
            // Click back into the game view after pressing Esc.
            if (lockCursor && Cursor.lockState != CursorLockMode.Locked &&
                Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                SetCursorLocked(true);

            if (cameraTransform == null && mainCam != null) cameraTransform = mainCam.transform;
            if (cameraTransform == null) return;

            Vector2 input = moveAction.action.ReadValue<Vector2>();

            // Camera-relative directions on the ground plane.
            Vector3 forward = cameraTransform.forward;
            forward.y = 0f;
            forward = forward.sqrMagnitude > 0.0001f ? forward.normalized : transform.forward;
            Vector3 right = cameraTransform.right;
            right.y = 0f;
            right.Normalize();

            Vector3 move = forward * input.y + right * input.x;
            if (move.sqrMagnitude > 1f) move.Normalize();

            float speed = walkSpeed * (sprintAction.action.IsPressed() ? sprintMultiplier : 1f);

            // Gravity and jump. v = sqrt(2 * g * h) gives exactly jumpHeight metres.
            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            if (controller.isGrounded && jumpAction.action.WasPressedThisFrame())
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            verticalVelocity += gravity * Time.deltaTime;

            controller.Move((move * speed + Vector3.up * verticalVelocity) * Time.deltaTime);

            // Facing: the target while locked, otherwise the movement direction.
            if (lockOn != null && lockOn.HasTarget)
            {
                Vector3 to = lockOn.TargetAimPosition - transform.position;
                to.y = 0f;
                FaceDirection(to);
            }
            else if (move.sqrMagnitude > 0.01f)
            {
                FaceDirection(move);
            }
        }

        void FaceDirection(Vector3 direction)
        {
            if (direction.sqrMagnitude < 0.0001f) return;
            float targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float yaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYaw, ref turnVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        static void SetCursorLocked(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
