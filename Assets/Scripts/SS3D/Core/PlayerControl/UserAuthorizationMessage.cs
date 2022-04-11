using JetBrains.Annotations;
using Mirror;

namespace SS3D.Core.PlayerControl
{
    /// <summary>
    /// Network messaged used to authorize the client.
    /// TODO: Update this with actual access token and add validation in PlayerControlManager later
    /// </summary>
    public struct UserAuthorizationMessage : NetworkMessage
    {
        public readonly string Ckey;

        public UserAuthorizationMessage(string ckey)
        {
            Ckey = ckey;
        }
    }
}