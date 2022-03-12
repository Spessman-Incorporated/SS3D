using DG.Tweening;
using UnityEngine;

namespace SS3D.Utils
{
    public class UiFade : MonoBehaviour
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
            DOTween.Init();
            
            bool newGameObjectState = _intendedState == State.On;
            _canvasGroup.alpha = newGameObjectState ? 0 : 1;

            _canvasGroup.DOFade((int) _intendedState, _transitionDuration).OnComplete(
                () => gameObject.SetActive(newGameObjectState)).SetEase(Ease.InCubic);
        }
    }
}
