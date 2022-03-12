using UnityEngine;

namespace SS3D.Utils
{
    /// <summary>
    /// Makes objects persist throughout the scenes
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
