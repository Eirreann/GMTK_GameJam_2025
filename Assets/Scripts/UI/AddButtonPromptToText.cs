using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public static class AddButtonPromptToText
    {
        public static string ReadAndReplaceBinding(string textToDisplay, string stringToReplace, InputBinding binding, TMP_SpriteAsset spriteAsset)
        {
            string stringButtonName = binding.ToString();
            stringButtonName = RenameInput(stringButtonName, binding.action);

            textToDisplay = textToDisplay.Replace(stringToReplace, $"<sprite name=\"{stringButtonName}\">");

            return textToDisplay;
        }
        private static string RenameInput(string stringButtonName, string binding)
        {
            stringButtonName = stringButtonName.Replace($"{binding}:", String.Empty);
            
            stringButtonName = stringButtonName.Replace("[Keyboard]", String.Empty);
            stringButtonName = stringButtonName.Replace("[Gamepad]", String.Empty);
            stringButtonName = stringButtonName.Replace("[Mouse]", String.Empty);
            stringButtonName = stringButtonName.Replace("[;Gamepad]", String.Empty);

            stringButtonName = stringButtonName.Replace(
                "<Keyboard>/", "Keyboard_"
            );
            stringButtonName = stringButtonName.Replace(
                "<Gamepad>/", "Gamepad_"
            );
            stringButtonName = stringButtonName.Replace(
                "<Mouse>/", "Mouse_"
            );
            stringButtonName = stringButtonName.Replace(
                "<Pointer>/delta", "Pointer_delta"
            );
            
            return stringButtonName;
        }
    }
}