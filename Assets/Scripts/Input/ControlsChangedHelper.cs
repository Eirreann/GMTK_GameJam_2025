using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Input
{
    // Place on same object as PlayerInput component
    public class ControlsChangedHelper : MonoBehaviour
    {
        public UnityAction OnControlsChanged;
        
        [SerializeField] private PlayerInput _playerInput;

        private string _lastControlScheme;
        
        private void Awake()
        {
            _playerInput = FindFirstObjectByType<PlayerInput>();
        }

        public void Update()
        {
            if (_playerInput.currentControlScheme != _lastControlScheme)
            {
                _lastControlScheme = _playerInput.currentControlScheme;
                
                if(OnControlsChanged != null) OnControlsChanged.Invoke();
            }
        }

    }
}