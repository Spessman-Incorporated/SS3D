using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using SS3D.Core.Networking.Utils;
using UnityEngine;
using UriParser = SS3D.Utils.UriParser;

namespace SS3D.Core.Networking.Helper
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
        private string _ckey;
        
        private void Awake()
        {
            Setup();
        }
        
        private void Setup()
        {
            _networkManager = NetworkManager.singleton;
        }

        /// <summary>
        /// Gets the command line arguments from the executable, for example: "-server-_ip localhost"
        /// </summary>
        private void GetCommandLineArgs()
        {
            try
            {
                _commandLineArgs = System.Environment.GetCommandLineArgs().ToList();
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
            Debug.Log("SessionNetworkVariable { " + "Is Host: " + _isHost + " Ckey: " + _ckey + "}");
            InitiateNetworkSession();
        }

        /// <summary>
        /// Uses the processed args to proceed with game initialization
        /// </summary>
        private void InitiateNetworkSession()
        { 
            Debug.Log("Initiating network session: hosting=" + _isHost + ", IP Address= " + _ip + ", CKEY: " + _ckey);
            if (_isHost)
            {
                _networkManager.StartHost();
            }

            else
            {
                _networkManager.StartClient(UriParser.TryParseIpAddress(_ip));
            }
        }
    }
}