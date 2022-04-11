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

            NetworkServer.RegisterHandler<UserAuthorizationMessage>(HandleAuthorizePlayer);
        }

        /// <summary>
        /// Used by the server when a player closes the game
        /// </summary>
        /// <param name="soul"></param>
        [Command(requiresAuthority = false)]
        public void CmdInvokePlayerLeftServer(Soul soul)
        {
            PlayerLeftServer playerLeftServer = new PlayerLeftServer(soul);

            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService?.Invoke(this, playerLeftServer);

            Debug.Log($"[{typeof(PlayerControlManager)}] - CMD - Player left server: {soul.Ckey}");
        }
        
        /// <summary>
        /// Used by the server when a player successfully authenticates on the game server
        /// </summary>
        /// <param name="soul"></param>
        [Command(requiresAuthority = false)]
        public void CmdInvokePlayerJoinedServer(Soul soul)
        {
            PlayerJoinedServer playerJoinedServer = new PlayerJoinedServer(soul);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService?.Invoke(this, playerJoinedServer);

            Debug.Log($"[{typeof(PlayerControlManager)}] - CMD - Player joined server: {soul.Ckey}");
        }

        /// <summary>
        /// Used by the server to validate credentials and reassign souls to clients.
        /// TODO: Actual authentication
        /// </summary>
        /// <param name="userAuthorizationMessage">struct containing the ckey and the connection that sent it</param>
        [Server]
        public void HandleAuthorizePlayer(NetworkConnection conn, UserAuthorizationMessage userAuthorizationMessage)
        {
            string ckey = userAuthorizationMessage.Ckey;

            Soul match = null;
            foreach (Soul soul in _serverSouls.Where((soul) => soul.Ckey == ckey))
            {
                match = soul;
                Debug.Log($"[{typeof(PlayerControlManager)}] - SERVER - Soul match for {soul} found, reassigning to client");
            }

            if (match == null)
            {
                Debug.Log($"[{typeof(PlayerControlManager)}] - SERVER - No Soul match for {ckey} found, creating a new one");

                match = Instantiate(_soulPrefab).GetComponent<Soul>();
                match.SetCkey(string.Empty ,ckey);
                _serverSouls.Add(match);

                NetworkServer.Spawn(match.gameObject);
            }

            // assign authority so the player can own it
            NetworkServer.AddPlayerForConnection(conn, match.gameObject);
            InvokePlayerJoinedServer(match);

            Debug.Log($"[{typeof(PlayerControlManager)}] - SERVER - Handle Authorize Player: {match.Ckey}");
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
        void InvokePlayerJoinedServer(Soul soul);
        void InvokePlayerLeftServer(Soul soul);
    }
}