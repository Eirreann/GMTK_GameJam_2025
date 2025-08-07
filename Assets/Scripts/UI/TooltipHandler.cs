using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TooltipHandler : MonoBehaviour
{
    [SerializeField] private TMP_SpriteAsset keyboardAsset;
    [SerializeField] private TMP_SpriteAsset gamepadAsset;

    [SerializeField] private TextMeshProUGUI tooltipText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeTextAsset(PlayerInput playerInput)
    {
        Debug.Log("Changing text asset.");
        tooltipText.text = tooltipText.text;
        
        if (playerInput.currentControlScheme == "Gamepad")
        {
            tooltipText.spriteAsset = gamepadAsset;
        }
        else if(playerInput.currentControlScheme == "Keyboard")
        {
            tooltipText.spriteAsset = keyboardAsset;
        }
    }
}
