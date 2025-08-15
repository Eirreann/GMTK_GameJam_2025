using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class UIMenuSelectableHelper : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private bool _isBackButton;
        
        private UISelectionHelper _uiMenu;
        private Button _button;
        private Toggle _toggle;
        private Slider _slider;
        private TextMeshProUGUI _textObj;
        private PlayerInput _playerInput;
        private String _originalText;

        private void Awake()
        {
            _uiMenu = FindFirstObjectByType<UISelectionHelper>(FindObjectsInactive.Exclude);
            _playerInput = FindFirstObjectByType<PlayerInput>();
            
            _button = GetComponent<Button>();
            _slider = GetComponent<Slider>();
            _toggle = GetComponent<Toggle>();
            
            if (_button)
            {
                _textObj = _button.GetComponentInChildren<TextMeshProUGUI>();
                _originalText = _textObj.text;
                
                if(_isBackButton && _uiMenu) _button.onClick.AddListener(_uiMenu.ReturnToLastButton); 
            }
        }
        
        public void Update() 
        {
            if (_textObj && _button)
            {
                var color = _textObj.color;
                color.a = _button.interactable ? 1f : 0.25f;
                _textObj.color = color;
            }
        }

        public void ResetButtonText()
        {
            var text = $"";
            if (_button)
            {
                text += $"{_originalText}";
                
                if (EventSystem.current.currentSelectedGameObject == _button.gameObject && _playerInput.currentControlScheme == "Gamepad")
                    text += $" <sprite name=\"Gamepad_buttonSouth\">";
                

                _textObj.text = text;
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            _uiMenu.SetLastVisited(gameObject);
            AdjustColorMultiplier(2f);
            
            if (_button && _playerInput.currentControlScheme == "Gamepad")
            {
                _textObj.text = $"{_originalText} <sprite name=\"Gamepad_buttonSouth\">";
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            AdjustColorMultiplier(1f);
            if (_button)
            {
                var text = $"{_originalText}";
                _textObj.text = text;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            AdjustColorMultiplier(2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            AdjustColorMultiplier(1f);
        }

        public void AdjustColorMultiplier(float target)
        {
            if (_button && _button.interactable)
            {
                var colors = _button.colors;
                colors.colorMultiplier = target;
                _button.colors = colors;
            }
            
            if (_toggle && _toggle.interactable)
            {
                var colors = _toggle.colors;
                colors.colorMultiplier = target;
                _toggle.colors = colors;
            }
            
            if (_slider && _slider.interactable)
            {
                var colors = _slider.colors;
                colors.colorMultiplier = target;
                _slider.colors = colors;
            }
        }
    }
}