using System;
using System.Collections.Generic;
using Coimbra;
using Mirror;
using SS3D.Core.Lobby.UI;
using UnityEngine;
using EventService = Coimbra.Services.EventService;

namespace SS3D.Core.Lobby
{
    public class LobbyManager : NetworkBehaviour, ILobbyService
    {
        private readonly SyncList<string> _players = new SyncList<string>();

        private void Awake()
        { 
            ServiceLocator.Shared.Set<ILobbyService>(this);
        }

        private void Start()
        {
            SyncLobbyPlayers();
        }

        public struct PlayerJoinedLobby
        {
            public string username;

            public PlayerJoinedLobby(string username)
            {
                this.username = username;
            }
        }

        public struct PlayerDisconnectedFromLobby
        {
            public string username;

            public PlayerDisconnectedFromLobby(string username)
            {
                this.username = username;
            }
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
        public void CmdInvokePlayerJoinedLobby(string username)
        {
            PlayerJoinedLobby playerJoinedLobby = new PlayerJoinedLobby(username);
            _players.Add(username);
            
            Debug.Log("CMD playerJoinedLobby " + username);
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
            
            PlayerJoinedLobby playerJoinedLobby = new PlayerJoinedLobby(username);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            Debug.Log("RPC playerJoinedLobby " + playerJoinedLobby.username);
            eventService!.Invoke(null, playerJoinedLobby);
        }

        /// <summary>
        /// Interface-implemented event to be able to call this from anywhere
        /// </summary>
        [Client]
        public void InvokePlayerJoinedServer(string username)
        {
            CmdInvokePlayerJoinedLobby(username);
        }
    }

    /// <summary>
    /// Interface used to work with the ServiceLocator
    /// </summary>
    public interface ILobbyService
    {
        void InvokePlayerJoinedServer(string username);
    }
}