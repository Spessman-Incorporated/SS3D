using System;
using SS3D.Core.EventChannel;
using SS3D.Core.Networking;
using SS3D.Core.SceneManagement;
using UnityEngine;

namespace SS3D.Core
{
    /// <summary>
    /// Responsible for controlling the game state, persistent throughout the instance
    /// </summary>
    public class ApplicationStateManager : MonoBehaviour
    {
        [SerializeField] private Scenes scenes;
        [SerializeField] private EventChannels eventChannels;

        public EventChannels EventChannels => eventChannels;

        private void Start()
        {
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
        }
    }
}
