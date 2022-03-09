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
            CmdSyncLobbyPlayers();
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


        [Command(requiresAuthority = false)]
        public void CmdSyncLobbyPlayers()
        {
            foreach (string player in _players)
            {
                RpcSyncLobbyPlayer(player);
            }
        }

        [ClientRpc]
        public void RpcSyncLobbyPlayer(string player)
        {
            if (isServer) return; 
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            PlayerJoinedLobby playerJoinedLobby = new PlayerJoinedLobby(player);
            eventService!.Invoke(null, playerJoinedLobby);    
        }
        
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

        [ClientRpc]
        public void RpcInvokePlayerJoinedLobby(string username)
        { 
            if (isServer) return;
            
            PlayerJoinedLobby playerJoinedLobby = new PlayerJoinedLobby(username);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            Debug.Log("RPC playerJoinedLobby " + playerJoinedLobby.username);
            eventService!.Invoke(null, playerJoinedLobby);
        }

        [Client]
        public void InvokePlayerJoinedLobby(string username)
        {
            CmdInvokePlayerJoinedLobby(username);
        }
    }

    public interface ILobbyService
    {
        void InvokePlayerJoinedLobby(string username);

    }
    
}