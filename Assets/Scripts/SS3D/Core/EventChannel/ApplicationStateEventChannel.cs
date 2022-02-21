using System;
using UnityEngine;

namespace SS3D.Core.EventChannel
{
    [CreateAssetMenu(fileName = "ApplicationStateEventChannel", menuName = "EventChannels/ApplicationStateEventChannel", order = 0)]
    public class ApplicationStateEventChannel : ScriptableObject
    {
        public event Action SetupSceneLoaded;

        public void InvokeSetupSceneLoaded()
        {
            SetupSceneLoaded?.Invoke();
        }
    }
}