using Turtinia.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Turtinia.Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        private IInteractable _currentInteractable;

        public void SetCurrent(IInteractable interactable) => _currentInteractable = interactable;

        public void ClearCurrent(IInteractable interactable)
        {
            if (_currentInteractable == interactable)
            {
                _currentInteractable = null;
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _currentInteractable?.Interact(gameObject);
            }
        }
    }
}