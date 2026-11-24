using System;
using Eflatun.SceneReference;

namespace Turtinia.SceneManagement
{
    [Serializable]
    public class SceneData
    {
        public SceneReference Reference;
        public string SceneName => Reference.Name;
        public SceneType SceneType;
    }
}