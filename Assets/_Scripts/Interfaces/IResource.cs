using Turtinia.Combat;

namespace Turtinia.Interfaces
{
    public interface IResource
    {
        ResourceType Type { get; }
        float Current { get; }
        float Max { get; }
        bool CanAfford(float amount);
        bool TryConsume(float amount);
        void Restore(float amount);
    }
}