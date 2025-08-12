using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputHandler : MonoBehaviour
{
    public const string KEYBOARD_INPUT_NAME = "Keyboard";
    public const string GAMEPAD_INPUT_NAME = "Gamepad";
    
    public PlayerInput playerInput;
    
    public InputSystem_Actions inputSystem;
    private InputSystem_Actions.PlayerActions playerActions;

    [SerializeField] private TooltipHandler tooltipHandler;

    public Vector2 _moveDirection;
    public Vector2 _lookDirection;

    public bool _jump = false;
    public bool _crouch;
    public bool _drawWall;

    public bool _interact;

    public bool _reset;

    public bool _pause;
    
    private const float GAMEPAD_LOOK_SENSITIVITY = 10f;
    public bool usingGamepad;
    

    private string _lastControlScheme = "";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public bool LookDeviceIsMouse;
    
    public float NonMouseSensitivityModifier = .5f;
    public float MouseSensitivityModifier = 2.0f;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        inputSystem = new InputSystem_Actions();
        inputSystem.Player.Look.performed += ctx => LookDeviceSet(ctx);
    }
    
    private void LookDeviceSet(InputAction.CallbackContext context)
    {
        LookDeviceIsMouse = context.control.device.name == "Mouse";
    }
    
    void Start()
    {
        playerActions = inputSystem.Player;
        playerActions.Enable();
    }

    public void OnGameOver()
    {
        playerActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Add listener functionality rather than per-frame update
        if(playerInput.currentControlScheme != _lastControlScheme){
            tooltipHandler.ChangeTextAsset(playerInput.currentControlScheme);
            _lastControlScheme = playerInput.currentControlScheme;
        }
        
        usingGamepad = playerInput.currentControlScheme == "Gamepad";
        
        var _bufferedLook = playerActions.Look.ReadValue<Vector2>();
        if (_bufferedLook.magnitude > 0.2f)
        {
            _lookDirection = usingGamepad ? _bufferedLook * GAMEPAD_LOOK_SENSITIVITY : _bufferedLook;
        }
        else
        {
            _lookDirection = Vector2.zero;
        }
        
        var _bufferedMove = playerActions.Move.ReadValue<Vector2>();
        if (_bufferedMove.magnitude > 0.2f)
        {
            _moveDirection = _bufferedMove.normalized;
        }
        else
        {
            _moveDirection = Vector2.zero;
        }

        _jump = playerActions.Jump.WasPressedThisFrame();

        _crouch = playerActions.Crouch.ReadValue<float>() > 0.25f;
        
        _drawWall = playerActions.Attack.IsPressed();

        _interact = playerActions.Interact.WasPressedThisFrame();

        _reset = playerActions.Restart.WasPressedThisFrame();
        
        _pause = playerActions.Pause.WasPressedThisFrame();
    }
}
}
