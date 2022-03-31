using System;
using System.Collections;
using Coimbra;
using DG.Tweening;
using kcp2k;
using Mirror;
using SS3D.Data;
using SS3D.Data.Messages;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace SS3D.Core.Networking.UI_Helper
{
    /// <summary>
    /// Displays the process of connecting to a server on ui elements
    /// </summary>
    public sealed class ServerConnectionUIHelper : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
    
        [Header("Loading Icon")]
        [SerializeField] private Transform _loadingIcon;
        [SerializeField] private float _loadingIconAnimationDuration;

        [Header("Buttons")]
        [SerializeField] private GameObject _buttons;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _retryButton;
        
        [SerializeField] private TMP_Text _messageText;

        public struct RetryButtonClicked {}

        private bool connectionFailed;
        
        private void Start()
        {
            ProcessConnectingToServer();
            Setup();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void Setup()
        {
            UpdateMessageText(ApplicationMessages.Network.ConnectingToServer);
            
            _buttons.SetActive(false);
        }

        private void SubscribeToEvents()
        {
            _quitButton.onClick.AddListener(Application.Quit);
            _retryButton.onClick.AddListener(OnRetryButtonPressed);
            KcpConnection.ConnectionFailed += OnServerConnectionFailed;
        }

        private void UnsubscribeFromEvents()
        {
            KcpConnection.ConnectionFailed -= OnServerConnectionFailed;
        }
        
        private void UpdateMessageText(string message)
        {
            _messageText.text = message;
        }
        
        /// <summary>
        /// Loops the rotating animation using DOTween until we are ot connecting anymore
        /// </summary>
        private void ProcessConnectingToServer()
        {
            if (connectionFailed)
            {
                return;
            }
            
            // loops a rotating animation
            _loadingIcon.DOLocalRotate(new Vector3(0, -360, 0), _loadingIconAnimationDuration, RotateMode.LocalAxisAdd).OnComplete(ProcessConnectingToServer).SetEase(Ease.Linear);
        }

        private void OnRetryButtonPressed()
        {
            connectionFailed = false;
            _buttons.SetActive(false);
            _loadingIcon.gameObject.SetActive(true);
            UpdateMessageText(ApplicationMessages.Network.ConnectingToServer);
            ProcessConnectingToServer();
            
            // Uses the event service to listen to lobby events
            IEventService eventService = ServiceLocator.Shared.Get<IEventService>(); 
            eventService?.Invoke(this, new RetryButtonClicked());
        }
        
        private void OnServerConnectionFailed()
        {
            connectionFailed = true;
            _buttons.SetActive(true);
            _loadingIcon.gameObject.SetActive(false);
            UpdateMessageText(ApplicationMessages.Network.ConnectionFailed);
        }
    }
}
