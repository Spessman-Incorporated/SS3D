using System.Collections.Generic;
using Coimbra;
using Mirror;
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
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            // instantiating a "Player" prefab gives it the name "Player(clone)"
            // => appending the connectionId is WAY more useful for debugging!
            player.name = "Waiting for validation: " + conn.connectionId;
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        public override void OnClientConnect()
        {
            // OnClientConnect by default calls AddPlayer but it should not do
            // that when we have online/offline scenes. so we need the
            // clientLoadedScene flag to prevent it.
            if (!clientLoadedScene)
            {
                // Ready/AddPlayer is usually triggered by a scene load completing.
                // if no scene was loaded, then Ready/AddPlayer it here instead.
                if (!NetworkClient.ready)
                    NetworkClient.Ready();

                if (autoCreatePlayer)
                    NetworkClient.AddPlayer();
            }
        }

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