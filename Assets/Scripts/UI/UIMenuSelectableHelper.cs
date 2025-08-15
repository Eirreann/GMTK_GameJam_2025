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
            if (_button)
            {
                _textObj.text = $"{_originalText}";
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
            if(_button && _button.interactable) AdjustColorMultiplier(2f);
            if(_toggle && _toggle.interactable) AdjustColorMultiplier(2f);
            if(_slider && _slider.interactable) AdjustColorMultiplier(2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(_button && _button.interactable) AdjustColorMultiplier(1f);
            if(_toggle && _toggle.interactable) AdjustColorMultiplier(1f);
            if(_slider && _slider.interactable) AdjustColorMultiplier(1f);
        }

        private void AdjustColorMultiplier(float target)
        {
            if (_button)
            {
                var colors = _button.colors;
                colors.colorMultiplier = target;
                _button.colors = colors;
            }
            
            if (_toggle)
            {
                var colors = _toggle.colors;
                colors.colorMultiplier = target;
                _toggle.colors = colors;
            }
            
            if (_slider)
            {
                var colors = _slider.colors;
                colors.colorMultiplier = target;
                _slider.colors = colors;
            }
        }
    }
}