// ResourceHub.cs
using System.Collections.Generic;
using Turtinia.Interfaces;
using UnityEngine;

namespace Turtinia.Combat
{
    [System.Serializable]
    public struct ResourceCost
    {
        public ResourceType Type;
        public float Amount;
    }

    public class ResourceHub : MonoBehaviour
    {
        private readonly Dictionary<ResourceType, IResource> _pools = new();

        private void Awake()
        {
            foreach (IResource pool in GetComponents<IResource>())
            {
                _pools[pool.Type] = pool;
            }
        }

        public bool TryGet(ResourceType type, out IResource pool) => _pools.TryGetValue(type, out pool);
        

        // TODO: Call when casting Mana Blast.
        public bool TryConsume(IReadOnlyList<ResourceCost> costs)
        {
            foreach (ResourceCost cost in costs)
            {
                if (!_pools.TryGetValue(cost.Type, out IResource pool) || !pool.CanAfford(cost.Amount))
                {
                    return false;
                }
            }
            foreach (ResourceCost cost in costs)
            {
                _pools[cost.Type].TryConsume(cost.Amount);
            }
            return true;
        }
    }
}