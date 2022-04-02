using System;
using Coimbra;
using Mirror;
using SS3D.Core.Networking;
using SS3D.Core.PlayerControl;
using UnityEngine;

namespace SS3D.Core.Systems.Entities
{
    /// <summary>
    /// Class that exists until the player sends auth information
    /// </summary>
    public sealed class UnauthorizedPlayer : NetworkBehaviour
    {
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            Setup();
            NetworkServer.Destroy(gameObject);
        }

        private void Setup()
        {
            bool testingServerOnlyInEditor = isServer && ApplicationStateManager.Instance.TestingServerOnlyInEditor && Application.isEditor;
            if (testingServerOnlyInEditor)
            {
                gameObject.name = "Editor Server"; 
                return;
            }

            string ckey = LocalPlayerAccountManager.Ckey;
            NetworkConnectionToClient conn = netIdentity.connectionToClient;
            ServiceLocator.Shared.Get<IPlayerControlManagerService>()?.InvokeAuthorizationRequested(ckey, conn);
        }
    }
}