using System;
using System.Collections.Generic;
using System.Linq;
using Coimbra;
using Mirror;
using SS3D.Core.Networking.UI_Helper;
using SS3D.Core.Networking.Utils;
using UnityEngine;
using UriParser = SS3D.Utils.UriParser;

namespace SS3D.Core.Networking.Helper
{
    /// <summary>
    /// Helps the NetworkManager to understand what we should do in this instance,
    /// if we are a server, or a client, and process respective data.
    /// </summary>
    public sealed class SessionNetworkHelper : MonoBehaviour
    {
        [SerializeField] private ApplicationStateManager applicationStateManager; 
        private NetworkManager _networkManager;
        
        /// <summary>
        /// The command line arguments read by the GetCommandLineArgs
        /// </summary>
        private List<string> _commandLineArgs;
        
        private bool _isHost;
        private string _ip;
        private string _ckey;
        
        private void Awake()
        {
            Setup();
        }
        
        /// <summary>
        /// Generic method that prepares the class on Start or Awake
        /// </summary>
        private void Setup()
        {
            // Uses the event service to listen to lobby events
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService?.AddListener<ServerConnectionUIHelper.RetryButtonClicked>(InitiateNetworkSession);
            
            _networkManager = NetworkManager.Singleton;
        }

        /// <summary>
        /// Gets the command line arguments from the executable, for example: "-server=localhost"
        /// </summary>
        private void GetCommandLineArgs()
        {
            try
            {
                _commandLineArgs = Environment.GetCommandLineArgs().ToList();
            }
            catch (Exception e)
            {
                Debug.LogError($"[{typeof(SessionNetworkHelper)}] - GetCommandLineArgs: {e}");
                throw;
            }
        }

        /// <summary>
        /// Uses the args to determine if we have to connect or host, etc
        /// </summary>
        private void ProcessCommandLineArgs()
        {
            if (Application.isEditor)
            {
                _isHost = !applicationStateManager.TestingClientInEditor;
                _ip = "localhost";
                _ckey = "editorUser";
                Debug.Log($"[{typeof(SessionNetworkHelper)}] - Testing application on the editor as {_ckey}");
            }
            else
            {
                GetCommandLineArgs();
                foreach (string arg in _commandLineArgs)
                {
                    if (arg.Contains(CommandLineArgs.Host))
                    {
                        _isHost = true;
                        Debug.Log($"[{typeof(SessionNetworkHelper)}] - Command args - {CommandLineArgs.Host} - is true");
                    }
                    
                    if (arg.Contains(CommandLineArgs.Ip))
                    {
                        _ip = arg.Replace(CommandLineArgs.Ip, "");
                        Debug.Log($"[{typeof(SessionNetworkHelper)}] - Command args - {CommandLineArgs.Ip} - {_ip}");
                    }

                    if (arg.Contains(CommandLineArgs.Ckey))
                    {
                        _ckey = arg.Replace(CommandLineArgs.Ckey, "");
                        Debug.Log($"[{typeof(SessionNetworkHelper)}] - Command args - {CommandLineArgs.Ckey} - {_ckey}");                        
                    }

                    if (arg.Contains(CommandLineArgs.SkipIntro))
                    {
                        ApplicationStateManager.Instance.SetSkipIntro(true);
                        Debug.Log($"[{typeof(SessionNetworkHelper)}] - Command args - {CommandLineArgs.SkipIntro} - {true}");
                    }
                }

                Debug.Log($"[{typeof(SessionNetworkHelper)}] - Testing application on executable");
            }

            LocalPlayerAccountManager.UpdateCkey(_ckey);
        }

        /// <summary>
        /// Uses the processed args to proceed with game network initialization
        /// </summary>
        public void InitiateNetworkSession()
        {
            ProcessCommandLineArgs();

            if (_networkManager == null)
                _networkManager = NetworkManager.Singleton;

            if (_isHost)
            {
                Debug.Log($"[{typeof(SessionNetworkHelper)}] - Hosting a new server");
                _networkManager.StartHost();
            }

            else
            {
                Debug.Log($"[{typeof(SessionNetworkHelper)}] - Joining to server {_ip} as {_ckey}");
                _networkManager.StartClient(UriParser.TryParseIpAddress(_ip));
            }
        }
        
        /// <summary>
        /// Overload to match the event type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitiateNetworkSession(object sender, ServerConnectionUIHelper.RetryButtonClicked e)
        {
            InitiateNetworkSession();
        }
    }
}