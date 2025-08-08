using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIMenuButtonHelper : MonoBehaviour, ISelectHandler
    {
        private UIMenu _uiMenu;

        private void Awake()
        {
            _uiMenu = FindFirstObjectByType<UIMenu>(FindObjectsInactive.Exclude);
        }

        public void OnSelect(BaseEventData eventData)
        {
            _uiMenu.SetLastVisited(gameObject);
        }
    }
}