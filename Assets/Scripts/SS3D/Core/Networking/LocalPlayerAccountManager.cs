using UnityEngine;

namespace SS3D.Core.Networking
{
    /// <summary>
    /// Holds data from the Hub to send the server when we connect
    /// </summary>
    public static class LocalPlayerAccountManager
    {
        private static string _ckey;

        public static string Ckey => _ckey;

        public static void UpdateCkey(string ckey)
        {
            _ckey = ckey; 
        }

    }
}
