using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerInput playerInput;
    
    public InputSystem_Actions inputSystem;
    private InputSystem_Actions.PlayerActions playerActions;

    public Vector2 _moveDirection;
    public Vector2 _lookDirection;

    public bool _isJumping = false;
    public bool _run;
    public bool _crouch;
    public bool _drawWall;

    public bool _interact;
    
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
        _lookDirection = playerActions.Look.ReadValue<Vector2>();
        _moveDirection = playerActions.Move.ReadValue<Vector2>();

        _isJumping = playerActions.Jump.WasPressedThisFrame();

        _crouch = playerActions.Crouch.ReadValue<float>() > 0.25f;
        _run = playerActions.Sprint.ReadValue<float>() > 0.25f;
        
        _drawWall = playerActions.Attack.IsPressed();

        _interact = playerActions.Interact.WasPressedThisFrame();
    }
}
