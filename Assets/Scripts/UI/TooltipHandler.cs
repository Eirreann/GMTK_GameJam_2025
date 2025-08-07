using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TooltipHandler : MonoBehaviour
{
    [SerializeField] private TMP_SpriteAsset keyboardAsset;
    [SerializeField] private TMP_SpriteAsset gamepadAsset;

    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private TextMeshProUGUI controlMapText;
    [SerializeField] private TextMeshProUGUI contextPopup;
    
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

        TMP_SpriteAsset asset = keyboardAsset;
        
        if (currentControlScheme == "Gamepad")
        {
            asset = gamepadAsset;
        }
        else if(currentControlScheme == "Keyboard&Mouse")
        {
            asset = keyboardAsset;
        }

        tooltipText.spriteAsset = asset;
        contextPopup.spriteAsset = asset;
    }
}
