using System;
using System.Collections.Generic;
using System.Linq;
using Coimbra;
using Mirror;
using SS3D.Core.Networking;
using SS3D.Core.Systems.Entities;
using UnityEngine;

namespace SS3D.Core.PlayerControl
{
    /// <summary>
    /// Controls the player flux, when users want to authenticate, rejoin the game, leave the game
    /// </summary>
    public sealed class PlayerControlManager : NetworkBehaviour, IPlayerControlManagerService
    {
        [SerializeField] private GameObject _soulPrefab;

        private SyncList<Soul> _serverSouls = new SyncList<Soul>();

        [Serializable]
        public struct AuthorizationRequested
        {
            public string Ckey;
            public NetworkConnectionToClient NetworkConnectionToClient;

            public AuthorizationRequested(string ckey, NetworkConnectionToClient networkConnectionToClient) { this.Ckey = ckey; this.NetworkConnectionToClient = networkConnectionToClient; }
        }

        [Serializable]
        public struct PlayerJoinedServer
        {
            public Soul Soul;
            
            public PlayerJoinedServer(Soul soul) { Soul = soul; }
        }

        [Serializable]
        public struct PlayerLeftServer
        {
            public Soul Soul;

            public PlayerLeftServer(Soul soul) { Soul = soul; }
        }

        private void Awake()
        { 
            ServiceLocator.Shared.Set<IPlayerControlManagerService>(this);
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            
            eventService?.AddListener<AuthorizationRequested>(HandleAuthorizePlayer);
        }

        /// <summary>
        /// Used by the server when a player closes the game
        /// </summary>
        /// <param name="soul"></param>
        [Command(requiresAuthority = false)]
        public void CmdInvokePlayerLeftServer(Soul soul)
        {
            Debug.Log("player control left server");
            PlayerLeftServer playerLeftServer = new PlayerLeftServer(soul);

            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService?.Invoke(this, playerLeftServer);
        }
        
        /// <summary>
        /// Used by the server when a player successfully authenticates on the game server
        /// </summary>
        /// <param name="soul"></param>
        [Command(requiresAuthority = false)]
        public void CmdInvokePlayerJoinedServer(Soul soul)
        {
            Debug.Log("player control joined server");
            PlayerJoinedServer playerJoinedServer = new PlayerJoinedServer(soul);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService?.Invoke(this, playerJoinedServer);
        }

        /// <summary>
        /// Tries to connect back to the existing Soul or create a new one
        /// </summary>
        [Command(requiresAuthority = false)]
        private void CmdInvokeAuthorizationRequested(string ckey, NetworkConnectionToClient sender = null)
        {
            Debug.Log("CmdInvokeAuthorizationRequested");

            // sends the Ckey to validate and assign a Soul to this
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            AuthorizationRequested authorizationRequested = new AuthorizationRequested(ckey, sender);
            eventService!.Invoke(this, authorizationRequested);
        }

        /// <summary>
        /// Used by the server to validate credentials and reassign souls to clients
        /// </summary>
        /// <param name="authorizationRequested">struct containing the ckey and the connection that sent it</param>
        [Server]
        public void HandleAuthorizePlayer(object sender, AuthorizationRequested authorizationRequested)
        {
            Debug.Log("Handle Authorize Player " + authorizationRequested.Ckey);

            string ckey = authorizationRequested.Ckey;

            Soul match = null;
            foreach (Soul soul in _serverSouls.Where((soul) => soul.Ckey == ckey))
            {
                match = soul;
            }

            if (match == null)
            {
                Debug.Log("No Soul match found, creating a new one");

                match = Instantiate(_soulPrefab).GetComponent<Soul>();
                match.SetCkey(string.Empty ,ckey);
                _serverSouls.Add(match);

                NetworkServer.Spawn(match.gameObject);
            }

            // assign authority so the player can own it
            match.netIdentity.AssignClientAuthority(authorizationRequested.NetworkConnectionToClient);
            InvokePlayerJoinedServer(match);
        }

        /// <summary>
        /// Tries to connect back to the existing Soul or create a new one
        /// </summary>
        public void InvokeAuthorizationRequested(string ckey, NetworkConnectionToClient sender)
        {
            CmdInvokeAuthorizationRequested(ckey, sender);
        }

        /// <summary>
        /// Interface implementation, called when we use the ServiceLocator
        /// </summary>
        /// <param name="soul"></param>
        /// <param name="newCkey"></param>
        public void InvokePlayerJoinedServer(Soul soul)
        {
            CmdInvokePlayerJoinedServer(soul);
        }
        
        /// <summary>
        /// Interface implementation, called when we use the ServiceLocator
        /// </summary>
        /// <param name="soul"></param>
        /// <param name="newCkey"></param>
        public void InvokePlayerLeftServer(Soul soul)
        {
            CmdInvokePlayerLeftServer(soul);
        }


    }

    /// <summary>
    /// Interface used by the ServiceLocator, so we can use these functions everywhere
    /// </summary>
    public interface IPlayerControlManagerService
    {
        void InvokeAuthorizationRequested(string ckey, NetworkConnectionToClient networkConnectionToClient);

        void InvokePlayerJoinedServer(Soul soul);
        void InvokePlayerLeftServer(Soul soul);
    }
}