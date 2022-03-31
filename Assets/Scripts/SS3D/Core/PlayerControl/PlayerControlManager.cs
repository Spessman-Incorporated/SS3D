using System;
using Coimbra;
using Mirror;
using SS3D.Core.Systems.Entities;
using UnityEngine;

namespace SS3D.Core.PlayerControl
{
    /// <summary>
    /// Controls the player flux, when users want to authenticate, rejoin the game, leave the game
    /// </summary>
    public sealed class PlayerControlManager : NetworkBehaviour, IPlayerControlManagerService
    {
        [Serializable]
        public struct AuthorizationRequested
        {
            public string Ckey;
            public NetworkConnectionToClient NetworkConnectionToClient;

            public AuthorizationRequested(string ckey, NetworkConnectionToClient networkConnectionToClient) { this.Ckey = ckey; this.NetworkConnectionToClient = networkConnectionToClient; }
        }

        [Serializable]
        public struct UpdateCkeyRequested
        {
            public Soul Soul;
            public string Ckey;

            public UpdateCkeyRequested(Soul soul, string newCkey) { this.Soul = soul; this.Ckey = newCkey; }
        }

        [Serializable]
        public struct PlayerJoinedServer
        {
            public Soul Soul;
            
            public PlayerJoinedServer(Soul soul) { this.Soul = soul; }
        }

        [Serializable]
        public struct PlayerLeftServer
        {
            public Soul Soul;

            public PlayerLeftServer(Soul soul) { this.Soul = soul; }
        }

        private void Awake()
        { 
            ServiceLocator.Shared.Set<IPlayerControlManagerService>(this);
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            
            eventService?.AddListener<UpdateCkeyRequested>(OnUpdateCkeyRequested);
        }

        public void OnUpdateCkeyRequested(object sender, UpdateCkeyRequested updateCkeyRequested)
        {
            Soul soul = updateCkeyRequested.Soul;
            soul.CmdUpdateCkey(updateCkeyRequested.Ckey);
            InvokePlayerJoinedServer(soul);
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
        /// Interface implementation, called when we use the ServiceLocator
        /// </summary>
        /// <param name="soul"></param>
        /// <param name="newCkey"></param>
        public void InvokeUpdateCkeyRequested(Soul soul, string newCkey)
        {
            UpdateCkeyRequested updateCkeyRequested = new UpdateCkeyRequested(soul, newCkey);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService?.Invoke(this, updateCkeyRequested);
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

    public interface IPlayerControlManagerService
    {
        void InvokeUpdateCkeyRequested(Soul soul, string newCkey);
        void InvokePlayerJoinedServer(Soul soul);
        void InvokePlayerLeftServer(Soul soul);
    }
}