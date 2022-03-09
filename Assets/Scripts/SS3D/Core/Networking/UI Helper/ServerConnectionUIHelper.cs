using DG.Tweening;
using Mirror;
using UnityEngine;

namespace SS3D.Core.Networking.UI_Helper
{
    public class ServerConnectionUIHelper : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
    
        [SerializeField] private Transform _loadingCircle;
        [SerializeField] private float _loadingCircleRotationSpeed;

        private void Start()
        {
            InitializeLoadingCircleAnimation();
        }

        private void InitializeLoadingCircleAnimation()
        {
            _loadingCircle.DOLocalRotate(new Vector3(0, 0, -360), _loadingCircleRotationSpeed, RotateMode.LocalAxisAdd).OnComplete(InitializeLoadingCircleAnimation).SetEase(Ease.Linear);
        }
    }
}
