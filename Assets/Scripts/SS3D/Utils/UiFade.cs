using DG.Tweening;
using UnityEngine;

namespace SS3D.Utils
{
    /// <summary>
    /// Fades a CanvasGroup alpha property by seconds
    /// </summary>
    public sealed class UiFade : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _transitionDuration;
        [SerializeField] private State _intendedState;

        private enum State
        {
            On = 1,
            Off = 0
        }

        private void Start()
        {
            Setup();
        }

        private void Setup()
        { 
            bool fadeOut = _intendedState == State.On;
            _canvasGroup.alpha = fadeOut ? 0 : 1;

            _canvasGroup.DOFade((int) _intendedState, _transitionDuration).OnComplete(
                () => gameObject.SetActive(fadeOut)).SetEase(Ease.InCubic);
        }
    }
}
