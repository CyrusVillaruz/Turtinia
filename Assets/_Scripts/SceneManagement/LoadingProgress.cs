using System;

namespace Turtinia.SceneManagement
{
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> OnProgressed;
        private const float _ratio = 1.0f;

        public void Report(float value)
        {
            OnProgressed?.Invoke(value / _ratio);
        }
    }
}