using DG.Tweening;
using Mirror;
using SS3D.Core;
using UnityEngine;
using UnityEngine.UI;

public class ServerConnectionUIHelper : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    
    [SerializeField] private Transform _loadingCircle;
    [SerializeField] private float _loadingCircleRotationSpeed;
    
    private void Start()
    {
        DOTween.Init();
        InitializeLoadingCircleAnimation();
        SubscribeToEvents();
    }

    private void InitializeLoadingCircleAnimation()
    {
        _loadingCircle.DOLocalRotate(new Vector3(0, 0, -360), _loadingCircleRotationSpeed, RotateMode.LocalAxisAdd).OnComplete(InitializeLoadingCircleAnimation).SetEase(Ease.Linear);
    }

    private void SubscribeToEvents()
    {
        NetworkServer.OnConnectedEvent += connection => { _root.SetActive(false); };
    }
}
