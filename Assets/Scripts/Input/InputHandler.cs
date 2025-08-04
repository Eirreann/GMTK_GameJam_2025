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

    public bool usingGamepad;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputSystem = new InputSystem_Actions();
        playerInput = GetComponent<PlayerInput>();

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
