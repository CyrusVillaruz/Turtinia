using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Turtinia.SceneManagement
{
    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;
        
        public float Progress => Operations.Count == 0 ? 0f : Operations.Average(o => o.progress);
        public bool IsDone => Operations.All(o => o.isDone);
        
        public AsyncOperationGroup(int initialCapacity)
        {
            Operations = new List<AsyncOperation>(initialCapacity);
        }
    }
}