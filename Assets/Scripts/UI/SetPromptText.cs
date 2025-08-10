using System;
using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.tvOS;

namespace UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class SetPromptText : MonoBehaviour
    {
        [TextArea(2, 3)] [SerializeField] private string _message = "Press BUTTONPROMPT to interact.";
        
        [Header("Sprite Asset List")]
        [SerializeField] private SpriteAssetList spriteAssetList;

        public List<InputAction> _bindingsList;

        public PlayerInput playerInput;
        private InputSystem_Actions _inputActions;
        private TMP_Text _textBox;
        
        public Dictionary<String, InputAction> _bindingsLookup;
        public Dictionary<String, int> _deviceType;

        private void Awake()
        {
            _inputActions = new InputSystem_Actions();
            _textBox = GetComponent<TMP_Text>();
            _bindingsList = new List<InputAction>();
            
            _bindingsLookup = new Dictionary<string, InputAction>()
            {
                { "Move", _inputActions.Player.Move },
                { "Look", _inputActions.Player.Look },
                { "Interact", _inputActions.Player.Interact },
                { "Attack", _inputActions.Player.Attack },
                { "Jump", _inputActions.Player.Jump },
                { "Crouch", _inputActions.Player.Crouch },
                { "Reset", _inputActions.Player.Restart },
            };

            _deviceType = new Dictionary<String, int>()
            {
                { "Keyboard", 0 },
                { "Gamepad", 1 },
            };
        }
        
        public void ReplaceMessage(string message, string currentControlScheme, List<string> bindingsList)
        {
            if (bindingsList.Count <= 0) return;
            
            _message = message;
            _bindingsList.Clear();

            foreach (var binding in bindingsList)
            {
                _bindingsList.Add(_bindingsLookup[binding]);
            }
            
            SetText();
        }

        public void Refresh()
        {
            SetText();
        }

        [ContextMenu("Set Text")]
        private void SetText()
        {
            if(!playerInput) playerInput = GameManager.Instance.inputHandler.playerInput;
            
            if (_deviceType[playerInput.currentControlScheme] > spriteAssetList.SpriteAssets.Count - 1)
            {
                Debug.Log($"Missing Asset for type: {_deviceType}");
            }

            var count = 0;
            foreach (var binding in _bindingsList)
            {
                _textBox.text = AddButtonPromptToText.ReadAndReplaceBinding(_message, $"[BP_{count}]", binding.bindings[_deviceType[playerInput.currentControlScheme]], spriteAssetList.SpriteAssets[_deviceType[playerInput.currentControlScheme]]);
                _message = _textBox.text;

                count++;
            }
            
        }
    }
}