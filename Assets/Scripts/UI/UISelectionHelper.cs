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
        private PlayerInput _playerInput;
        public InputSystem_Actions _inputSystem;
        
        [SerializeField] private Button _defaultButton;

        [SerializeField] private GameObject _lastVisitedButton;
        [SerializeField] private GameObject _lastSelectedButton;
        
        [SerializeField] private String _lastControlScheme;

        [SerializeField] private List<GameObject> _activePanels;
        [SerializeField] public Image tint;
        
        [SerializeField] private bool _canCloseRoot;
        public UnityAction OnCloseRoot;

        private void Start()
        {
            _lastSelectedButton = _defaultButton.gameObject;
            _lastVisitedButton = _defaultButton.gameObject;
            EventSystem.current.SetSelectedGameObject(_defaultButton.gameObject);
            
            _playerInput = GetComponent<PlayerInput>();
            _inputSystem = new InputSystem_Actions();
        }

        private void Update()
        {
            if (_playerInput.currentControlScheme != _lastControlScheme)
            {
                EventSystem.current.SetSelectedGameObject(_lastVisitedButton);
                _lastControlScheme = _playerInput.currentControlScheme;

                if (_lastVisitedButton && _playerInput.currentControlScheme != "Gamepad")
                {
                    _lastVisitedButton.GetComponent<UIMenuButtonHelper>().ResetButtonText();
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
            
            if (_inputSystem.UI.Cancel.WasPressedThisFrame())
            {
                if (_activePanels.Count == 1 && _canCloseRoot)
                {
                    OnCloseRoot.Invoke();
                    return;
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
            EventSystem.current.SetSelectedGameObject(_lastVisitedButton);
        }
        public void GrabLastSelectedButton()
        {
            EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
        }
        
        public void SetLastVisited(GameObject lastVisitedButton)
        {
            if(_lastVisitedButton) _lastVisitedButton.GetComponent<UIMenuButtonHelper>().ResetButtonText();
            _lastVisitedButton = lastVisitedButton;
        }
        public void SetLastSelected(GameObject lastSelectedButton)
        {
            
            _lastSelectedButton = lastSelectedButton;
        }
    }
}