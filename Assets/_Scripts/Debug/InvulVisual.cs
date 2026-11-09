using System.Collections;
using Turtinia.Player;
using UnityEngine;

namespace Turtinia.Debug
{
    public class InvulVisual : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Material _invulnerableMaterial;
        [SerializeField] private Material _perfectDodgeMaterial;
        [SerializeField] private float _perfectDodgeDuration = 0.5f;
        
        private PerfectDodge _perfectDodge;
        private Material _originalMaterial;
        private Material _applied;
        private float _invulnerableUntil;
        private float _perfectUntil;

        private void Awake()
        {
            _originalMaterial = _renderer.sharedMaterial;
            _applied = _originalMaterial;
            _perfectDodge = GetComponent<PerfectDodge>();
        }

        private void OnEnable()
        {
            if (_perfectDodge != null)
            {
                _perfectDodge.OnPerfectDodge += PlayOnPerfectDodge;
            }
        }

        private void OnDisable()
        {
            if (_perfectDodge != null)
            {
                _perfectDodge.OnPerfectDodge -= PlayOnPerfectDodge;
            }
            Apply(_originalMaterial);
        }

        public void Play(float duration)
        {
            _invulnerableUntil = Time.time + duration;
        }

        private void PlayOnPerfectDodge()
        {
            _perfectUntil = Time.time + _perfectDodgeDuration;
        }

        private void Update()
        {
            Material target =
                Time.time < _perfectUntil ? _perfectDodgeMaterial :
                Time.time < _invulnerableUntil ? _invulnerableMaterial :
                _originalMaterial;

            Apply(target);
        }

        private void Apply(Material material)
        {
            if (material == _applied) return;
            _renderer.sharedMaterial = material;
            _applied = material;
        }
    }
}