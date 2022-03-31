using System;
using Coimbra;
using Mirror;
using SS3D.Core.Networking;
using SS3D.Core.PlayerControl;
using UnityEngine;

namespace SS3D.Core.Systems.Entities
{
    /// <summary>
    /// Temporary in-game class that exists until the player sends auth information
    /// </summary>
    public sealed class UnauthorizedPlayer : NetworkBehaviour
    {
        private void Awake()
        {
            CmdTryToAuthorizeUser();
        }

        /// <summary>
        /// Tries to connect back to the existing Soul or create a new one
        /// </summary>
        [Command(requiresAuthority = false)]
        private void CmdTryToAuthorizeUser(NetworkConnectionToClient sender = null)
        {
            // sends the Ckey to validate and assign a Soul to this
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();

            // fires an event to tell the PlayerControlManager we want to authenticate
            string ckey = LocalPlayerAccountManager.Ckey;
            PlayerControlManager.AuthorizationRequested authorizationRequested = new PlayerControlManager.AuthorizationRequested(ckey, sender);
            eventService!.Invoke(null, authorizationRequested);
        }
    }
}