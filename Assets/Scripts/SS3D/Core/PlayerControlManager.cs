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
            ServiceLocator.Shared.Get<ILobbyService>()!.InvokePlayerJoinedServer(updateCkeyRequested.ckey);
        }
        
        public void InvokeUpdateCkeyRequested(Soul soul, string newCkey)
        {
            UpdateCkeyRequested updateCkeyRequested = new UpdateCkeyRequested(soul, newCkey);
            
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            eventService?.Invoke(this, updateCkeyRequested);
        }
    }

    public interface IPlayerControlManagerService
    {
        void InvokeUpdateCkeyRequested(Soul soul, string newCkey);
    }
}