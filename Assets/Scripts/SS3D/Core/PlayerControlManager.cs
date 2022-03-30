using System;
using Coimbra;
using Mirror;
using SS3D.Core.Lobby;
using SS3D.Systems.Entities;
using UnityEngine;

namespace SS3D.Core
{
    public class PlayerControlManager : NetworkBehaviour, IPlayerControlManagerService
    {
        [Serializable]
        public struct UpdateCkeyRequested
        {
            public Soul soul;
            public string ckey;

            public UpdateCkeyRequested(Soul soul, string newCkey) { this.soul = soul; this.ckey = newCkey; }
        }

        [Serializable]
        public struct PlayerJoinedServer
        {
            public Soul soul;
            
            public PlayerJoinedServer(Soul soul) { this.soul = soul; }
        }

        [Serializable]
        public struct PlayerLeftServer
        {
            public Soul soul;

            public PlayerLeftServer(Soul soul) { this.soul = soul; }
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
            Soul soul = updateCkeyRequested.soul;
            soul.CmdUpdateCkey(updateCkeyRequested.ckey);
            InvokePlayerJoinedServer(soul);
        }

        [Command(requiresAuthority = false)]
        public void CmdInvokePlayerLeftServer(Soul soul)
        {
            Debug.Log("player control left server");
            PlayerLeftServer playerLeftServer = new PlayerLeftServer(soul);

            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService?.Invoke(this, playerLeftServer);
        }
        
        [Command(requiresAuthority = false)]
        public void CmdInvokePlayerJoinedServer(Soul soul)
        {
            Debug.Log("player control joined server");
            PlayerJoinedServer playerJoinedServer = new PlayerJoinedServer(soul);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService?.Invoke(this, playerJoinedServer);
        }
        
        public void InvokeUpdateCkeyRequested(Soul soul, string newCkey)
        {
            UpdateCkeyRequested updateCkeyRequested = new UpdateCkeyRequested(soul, newCkey);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService?.Invoke(this, updateCkeyRequested);
        }

        public void InvokePlayerJoinedServer(Soul soul)
        {
            CmdInvokePlayerJoinedServer(soul);
        }
        
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