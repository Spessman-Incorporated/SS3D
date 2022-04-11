using System;
using DG.Tweening;
using SS3D.Core.Networking;
using SS3D.Core.Networking.Helper;
using SS3D.Core.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SS3D.Core
{
    /// <summary>
    /// Responsible for controlling the game state, persistent throughout the application
    /// Should hopefully be the only Singleton in the project
    /// </summary>
    public sealed class ApplicationStateManager : MonoBehaviour
    {
        public static ApplicationStateManager Instance;
        
        [Header("Scenes")]
        [SerializeField] private Scenes scenes;
        
        [Header("Managers")]
        [SerializeField] private SessionNetworkHelper networkHelper;

        [Header("Test Cases")]
        [SerializeField] private bool testingClientInEditor;
        [SerializeField] private bool testingServerOnlyInEditor;

        public bool TestingClientInEditor => testingClientInEditor;
        public bool TestingServerOnlyInEditor => testingServerOnlyInEditor;

        private void Awake()
        {
            Setup();
        }

        private void Setup()
        {
            Debug.Log($"[{typeof(ApplicationStateManager)}] - Initializing application");

            InitializeSingleton();
            ProcessTestParams();
            InitializeEssentialSystems();
        }

        private void ProcessTestParams()
        {
            if (!Application.isEditor)
            {
                testingClientInEditor = false;
                testingServerOnlyInEditor = false;
            }
        }
        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            Debug.Log($"[{typeof(ApplicationStateManager)}] - Initializing Application State Manager singleton");
        }

        private void InitializeEssentialSystems()
        {
            DOTween.Init();
            networkHelper.ProcessCommandLineArgs();
            Debug.Log($"[{typeof(ApplicationStateManager)}] - Initializing essential systems");
        }
    }
}
