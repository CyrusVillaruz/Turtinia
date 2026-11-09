using Turtinia.Combat;
using Turtinia.Interfaces;
using UnityEngine;

namespace Turtinia.Debug
{
    public class TestResourceDrain : MonoBehaviour
    {
        [SerializeField] private ResourceType _type;
        [SerializeField] private float _amountPerSecond = 20f;

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out ResourceHub hub) && hub.TryGet(_type, out IResource pool))
            {
                pool.TryConsume(_amountPerSecond * Time.fixedDeltaTime);
            }
        }
    }
}