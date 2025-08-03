using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerInput playerInput;
    
    public InputSystem_Actions inputSystem;
    private InputSystem_Actions.PlayerActions playerActions;

    public Vector2 _moveDirection;
    public Vector2 _lookDirection;

    public bool _jump = false;
    public bool _crouch;
    public bool _drawWall;

    public bool _interact;

    public bool _reset;

    public bool _pause;
    
    private const float GAMEPAD_LOOK_SENSITIVITY = 25f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputSystem = new InputSystem_Actions();
        playerInput = GetComponent<PlayerInput>();

        playerActions = inputSystem.Player;
        playerActions.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        var look = playerActions.Look.ReadValue<Vector2>();
        _lookDirection = playerInput.currentControlScheme == "Gamepad" ? look * GAMEPAD_LOOK_SENSITIVITY : look;
        _moveDirection = playerActions.Move.ReadValue<Vector2>();

        _jump = playerActions.Jump.WasPressedThisFrame();

        _crouch = playerActions.Crouch.ReadValue<float>() > 0.25f;
        
        _drawWall = playerActions.Attack.IsPressed();

        _interact = playerActions.Interact.WasPressedThisFrame();

        _reset = playerActions.Restart.WasPressedThisFrame();
        
        _pause = playerActions.Pause.WasPressedThisFrame();
    }
}
