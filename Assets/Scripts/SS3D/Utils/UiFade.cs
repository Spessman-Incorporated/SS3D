using DG.Tweening;
using UnityEngine;

namespace SS3D.Utils
{
    /// <summary>
    /// Fades a CanvasGroup alpha property by seconds
    /// </summary>
    public sealed class UiFade : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [Header("Settings")]
        [SerializeField] private float _transitionDuration;
        [SerializeField] private State _intendedState;
        [SerializeField] private bool _fadeOnStart;
        
        private bool _currentState;

        private enum State
        {
            On = 1,
            Off = 0
        }

        private void Start()
        {
            if (_fadeOnStart)
            {
                ProcessFade();
            }
        }

        public void ProcessFade()
        { 
            bool fadeOut = _intendedState == State.On;
            _currentState = fadeOut;
            _canvasGroup.alpha = fadeOut ? 0 : 1;

            _canvasGroup.DOFade((int) _intendedState, _transitionDuration).OnComplete(
                () => gameObject.SetActive(fadeOut)).SetEase(Ease.InCubic);

            _currentState = !_currentState;
        }
    }
}
