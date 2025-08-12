using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Input
{
    // Place on same object as PlayerInput component
    public class ControlsChangedHelper : MonoBehaviour
    {
        public UnityAction OnControlsChanged;
        private PlayerInput _playerInput;

        private string _lastControlScheme;
        
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        public void Update()
        {
            if (_playerInput.currentControlScheme != _lastControlScheme)
            {
                _lastControlScheme = _playerInput.currentControlScheme;
                OnControlsChanged.Invoke();
            }
        }

    }
}