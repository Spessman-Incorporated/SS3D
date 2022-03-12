using TMPro;
using UnityEngine;

namespace SS3D.Core.Lobby.UI
{
    /// <summary>
    /// Simple username ui element controller
    /// </summary>
    public class PlayerUsernameUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameLabel;

        public string Name => _nameLabel.text;
    
        public void UpdateNameText(string newName)
        {
            _nameLabel.text = newName;
        }
    }
}
