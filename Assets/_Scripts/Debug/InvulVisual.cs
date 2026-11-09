using System.Collections;
using UnityEngine;

namespace Turtinia.Debug
{
    public class InvulVisual : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Material _invulnerableMaterial;

        private Material _originalMaterial;
        private Coroutine _routine;

        private void Awake()
        {
            _originalMaterial = _renderer.sharedMaterial;
        }

        public void Play(float duration)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
            }
            _routine = StartCoroutine(SwapRoutine(duration));
        }

        private IEnumerator SwapRoutine(float duration)
        {
            _renderer.sharedMaterial = _invulnerableMaterial;
            yield return new WaitForSeconds(duration);
            _renderer.sharedMaterial = _originalMaterial;
            _routine = null;
        }

        private void OnDisable()
        {
            _renderer.sharedMaterial = _originalMaterial;
        }
    }
}