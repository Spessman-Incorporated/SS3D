using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SS3D.Core.EventChannel.Logic
{
    [CreateAssetMenu(fileName = "SceneLoaderEventChannel", menuName = "EventChannels/SceneLoaderEventChannel", order = 0)]
    public class SceneLoaderEventChannel : ScriptableObject
    {
        public event Action<SceneReference, LoadSceneMode> SceneLoadRequested;
        public event Action<SceneReference> SceneLoadCompleted;
        
        public void InvokeSceneLoadCompleted(SceneReference sceneReference)
        {
            SceneLoadCompleted?.Invoke(sceneReference);
        }
        
        public void InvokeSceneLoadRequested(SceneReference sceneReference, LoadSceneMode loadSceneMode)
        {
            SceneLoadRequested?.Invoke(sceneReference, loadSceneMode);
        }
    }
}