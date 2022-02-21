using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UriParser = SS3D.Utils.UriParser;

namespace SS3D.Core.Networking
{
    /// <summary>
    /// Helps the NetworkManager to understand what we should do in this instance,
    /// if we are a server, or a client, and process respective data.
    /// </summary>
    public class SessionNetworkHelper : MonoBehaviour
    {
        [SerializeField] private ApplicationStateManager applicationStateManager; 
        private NetworkManager _networkManager;
        private List<string> _commandLineArgs;

        private bool _isHost;
        /// <summary>
        /// SS3D server IP address
        /// </summary>
        private string _ip;
        private string _username;
        
        private void Awake()
        {
            Setup();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            applicationStateManager.EventChannels.ApplicationState.SetupSceneLoaded += ProcessCommandLineArgs;
        }

        private void UnsubscribeFromEvents()
        {
            applicationStateManager.EventChannels.ApplicationState.SetupSceneLoaded -= ProcessCommandLineArgs;
        }

        private void Setup()
        {
            _networkManager = NetworkManager.singleton;

            if (!Application.isEditor)
            {
                GetCommandLineArgs();
            }
            else
            {
                _isHost = true;
                _username = "editorUser";
            }
            
            Debug.Log("SessionNetworkVariable: " + "_isHost " + _isHost + " Username: " + _username);
        }

        /// <summary>
        /// Gets the command line arguments from the executable, for example: "-server-_ip localhost"
        /// </summary>
        private void GetCommandLineArgs()
        {
            _commandLineArgs = System.Environment.GetCommandLineArgs().ToList();
        }

        /// <summary>
        /// Uses the args to determine if we have to connect or host, etc
        /// </summary>
        public void ProcessCommandLineArgs()
        {
            try
            {
                foreach (string arg in _commandLineArgs)
                {
                    switch (arg)
                    {
                        case CommandLineArgs.Host:
                            _isHost = (_commandLineArgs[_commandLineArgs.IndexOf(arg) + 1][0] == '1');
                            break;
                        case CommandLineArgs.Ip:
                            _ip = _commandLineArgs[_commandLineArgs.IndexOf(arg) + 1];
                            break;
                        case CommandLineArgs.Username:
                            _username = _commandLineArgs[_commandLineArgs.IndexOf(arg) + 1];
                            break;
                        default:
                            break;
                    }
                }
                
                InitiateNetworkSession();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Uses the processed args to proceed with game initialization
        /// </summary>
        private void InitiateNetworkSession()
        {
            if (_isHost)
            {
                _networkManager.StartHost();
            }

            if (_ip != string.Empty)
            {
                _networkManager.StartClient(UriParser.TryParseIpAddress(_ip));
            }
        }
    }
}