using System;
using System.Collections.Generic;
using System.Linq;
using Coimbra;
using TMPro;
using UnityEngine;

namespace SS3D.Core.Lobby.UI
{
    /// <summary>
    /// Controls the player list in the lobby
    /// </summary>
    public class PlayerUsernameListUIHelper : MonoBehaviour
    {
        /// <summary>
        /// The UI element this is linked to
        /// </summary>
        [SerializeField] private Transform _root;
        /// <summary>
        /// Username list, local list that is "networked" by the SyncList on LobbyManager
        /// </summary>
        [SerializeField] private List<PlayerUsernameUI> _playerUsernames;
        /// <summary>
        /// The Username panel prefab
        /// </summary>
        [SerializeField] private GameObject _uiPrefab;

        private void Start()
        {
            SubscribeToEvents();
        }

        /// <summary>
        /// Generic method to agglomerate all event managing
        /// </summary>
        public void SubscribeToEvents()
        {
            // Uses the event service to listen to lobby events
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            
            eventService!.AddListener<LobbyManager.PlayerJoinedLobby>(AddUsernameUI);
            eventService!.AddListener<LobbyManager.PlayerDisconnectedFromLobby>(RemoveUsernameUI);
        }

        /// <summary>
        /// Adds the new Username to the player list
        /// </summary>
        /// <param name="sender">Required by the ServiceLocator, unused in this function</param>
        /// <param name="data">A PlayerJoinedlobby event, that simply carries the Username</param>
        public void AddUsernameUI(object sender, LobbyManager.PlayerJoinedLobby data)
        {
            // if this Username already exists we return
            if (_playerUsernames.Exists((player) => data.Username == player.Name))
            {
                return;
            }
            
            // adds the UI element and updates the text
            GameObject uiInstance = Instantiate(_uiPrefab, _root);

            PlayerUsernameUI playerUsernameUI = uiInstance.GetComponent<PlayerUsernameUI>();
            playerUsernameUI.UpdateNameText(data.Username);
            _playerUsernames.Add(playerUsernameUI);
        }
        
        /// <summary>
        /// Removes the player from the list based on the Username
        /// </summary>
        /// <param name="sender">Required by the ServiceLocator, unused in this function</param>
        /// <param name="data">A PlayerJoinedlobby event, that simply carries the Username</param>
        private void RemoveUsernameUI(object sender, LobbyManager.PlayerDisconnectedFromLobby data)
        {
            PlayerUsernameUI removedUsername = null;
            foreach (PlayerUsernameUI playerUsernameUI in _playerUsernames.Where(playerUsernameUI => playerUsernameUI.Name.Equals(data.Username)))
            {
                removedUsername = playerUsernameUI;
                Destroy(playerUsernameUI.gameObject);
            }

            _playerUsernames.Remove(removedUsername);
            Destroy(removedUsername != null ? removedUsername.gameObject : null);
        }
    }
}