using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Input;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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

        private void Awake()
        {
            playerInput = InputHandler.Instance.playerInput;
            _textBox = GetComponent<TMP_Text>();
        }
        
        public void ReplaceMessage(string message, TooltipSO tooltip)
        {
            var list = new List<InputAction>();
            foreach (var binding in tooltip.bindingsList)
            {
                list.Add(tooltip.GetBinding(binding));
            }
            
            _message = message;
            SetText(message, list);
        }

        public void ReplaceMessage(string message, InputAction action)
        {
            _message = message;
            SetText(message, new List<InputAction>() { action });
        }

        public void Refresh()
        {
            SetText("", new List<InputAction>());
        }

        [ContextMenu("Set Text")]
        private void SetText(string message, List<InputAction> actions )
        {
            var textBox = GetComponent<TMP_Text>();
            
            if (actions == null || actions.Count == 0)
            {
                textBox.text = message;
                return;
            }
            
            var list = new List<string>() { "Keyboard", "Gamepad" };
            
            var schemeIndex = list.IndexOf(playerInput.currentControlScheme);

            var count = 0;
            foreach (var action in actions)
            {
                textBox.spriteAsset = spriteAssetList.SpriteAssets[schemeIndex];
                
                textBox.text = AddButtonPromptToText.ReadAndReplaceBinding(message, $"[BP_{count}]", action.bindings[schemeIndex]);
                message = textBox.text;

                count++;
            }
            
        }
    }
}