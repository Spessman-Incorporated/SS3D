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
            eventService.AddListener<ServerConnectionUIHelper.RetryButtonClicked>(InitiateNetworkSession);
            
            _networkManager = NetworkManager.Singleton;
        }

        /// <summary>
        /// Gets the command line arguments from the executable, for example: "-server-_ip localhost"
        /// </summary>
        private void GetCommandLineArgs()
        {
            try
            {
                _commandLineArgs = Environment.GetCommandLineArgs().ToList();
            }
            catch (Exception e)
            {
                Debug.LogError("GetCommandLineArgs: " + e);
                throw;
            }
        }

        /// <summary>
        /// Uses the args to determine if we have to connect or host, etc
        /// </summary>
        public void ProcessCommandLineArgs()
        {
            if (Application.isEditor)
            {
                _isHost = !applicationStateManager.TestingClientInEditor;
                _ckey = "editorUser";
            }
            else
            {
                GetCommandLineArgs();
                foreach (string arg in _commandLineArgs)
                {
                    string value = arg;

                    if (value.Contains(CommandLineArgs.Host))
                    {
                        _isHost = true;
                    }
                    
                    if (value.Contains(CommandLineArgs.Ip))
                    {
                        _ip = value.Replace(CommandLineArgs.Ip, "");
                    }

                    if (value.Contains(CommandLineArgs.Ckey))
                    {
                        _ckey = value.Replace(CommandLineArgs.Ckey, "");
                    }
                }
            }

            LocalPlayerAccountManager.UpdateCkey(_ckey);
            InitiateNetworkSession();
        }

        /// <summary>
        /// Uses the processed args to proceed with game network initialization
        /// </summary>
        public void InitiateNetworkSession()
        {
            if (_networkManager == null)
                _networkManager = NetworkManager.Singleton;

            if (_isHost)
            {
                _networkManager.StartHost();
            }

            else
            {
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