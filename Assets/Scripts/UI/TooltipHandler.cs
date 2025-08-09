using System;
using TMPro;
using UI;
using UnityEngine;



public class TooltipHandler : MonoBehaviour
{
    [SerializeField] private TMP_SpriteAsset _keyboardAsset;
    [SerializeField] private TMP_SpriteAsset _gamepadAsset;

    [SerializeField] private TextMeshProUGUI _tooltipText;
    [SerializeField] private TextMeshProUGUI _controlMapText;
    [SerializeField] private TextMeshProUGUI _contextPopup;

    [SerializeField] private SetPromptText _tooltipPromptSetter;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeTextAsset(String currentControlScheme)
    {
        Debug.Log("Changing text asset.");
        TMP_SpriteAsset asset = _keyboardAsset;
        
        if (currentControlScheme == "Gamepad")
        {
            asset = _gamepadAsset;
        }
        else if(currentControlScheme == "Keyboard")
        {
            asset = _keyboardAsset;
        }

        _tooltipText.spriteAsset = asset;
        _contextPopup.spriteAsset = asset;
    }
}
