using System;
using UnityEngine;

namespace SS3D.Core.EventChannel
{
    [CreateAssetMenu(fileName = "SessionNetworkHelperEventChannel", menuName = "EventChannels/SessionNetworkHelper", order = 0)]
    public class SessionNetworkHelperEventChannel : ScriptableObject
    {
        public event Action SessionInitiationRequested;

        public void InvokeSessionInitiationRequested()
        {
            SessionInitiationRequested?.Invoke();
        }
    }
}