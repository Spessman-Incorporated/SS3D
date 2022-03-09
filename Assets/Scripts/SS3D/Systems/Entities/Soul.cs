using System;
using Coimbra;
using Mirror;
using SS3D.Core.Lobby;
using SS3D.Core.Networking;
using UnityEngine;

namespace SS3D.Systems.Entities
{
    /// <summary>
    /// Unique, persistent object that the player owns, it manages what character it is controlling and stores other player data.
    /// </summary>
    public class Soul : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SetCkey))] private string _ckey;

        /// <summary>
        /// Unique client key, originally used in BYOND's user management
        /// </summary>
        public string Ckey => _ckey;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            // Sends the command to update the ckey on the server
            CmdUpdateCkey(LocalPlayerAccountManager.Ckey);
            ServiceLocator.Shared.Get<ILobbyService>()!.InvokePlayerJoinedLobby(LocalPlayerAccountManager.Ckey);
        }
        
        /// <summary>
        /// Uses a network command to update the ckey
        /// </summary>
        [Command(requiresAuthority = false)]
        public void CmdUpdateCkey(string ckey, NetworkConnectionToClient sender = null)
        {
            SetCkey(_ckey, ckey);
            gameObject.name = "Soul: " + ckey;
        }
        
        /// <summary>
        /// Used by Mirror Networking to update the variable and sync it across instances.
        /// </summary>
        private void SetCkey(string oldCkey, string newCkey)
        {
            _ckey = newCkey; 
            gameObject.name = "Soul: " + _ckey;
        }
    }
}
