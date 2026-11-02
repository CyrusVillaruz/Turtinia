using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Turtinia.Camera
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private float _zoomSpeed = 2.0f;
        [SerializeField] private float _zoomLerpSpeed = 10.0f;
        [SerializeField] private float _minDistance = 3.0f;
        [SerializeField] private float _maxDistance = 15.0f;
        
        private PlayerActions _playerActions;
        private CinemachineCamera _camera;
        private CinemachineOrbitalFollow _orbitalFollow;
        private Vector2 _scrollDelta;

        private float _targetZoom;
        private float _currentZoom;

        private void Awake()
        {
            _playerActions = new PlayerActions();
            _playerActions.Enable();
            _playerActions.Camera.MouseZoom.performed += HandleMouseScroll;
            
            Cursor.lockState  = CursorLockMode.Locked;
            _camera = GetComponent<CinemachineCamera>();
            _orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
            _targetZoom = _currentZoom = _orbitalFollow.Radius;
        }

        private void HandleMouseScroll(InputAction.CallbackContext context)
        {
            _scrollDelta = context.ReadValue<Vector2>();
        }

        private void Update()
        {
            if (_scrollDelta.y != 0)
            {
                if (_orbitalFollow != null)
                {
                    _targetZoom = Mathf.Clamp(_orbitalFollow.Radius - _scrollDelta.y * _zoomSpeed,
                        _minDistance, _maxDistance);
                    _scrollDelta = Vector3.zero;
                }
            }

            _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, Time.deltaTime * _zoomLerpSpeed);
            _orbitalFollow.Radius = _currentZoom;
        }
    }
}