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
        private void Start()
        {
            CmdTryToAuthorizeUser(LocalPlayerAccountManager.Ckey);
        }

        /// <summary>
        /// Tries to connect back to the existing Soul or create a new one
        /// </summary>
        [Command(requiresAuthority = false)]
        private void CmdTryToAuthorizeUser(string ckey, NetworkConnectionToClient sender = null)
        {
            Debug.Log("Unauthorized player: try to authorize player");

            // sends the Ckey to validate and assign a Soul to this
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            PlayerControlManager.AuthorizationRequested authorizationRequested = new PlayerControlManager.AuthorizationRequested(ckey, sender);
            eventService!.Invoke(null, authorizationRequested);
        }
    }
}