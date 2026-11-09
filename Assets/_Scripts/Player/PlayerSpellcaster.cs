using UnityEngine;
using UnityEngine.InputSystem;

namespace Turtinia.Player
{
    public class PlayerSpellcaster : MonoBehaviour
    {
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // TODO: Cast projectile (e.g. instantiate ball). Should be able to press repeatedly or hold down LMB
                UnityEngine.Debug.Log("Cast Basic Spell");
            }
        }
    }
}