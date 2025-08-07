using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIMenuButtonHelper : MonoBehaviour, ISelectHandler
    {
        [SerializeField] private UIMenu _uiMenu;
        private Button _button;

        private void Start()
        {
            _uiMenu = FindObjectsByType<UIMenu>(FindObjectsSortMode.None).First();
            _button = GetComponent<Button>();
        }

        public void OnSelect(BaseEventData eventData)
        {
            _uiMenu.SetLastVisited(gameObject);
        }
    }
}