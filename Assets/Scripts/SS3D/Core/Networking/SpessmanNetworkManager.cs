using System.Collections.Generic;
using Coimbra;
using Mirror;
using SS3D.Core.Networking.Helper;
using SS3D.Core.PlayerControl;
using SS3D.Core.Systems.Entities;
using UnityEngine;

namespace SS3D.Core.Networking
{
    /// <summary>
    /// A custom Network Manager to guarantee Mirror won't fuck our game with their base functions
    /// The changes should be minimal in relation to Mirror's
    /// </summary>
    public sealed class SpessmanNetworkManager : NetworkManager
    {
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            NetworkIdentity[] ownedObjects = new NetworkIdentity[conn.clientOwnedObjects.Count];
            conn.clientOwnedObjects.CopyTo(ownedObjects);
            foreach (var networkIdentity in ownedObjects)
            {
                Soul soul = networkIdentity.GetComponent<Soul>();
                if (soul == null) 
                    continue;

                ServiceLocator.Shared.Get<IPlayerControlManagerService>()?.InvokePlayerLeftServer(soul);
                Debug.Log("Invoking the player server left event");
            }
        }
    }
}