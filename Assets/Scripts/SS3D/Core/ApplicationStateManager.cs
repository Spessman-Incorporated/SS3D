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
    /// Should hopefully be the only singleton in the project
    /// </summary>
    public class ApplicationStateManager : MonoBehaviour
    {
        public static ApplicationStateManager Instance;
        
        [Header("Scenes")]
        [SerializeField] private Scenes scenes;
        
        [Header("Managers")]
        [SerializeField] private SessionNetworkHelper networkHelper;

        [Header("Test Cases")]
        [SerializeField] private bool testingClientInEditor;

        public bool TestingClientInEditor => testingClientInEditor;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            DOTween.Init();
            networkHelper.ProcessCommandLineArgs();
        }
    }
}
