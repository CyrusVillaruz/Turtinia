using Turtinia.Interfaces;
using Turtinia.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Turtinia.Level
{
    public class MagicCircle : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _sceneName;
        [SerializeField] private GameObject _textPrompt;

        private PlayerInteractor _interactor;
        private UnityEngine.Camera _camera;
        private bool _loading;

        private string TargetScene => string.IsNullOrEmpty(_sceneName) ? gameObject.name : _sceneName;

        private void Awake()
        {
            _camera = UnityEngine.Camera.main;
            _textPrompt.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerInteractor interactor = other.GetComponent<PlayerInteractor>();
            if (!interactor) return;

            _interactor = interactor;
            _interactor.SetCurrent(this);
            _textPrompt.SetActive(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_interactor && other.GetComponent<PlayerInteractor>() == _interactor)
            {
                Deselect();
            }
        }

        private void OnDisable() => Deselect();

        public void Interact(GameObject interactor)
        {
            if (_loading) return;

            // TODO: Load scene asynchronously to later add a Loading screen and fade-in/out effects.
            string scene = TargetScene;
            _loading = true;
            SceneManager.LoadScene(scene);
        }

        private void LateUpdate()
        {
            if (!_interactor || !_camera) return;
            _textPrompt.transform.rotation = _camera.transform.rotation;
        }

        private void Deselect()
        {
            if (_interactor)
            {
                _interactor.ClearCurrent(this);
            }
            _interactor = null;
            if (_textPrompt)
            {
                _textPrompt.SetActive(false);
            }
        }
    }
}