using System;
using Mirror;
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
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            CmdSyncCkey();
        }

        [Command(requiresAuthority = false)]
        public void CmdSyncCkey(NetworkConnectionToClient sender = null)
        {
            SetCkey(_ckey, _ckey);
            gameObject.name = "Soul: " + _ckey;
        }
        
        /// <summary>
        /// Uses a network command to update the ckey
        /// </summary>
        [Command(requiresAuthority = false)]
        public void CmdUpdateCkey(string ckey, NetworkConnectionToClient sender = null)
        {
            _ckey = ckey;
            Debug.Log("CmdUpdateCkey: " +_ckey);
            gameObject.name = "Soul: " + ckey;
            RpcUpdateKey(gameObject.name);
        }

        [ClientRpc]
        public void RpcUpdateKey(string gameObjectName)
        {
            gameObject.name = gameObjectName;
        }
        

        /// <summary>
        /// Used by Mirror Networking to update the variable and sync it across instances.
        /// </summary>
        private void SetCkey(string oldCkey, string newCkey)
        {
            _ckey = newCkey;
            Debug.Log("UpdateCkey: " + newCkey);
        }
    }
}
