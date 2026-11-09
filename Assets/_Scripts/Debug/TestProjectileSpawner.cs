using UnityEngine;

namespace Turtinia.Debug
{
    public class TestProjectileSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private float _interval = 2f;

        private float _nextShot;

        private void Update()
        {
            if (Time.time < _nextShot) return;
            _nextShot = Time.time + _interval;
            Instantiate(_projectilePrefab, transform.position, transform.rotation);
        }
    }
}