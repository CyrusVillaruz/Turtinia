using Turtinia.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Turtinia.UI
{
    public class StaminaResourceBar : MonoBehaviour
    {
        [SerializeField] private Image _fill;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Transform _target;
        [SerializeField] private Vector2 _offset = new Vector2(0.8f, 0.5f);
        [SerializeField] private float _hideDelay = 1f;
        [SerializeField] private float _fadeSpeed = 4f;

        private UnityEngine.Camera _camera;
        private float _visibleUntil;

        private void Awake()
        {
            _camera = UnityEngine.Camera.main;
            _group.alpha = 0f;
        }

        public void OnResourceChanged(Resource change)
        {
            _fill.fillAmount = change.Max > 0f ? change.Current / change.Max : 0f;

            if (change.Current < change.Max)
            {
                _visibleUntil = Time.time + _hideDelay;
            }
        }

        private void LateUpdate()
        {
            Transform cam = _camera.transform;
            transform.position = _target.position + cam.right * _offset.x + Vector3.up * _offset.y;
            transform.rotation = cam.rotation;

            float goal = Time.time < _visibleUntil ? 1f : 0f;
            _group.alpha = Mathf.MoveTowards(_group.alpha, goal, _fadeSpeed * Time.deltaTime);
        }
    }
}