using System;
using System.Collections.Generic;
using System.Linq;
using Coimbra;
using TMPro;
using UnityEngine;

namespace SS3D.Core.Lobby.UI
{
    public class PlayerUsernameListUIHelper : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        [SerializeField] private List<PlayerUsernameUI> _playerUsernames;
        [SerializeField] private GameObject _uiPrefab;

        private void Start()
        {
            SubscribeToEvents();
        }

        public void SubscribeToEvents()
        {
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>();
            
            eventService!.AddListener<LobbyManager.PlayerJoinedLobby>(AddUsernameUI);
            eventService!.AddListener<LobbyManager.PlayerDisconnectedFromLobby>(RemoveUsernameUI);
        }

        public void AddUsernameUI(object sender, LobbyManager.PlayerJoinedLobby data)
        {
            if (_playerUsernames.Exists((player) => data.username == player.Name))
            {
                return;
            }
            
            GameObject uiInstance = Instantiate(_uiPrefab, _root);

            PlayerUsernameUI playerUsernameUI = uiInstance.GetComponent<PlayerUsernameUI>();
            playerUsernameUI.UpdateNameText(data.username);
            _playerUsernames.Add(playerUsernameUI);
        }
        
        private void RemoveUsernameUI(object sender, LobbyManager.PlayerDisconnectedFromLobby data)
        {
            foreach (PlayerUsernameUI playerUsernameUI in _playerUsernames.Where(playerUsernameUI => playerUsernameUI.Name.Equals(data.username)))
            {
                _playerUsernames.Remove(playerUsernameUI);
            }
        }
    }
}