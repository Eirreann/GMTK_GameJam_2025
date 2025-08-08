using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Input
{
    // Place on same object as PlayerInput component
    public class ControlsChangedHelper : MonoBehaviour
    {
        private UnityAction _onControlsChanged;
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
                _onControlsChanged.Invoke();
                _lastControlScheme = _playerInput.currentControlScheme;
            }
        }

    }
}