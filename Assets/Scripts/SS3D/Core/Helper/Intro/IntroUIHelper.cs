using DG.Tweening;
using UnityEngine;

namespace SS3D.Core.Helper.Intro
{
    /// <summary>
    /// This class simply manages the UI in the intro
    /// </summary>
    public class IntroUIHelper : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _introUiFade;
        [SerializeField] private CanvasGroup _connectionUiFade;
        [SerializeField] private float _transitionDuration;

        private void Awake()
        {
            Setup();
        }

        private void Setup()
        {
            if (ApplicationStateManager.Instance.SkipIntro)
            {
                ApplicationStateManager.Instance.InitializeApplication();
            }
            else
            {
                TurnOnConnectionUIAfterFade();
            }
        }

        private void TurnOnConnectionUIAfterFade()
        {
            _introUiFade.alpha = 0;

            _introUiFade.DOFade(1, _transitionDuration / 3).OnComplete(() =>
            {
                _introUiFade.DOFade(0, _transitionDuration).SetDelay(3).OnComplete(() =>
                {
                    _connectionUiFade.DOFade(1, _transitionDuration / 2);
                }).OnComplete(ApplicationStateManager.Instance.InitializeApplication);
            }).SetEase(Ease.InCubic);
        }
    }
}
