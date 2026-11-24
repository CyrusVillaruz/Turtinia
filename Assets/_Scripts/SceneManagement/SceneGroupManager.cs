using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Turtinia.SceneManagement
{
    public class SceneGroupManager
    {
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };

        private SceneGroup _activeSceneGroup;

        public async Task LoadScenes(SceneGroup sceneGroup, IProgress<float> progress,
            bool shouldReloadDupScenes = false)
        {
            _activeSceneGroup = sceneGroup;
            List<string> loadedScenes = new List<string>();

            await UnloadScenes();

            int sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            int totalScenesToLoad = _activeSceneGroup.Scenes.Count;
            AsyncOperationGroup operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            for (int i = 0; i < totalScenesToLoad; i++)
            {
                SceneData sceneData = sceneGroup.Scenes[i];
                if (shouldReloadDupScenes == false && loadedScenes.Contains(sceneData.SceneName)) continue;

                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                operationGroup.Operations.Add(operation);
                OnSceneLoaded.Invoke(sceneData.SceneName);
            }
            
            // Wait until all AsyncOperations in the group are done.
            while (!operationGroup.IsDone)
            {
                progress?.Report(operationGroup.Progress);
                await Task.Delay(100); // Prevent report going off too frequently.
            }

            Scene activeScene =
                SceneManager.GetSceneByName(_activeSceneGroup.FindSceneNameByType(SceneType.ActiveScene));

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }
            
            OnSceneGroupLoaded.Invoke();
        }

        public async Task UnloadScenes()
        {
            List<string> scenes = new List<string>();
            string activeScene = SceneManager.GetActiveScene().name;

            int sceneCount = SceneManager.sceneCount;
            for (int i = sceneCount - 1; i > 0; i--)
            {
                Scene sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;
                
                string sceneName  = sceneAt.name;
                
                // Do not unload persistent scenes such as the Bootstrapper.
                if (sceneName.Equals(activeScene) || sceneName == "Bootstrapper") continue;
            }
        }
    }
}