using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class ControlDisplayHelper : MonoBehaviour
    {
        enum ControlMethod
        {
            Keyboard,
            Gamepad
        }
        
        public enum Binding
        {
            Move,
            Look,
            Jump,
            Crouch,
            Attack,
            Interact,
            Restart
        }
        
        private PlayerInput _playerInput;
        private InputSystem_Actions _actions;
        private TextMeshProUGUI _textBox;

        [SerializeField] private ControlMethod _method;
        [SerializeField] private Binding _binding;
        
        public void Start()
        {
            _textBox = GetComponent<TextMeshProUGUI>();
            _playerInput = FindFirstObjectByType<PlayerInput>();
            _actions = new InputSystem_Actions();
            
            _textBox.text = AddButtonPromptToText.ReadAndReplaceBinding("[BP_0]", "[BP_0]", _actions.FindAction(_binding.ToString()).bindings[(int)_method]);
        }
    }
}