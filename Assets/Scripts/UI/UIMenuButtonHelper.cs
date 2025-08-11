using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class UIMenuButtonHelper : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        private UISelectionHelper _uiMenu;
        private Button _button;

        private PlayerInput _playerInput;
        
        private String _originalText;

        private void Awake()
        {
            _uiMenu = FindFirstObjectByType<UISelectionHelper>(FindObjectsInactive.Exclude);
            _playerInput = FindFirstObjectByType<PlayerInput>();
            
            
            _button = GetComponent<Button>();
            if (_button)
            {
                _originalText = _button.GetComponentInChildren<TextMeshProUGUI>().text;
            }
        }

        public void ResetButtonText()
        {
            if (_button)
            {
                _button.GetComponentInChildren<TextMeshProUGUI>().text = $"{_originalText}";
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            _uiMenu.SetLastVisited(gameObject);
            
            if (_button && _playerInput.currentControlScheme == "Gamepad")
            {
                _button.GetComponentInChildren<TextMeshProUGUI>().text = $"{_originalText} <sprite name=\"Gamepad_buttonSouth\">";
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (_button)
            {
                _button.GetComponentInChildren<TextMeshProUGUI>().text = $"{_originalText}";
            }
        }
    }
}