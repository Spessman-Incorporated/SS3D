using System;
using System.Collections.Generic;
using Coimbra;
using Mirror;
using SS3D.Core.Lobby.UI;
using UnityEngine;
using EventService = Coimbra.Services.EventService;

namespace SS3D.Core.Lobby
{
    public class LobbyManager : NetworkBehaviour
    {
        private readonly SyncList<string> _players = new SyncList<string>();

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
        public void CmdInvokePlayerJoinedLobby(object sender, PlayerControlManager.PlayerJoinedServer playerJoinedServer)
        {
            string username = playerJoinedServer.soul.Ckey;
            
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
            
            PlayerJoinedLobby playerJoinedLobby = new PlayerJoinedLobby(username);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService!.Invoke(null, playerJoinedLobby);
        }

        [Command(requiresAuthority = false)]
        public void CmdInvokePlayerLeftLobby(object sender, PlayerControlManager.PlayerLeftServer playerLeftServer)
        {
            string username = playerLeftServer.soul.Ckey;
            
            PlayerDisconnectedFromLobby playerDisconnectedFromLobby = new PlayerDisconnectedFromLobby(username);
            _players.Remove(username);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService!.Invoke(null, playerDisconnectedFromLobby);
            RpcInvokePlayerLeftLobby(username);
        }


        [ClientRpc]
        public void RpcInvokePlayerLeftLobby(string username)
        {
            if (isServer) return;
            
            PlayerDisconnectedFromLobby playerDisconnectedFromLobby = new PlayerDisconnectedFromLobby(username);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService!.Invoke(null, playerDisconnectedFromLobby);
        }
    }
}