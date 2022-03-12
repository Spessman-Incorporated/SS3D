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
        /// The username panel prefab
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
        /// Adds the new username to the player list
        /// </summary>
        /// <param name="sender">Required by the ServiceLocator, unused in this function</param>
        /// <param name="data">A PlayerJoinedlobby event, that simply carries the username</param>
        public void AddUsernameUI(object sender, LobbyManager.PlayerJoinedLobby data)
        {
            // if this username already exists we return
            if (_playerUsernames.Exists((player) => data.username == player.Name))
            {
                return;
            }
            
            // adds the UI element and updates the text
            GameObject uiInstance = Instantiate(_uiPrefab, _root);

            PlayerUsernameUI playerUsernameUI = uiInstance.GetComponent<PlayerUsernameUI>();
            playerUsernameUI.UpdateNameText(data.username);
            _playerUsernames.Add(playerUsernameUI);
        }
        
        /// <summary>
        /// Removes the player from the list based on the username
        /// </summary>
        /// <param name="sender">Required by the ServiceLocator, unused in this function</param>
        /// <param name="data">A PlayerJoinedlobby event, that simply carries the username</param>
        private void RemoveUsernameUI(object sender, LobbyManager.PlayerDisconnectedFromLobby data)
        {
            foreach (PlayerUsernameUI playerUsernameUI in _playerUsernames.Where(playerUsernameUI => playerUsernameUI.Name.Equals(data.username)))
            {
                _playerUsernames.Remove(playerUsernameUI);
            }
        }
    }
}