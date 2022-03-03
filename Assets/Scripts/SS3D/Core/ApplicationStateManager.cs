using System;
using SS3D.Core.EventChannel;
using SS3D.Core.Networking;
using SS3D.Core.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SS3D.Core
{
    /// <summary>
    /// Responsible for controlling the game state, persistent throughout the instance
    /// </summary>
    public class ApplicationStateManager : MonoBehaviour
    {
        public static ApplicationStateManager Instance;
        
        [Header("Scenes")]
        [SerializeField] private Scenes scenes;
        
        [Header("Managers")]
        [SerializeField] private EventChannels eventChannels;
        [SerializeField] private SessionNetworkHelper networkHelper;

        [Header("Test Cases")]
        [SerializeField] private bool testingClientInEditor;

        public bool TestingClientInEditor => testingClientInEditor;
        
        public EventChannels EventChannels => eventChannels;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            networkHelper.ProcessCommandLineArgs();
        }
    }
}
