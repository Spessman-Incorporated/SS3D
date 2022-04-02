using System;
using Coimbra;
using Mirror;
using SS3D.Core.Networking;
using SS3D.Core.PlayerControl;
using UnityEngine;

namespace SS3D.Core.Systems.Entities
{
    /// <summary>
    /// Unique, persistent object that the player owns, it manages what character it is controlling and stores other player data.
    /// </summary>
    [Serializable]
    public sealed class Soul : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SetCkey))] private string _ckey;

        /// <summary>
        /// Unique client key, originally used in BYOND's user management, nostalgically used
        /// </summary>
        public string Ckey => _ckey;

        /// <summary>
        /// Uses a network command to update the Ckey
        /// </summary>
        [Command(requiresAuthority = false)]
        public void CmdUpdateCkey(string ckey, NetworkConnectionToClient sender = null)
        {

            SetCkey(_ckey, ckey);
            gameObject.name = "Soul: " + ckey;
            //RpcUpdateCkey();
        }

        [ClientRpc]
        public void RpcUpdateCkey()
        {
            gameObject.name = "Soul: " + Ckey;
        }
        
        /// <summary>
        /// Used by Mirror Networking to update the variable and sync it across instances.
        /// 
        /// This is also called by the server when the client enters the server to update his data
        /// </summary>
        public void SetCkey(string oldCkey, string newCkey)
        {
            Debug.Log("Updating player ckey");
            _ckey = newCkey; 
            gameObject.name = "Soul: " + _ckey;
        }
    }
}
