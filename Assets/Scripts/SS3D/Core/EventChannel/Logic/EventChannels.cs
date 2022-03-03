using SS3D.Core.EventChannel.Logic;
using UnityEngine;

namespace SS3D.Core.EventChannel
{
    [CreateAssetMenu(fileName = "EventChannels", menuName = "EventChannels/EventChannels", order = 0)]
    public class EventChannels : ScriptableObject
    {
        public ApplicationStateEventChannel ApplicationState;
        public SceneLoaderEventChannel SceneLoader;
        public SessionNetworkHelperEventChannel SessionNetworkHelper;
    }
}