using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class UISelectionHelper : MonoBehaviour
    {
        public PlayerInput playerInput;
        public InputSystem_Actions _inputSystem;
        
        [SerializeField] private Button _defaultButton;

        [SerializeField] private GameObject _lastVisitedButton;
        [SerializeField] private GameObject _lastSelectedButton;
        
        [SerializeField] private String _lastControlScheme;

        [SerializeField] private List<GameObject> _activePanels;
        [SerializeField] public Image tint;
        
        [SerializeField] private bool _canCloseRoot;
        [SerializeField] private bool _activateUIInputsOnStart;
        
        
        public UnityAction OnCloseRoot;

        private void Start()
        {
            SetLastSelected(_defaultButton.gameObject);
            SetLastVisited(_defaultButton.gameObject);
            EventSystem.current.SetSelectedGameObject(_defaultButton.gameObject);
            
            playerInput = GetComponent<PlayerInput>();
            _inputSystem = new InputSystem_Actions();
            
            if(_activateUIInputsOnStart) _inputSystem.UI.Enable();
        }

        private void Update()
        {
            if (playerInput.currentControlScheme == "Keyboard" && EventSystem.current.IsPointerOverGameObject())
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            
            if (playerInput.currentControlScheme != _lastControlScheme)
            {
                if (_lastVisitedButton && playerInput.currentControlScheme != "Gamepad")
                {
                    _lastVisitedButton.GetComponent<UIMenuSelectableHelper>().ResetButtonText();
                }
                else
                {
                    GrabLastVisitedButton();
                }
                
                _lastControlScheme = playerInput.currentControlScheme;
            }

            
            
            if (_inputSystem.UI.Cancel.WasPressedThisFrame())
            {
                if (_activePanels.Count == 1 && _canCloseRoot)
                {
                    OnCloseRoot.Invoke();
                }
                if(_activePanels.Count > 1) ReturnToLastButton();
            }
        }
        
        public void ReturnToLastButton()
        {
            _activePanels[_activePanels.Count - 1].SetActive(false);
            RemoveActivePanel(_activePanels[_activePanels.Count - 1]);
            
            _activePanels[_activePanels.Count - 1].SetActive(true);
            if(tint) tint.enabled = _activePanels.Count > 1;
                
            EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
        }

        public void AddActivePanel(GameObject panel)
        {
            _activePanels.Add(panel);
        }
        
        public void RemoveActivePanel(GameObject panel)
        {
            _activePanels.Remove(panel);
        }

        public void GrabLastVisitedButton()
        {
            var helper = _lastVisitedButton.GetComponent<UIMenuSelectableHelper>();
            helper.AdjustColorMultiplier(2f);
            
            if (playerInput.currentControlScheme != "Keyboard")
            {
                EventSystem.current.SetSelectedGameObject(_lastVisitedButton);
                return;
            }
            
            EventSystem.current.SetSelectedGameObject(null);
            
        }
        public void GrabLastSelectedButton()
        {
            var helper = _lastSelectedButton.GetComponent<UIMenuSelectableHelper>();
            helper.AdjustColorMultiplier(2f);
            
            if (playerInput.currentControlScheme != "Keyboard")
            {
                EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
                return;
            }
            
            EventSystem.current.SetSelectedGameObject(null);
        }
        
        public void SetLastVisited(GameObject lastVisitedButton)
        {
            if (_lastVisitedButton)
            {
                var helper = _lastVisitedButton.GetComponent<UIMenuSelectableHelper>();
                helper.AdjustColorMultiplier(1f);
                helper.ResetButtonText();
                
            }
            
            _lastVisitedButton = lastVisitedButton;
        }
        public void SetLastSelected(GameObject lastSelectedButton)
        {
            if (_lastVisitedButton)
            {
                var helper = _lastSelectedButton.GetComponent<UIMenuSelectableHelper>();
                helper.ResetButtonText();
                
                helper.AdjustColorMultiplier(1f);
            }
            
            _lastSelectedButton = lastSelectedButton;
        }
    }
}