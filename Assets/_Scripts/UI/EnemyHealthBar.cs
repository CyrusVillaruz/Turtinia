using Turtinia.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Turtinia.UI
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private ResourcePool _pool;
        [SerializeField] private Image _fill;

        private UnityEngine.Camera _camera;

        private void Awake()
        {
            _camera = UnityEngine.Camera.main;
            if (_pool == null)
            {
                _pool = GetComponentInParent<ResourcePool>();
            }
        }

        private void OnEnable()
        {
            _pool.OnValueChanged += OnChanged;
            OnChanged(new Resource { Current = _pool.Current, Max = _pool.Max });
        }

        private void OnDisable()
        {
            _pool.OnValueChanged -= OnChanged;
        }

        private void OnChanged(Resource change)
        {
            _fill.fillAmount = change.Max > 0f ? change.Current / change.Max : 0f;
        }

        private void LateUpdate()
        {
            transform.rotation = _camera.transform.rotation;
        }
    }
}