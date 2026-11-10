using System.Collections;
using Turtinia.Interfaces;
using UnityEngine;

namespace Turtinia.Player
{
    public class PlayerKnockback : MonoBehaviour, IKnockbackable
    {
        [SerializeField] private float _duration = 0.15f;

        private CharacterController _controller;
        private Coroutine _routine;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        public void Knockback(Vector3 direction, float distance)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f) return;

            if (_routine != null)
            {
                StopCoroutine(_routine);
            }
            _routine = StartCoroutine(KnockbackRoutine(direction.normalized * distance));
        }

        private IEnumerator KnockbackRoutine(Vector3 totalMovement)
        {
            float elapsed = 0f;
            while (elapsed < _duration)
            {
                float dt = Mathf.Min(Time.deltaTime, _duration - elapsed);
                _controller.Move(totalMovement * (dt / _duration));
                elapsed += dt;
                yield return null;
            }
            _routine = null;
        }
    }
}