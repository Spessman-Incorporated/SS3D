using Coimbra;
using Mirror;
using SS3D.Systems.Entities;

namespace SS3D.Core.Networking
{
    public class SpessmanNetworkManager : NetworkManager
    {
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            NetworkIdentity[] ownedObjects = new NetworkIdentity[conn.clientOwnedObjects.Count];
            conn.clientOwnedObjects.CopyTo(ownedObjects);
            foreach (var networkIdentity in ownedObjects)
            {
                Soul soul = networkIdentity.GetComponent<Soul>();
                if (soul != null)
                {
                    ServiceLocator.Shared.Get<IPlayerControlManagerService>()?.InvokePlayerLeftServer(soul);    
                }
                
                networkIdentity.RemoveClientAuthority();
            }
        }
    }
}