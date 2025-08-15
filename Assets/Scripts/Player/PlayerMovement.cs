using Game;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Camera playerCamera;
    
    public bool IsGrounded;
    public LayerMask GroundLayer;
    
    [SerializeField] public Vector3 _desiredMoveDirection;
    [SerializeField] public Vector3 _lastMoveDirection;
    
    [Header("Sliding")]
    [Range(0f, 5f)] [SerializeField] public float slideForce = 2.5f;
    
    [Header("Jumping")]
    [SerializeField] public float jumpForce = 20f;

    [SerializeField] public float playerSpeed;
    [SerializeField] private float _currentSpeed;
    [Range(1f, 99f)][SerializeField] private float _speedRampUp;
    [Range(1f, 99f)][SerializeField] private float _speedRampDown;
    
    public float xRotation = 0f;
    public float yRotation = 0f;
    
    [Range(0.1f, 9f)][SerializeField] private float _sensitivity = 2f;
    [Range(0f, 90f)][SerializeField] private float _yRotationLimit = 88f;

    public Vector3 velocity;
    public float gravityScale = 5f;
     
    private float _knockbackCooldown;
    private const float RESET_COOLDOWN = 1f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        _desiredMoveDirection = (new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z) * GameManager.Instance.inputHandler._moveDirection.y + playerCamera.transform.right * GameManager.Instance.inputHandler._moveDirection.x).normalized;
        _desiredMoveDirection = new Vector3(_desiredMoveDirection.x, 0, _desiredMoveDirection.z);
    }
    
    public void RotatePlayer(Vector2 directionValue)
    {
        
        var _modifier = GameManager.Instance.inputHandler.NonMouseSensitivityModifier * _sensitivity * Time.deltaTime;
        if (GameManager.Instance.inputHandler.LookDeviceIsMouse)
        {
            _modifier = GameManager.Instance.inputHandler.MouseSensitivityModifier * _sensitivity;
        }
        
        directionValue *= _modifier;
        
        yRotation += directionValue.x;
        xRotation -= directionValue.y;
         
        xRotation = Mathf.Clamp(xRotation, -_yRotationLimit, _yRotationLimit);
         
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
    
    public void ProcessMovement(float desiredSpeed)
     {
         AdjustPlayerSpeed(desiredSpeed);
         
         _rb.linearVelocity = new Vector3(
             _desiredMoveDirection.x * _currentSpeed,
             _rb.linearVelocity.y,
             _desiredMoveDirection.z * _currentSpeed
         );
     }
    
    private void AdjustPlayerSpeed(float desiredSpeed)
    {
        if (_desiredMoveDirection != Vector3.zero)
        {
            if (_currentSpeed <= desiredSpeed)
            {
                _currentSpeed += desiredSpeed * _speedRampUp * Time.unscaledDeltaTime;
            }
            else if (_currentSpeed > desiredSpeed)
            {
                _currentSpeed -= desiredSpeed * _speedRampDown * Time.unscaledDeltaTime;
            }
        }
        else
        {
            if (_currentSpeed > 0f)
            {
                _currentSpeed -= desiredSpeed * _speedRampDown * Time.unscaledDeltaTime;
                return;
            }
            _currentSpeed = 0f;
        }
    }
    
    public void SetKnockback(Vector3 force)
    {
        _knockbackCooldown = RESET_COOLDOWN;
        _rb.linearVelocity = Vector3.zero;
        _rb.AddForce(force, ForceMode.Impulse);
    }

    public void DoWallJump()
    {
        Vector3 forceToApply = transform.up * jumpForce + playerCamera.transform.forward * jumpForce;
        
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        
        _rb.AddForce(
            new Vector3(
                forceToApply.x,
                Mathf.Clamp(forceToApply.y, 30f, 75f),
                forceToApply.z
            ), 
            ForceMode.Impulse
        );
    }
}
}