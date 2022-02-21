using Mirror;
using UnityEngine;

namespace SS3D.Systems.Entities
{
    /// <summary>
    /// Unique, persistent object that the player owns, it manages what character it is controlling and stores other player data.
    /// </summary>
    public class Soul : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SetCkey))]
        private string _ckey;

        /// <summary>
        /// Unique client key, originally used in BYOND's user management
        /// </summary>
        public string Ckey => _ckey;

        /// <summary>
        /// Used by Mirror Networking to update the variable and sync it across instances.
        /// </summary>
        private void SetCkey(string oldCkey, string newCkey)
        {
            _ckey = newCkey;
        }
    }
}
