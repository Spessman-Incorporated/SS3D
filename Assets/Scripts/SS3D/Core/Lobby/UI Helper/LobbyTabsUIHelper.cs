using System;
using SS3D.Core.Lobby.UI;
using UnityEngine;

namespace SS3D.Core.Lobby.UI_Helper
{
    public sealed class LobbyTabsUIHelper : MonoBehaviour
    {
        [SerializeField] private GenericTabUI[] _categoryUi;
        
        private void Start()
        {
            SetupGenericsTabs();
            OnTabButtonClicked(0);
        }
        
        /// <summary>
        /// Sets up UI logic
        /// </summary>
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
            foreach (GenericTabUI tab in _categoryUi)
            {
                tab.UpdateCategoryState(tab == _categoryUi[index]);
            }
        }
    }
}
