using System;
using System.Collections;
using Coimbra;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SS3D.Core.SceneManagement
{
    /// <summary>
    /// Persistent object that loads and unloads scenes
    /// </summary>
    public class SceneLoaderManager : MonoBehaviour
    { 
        [SerializeField] private ApplicationStateManager applicationStateManager;
        
        public event Action<SceneReference, LoadSceneMode> SceneLoadRequested;
        public event Action<SceneReference> SceneLoadCompleted;

        private void Awake()
        {
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            SceneLoadRequested += LoadScene;
        }
        
        /// <summary>
        /// Tries to load a scene
        /// </summary>
        /// <param name="scene">A SceneReference of a scene</param>
        public void LoadScene(SceneReference scene, LoadSceneMode loadSceneMode)
        {
            TryUnloadScene(scene);
            StartCoroutine(LoadSceneCoroutine(scene, loadSceneMode));
        }

        /// <summary>
        /// Unloads the scene if it is loaded
        /// </summary>
        /// <param name="scene">A SceneReference of a scene</param>
        private static void TryUnloadScene(SceneReference scene)
        {
            if (SceneManager.GetSceneByName(scene).isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }

        /// <summary>
        /// Responsible for loading a scene
        /// </summary>
        /// <param name="scene">A SceneReference of a scene</param>
        /// <returns></returns>
        private IEnumerator LoadSceneCoroutine(SceneReference scene, LoadSceneMode loadSceneMode)
        {
            if (SceneManager.GetSceneByPath(scene.ScenePath).isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
            AsyncOperation operation = (SceneManager.LoadSceneAsync(scene, loadSceneMode));
            
            yield return new WaitUntil( () => operation.isDone);

            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService!.Invoke(SceneLoadCompleted, scene);
            
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scene.ScenePath));
        }
    }
}
