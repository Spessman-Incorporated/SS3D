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
        }

        private void Setup()
        {
            string ckey = LocalPlayerAccountManager.Ckey;

            CmdDestroyAfterLogin();
            Destroy(gameObject);

            Debug.Log($"[{typeof(UnauthorizedPlayer)}] - OnStartLocalPlayer - Destroying temporary player for {ckey}");

            bool testingServerOnlyInEditor = isServer && ApplicationStateManager.Instance.TestingServerOnlyInEditor && Application.isEditor;
            if (testingServerOnlyInEditor)
            {
                return;
            }

            UserAuthorizationMessage userAuthorizationMessage = new UserAuthorizationMessage(ckey);
            NetworkClient.Send(userAuthorizationMessage);
        }

        [Command(requiresAuthority = false)]
        private void CmdDestroyAfterLogin()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}