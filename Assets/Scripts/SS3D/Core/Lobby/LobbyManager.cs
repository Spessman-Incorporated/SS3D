using System;
using System.Collections.Generic;
using Coimbra;
using Mirror;
using SS3D.Core.PlayerControl;
using UnityEngine;

namespace SS3D.Core.Lobby
{
    /// <summary>
    /// Manages all networked lobby stuff
    /// </summary>
    public sealed class LobbyManager : NetworkBehaviour
    {
        /// <summary>
        /// Current lobby players
        /// </summary>
        private readonly SyncList<string> _players = new SyncList<string>();

        public struct PlayerJoinedLobby
        {
            public readonly string Username;

            public PlayerJoinedLobby(string username) { Username = username; }
        }

        public struct PlayerDisconnectedFromLobby
        {
            public readonly string Username;

            public PlayerDisconnectedFromLobby(string username) { Username = username; }
        }

        private void Awake()
        {
            SubscribeToEvents();
        }

        private void Start()
        {
            SyncLobbyPlayers();
        }

        private void SubscribeToEvents()
        {
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            
            eventService?.AddListener<PlayerControlManager.PlayerJoinedServer>(CmdInvokePlayerJoinedLobby);
            eventService?.AddListener<PlayerControlManager.PlayerLeftServer>(CmdInvokePlayerLeftLobby);
        }

        /// <summary>
        /// Updates the lobby players on Start
        /// </summary>
        public void SyncLobbyPlayers()
        {
            foreach (string player in _players)
            {
                IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
                PlayerJoinedLobby playerJoinedLobby = new PlayerJoinedLobby(player);
                eventService!.Invoke(null, playerJoinedLobby);
            }
        }
        
        /// <summary>
        /// A network command that invokes the PlayerJoinedLobby event on the host and client
        /// </summary>
        /// <param name="username"></param>
        [Command(requiresAuthority = false)]
        public void CmdInvokePlayerJoinedLobby(object sender, PlayerControlManager.PlayerJoinedServer playerJoinedServer)
        {
            Debug.Log("Cmd player joined lobby");
            string username = playerJoinedServer.Soul.Ckey;
            
            PlayerJoinedLobby playerJoinedLobby = new PlayerJoinedLobby(username);
            _players.Add(username);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService!.Invoke(null, playerJoinedLobby);
            RpcInvokePlayerJoinedLobby(username);
        }

        /// <summary>
        /// RPC call that calls the PlayerJoinedLobby event
        /// </summary>
        [ClientRpc]
        public void RpcInvokePlayerJoinedLobby(string username)
        { 
            if (isServer) return;
            Debug.Log("Rpc player joined lobby");
            
            PlayerJoinedLobby playerJoinedLobby = new PlayerJoinedLobby(username);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService!.Invoke(null, playerJoinedLobby);
        }

        /// <summary>
        /// Called when a player disconnects from the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="playerLeftServer"></param>
        [Command(requiresAuthority = false)]
        public void CmdInvokePlayerLeftLobby(object sender, PlayerControlManager.PlayerLeftServer playerLeftServer)
        {
            Debug.Log("Cmd player left lobby");
            string username = playerLeftServer.Soul.Ckey;
            
            PlayerDisconnectedFromLobby playerDisconnectedFromLobby = new PlayerDisconnectedFromLobby(username);
            _players.Remove(username);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService!.Invoke(null, playerDisconnectedFromLobby);
            RpcInvokePlayerLeftLobby(username);
        }


        /// <summary>
        /// RPC call when the player leaves the lobby
        /// </summary>
        /// <param name="username"></param>
        [ClientRpc]
        public void RpcInvokePlayerLeftLobby(string username)
        {
            Debug.Log("Rpc player left lobby");
            if (isServer) return;
            
            PlayerDisconnectedFromLobby playerDisconnectedFromLobby = new PlayerDisconnectedFromLobby(username);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService!.Invoke(null, playerDisconnectedFromLobby);
        }
    }
}