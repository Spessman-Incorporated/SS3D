using System;
using SS3D.Core.Lobby.UI;
using UnityEngine;

namespace SS3D.Core.Lobby.UI_Helper
{
    public class LobbyTabsUIHelper : MonoBehaviour
    {
        [SerializeField] private GenericTabUI[] _categoryUi;
        
        private void Start()
        {
            Destroy(this);
            SetupGenericsTabs();
            OnTabButtonClicked(0);
        }
        
        private void SetupGenericsTabs()
        {
            for (int i = 0; i < _categoryUi.Length; i++)
            {
                int index = i;
                _categoryUi[i].Button.onClick.AddListener(() => OnTabButtonClicked(index));
            }
        }

        private void OnTabButtonClicked(int index)
        {
            Debug.Log(index);
            foreach (GenericTabUI tab in _categoryUi)
            {
                tab.UpdateCategoryState(tab == _categoryUi[index]);
            }
        }
    }
}
