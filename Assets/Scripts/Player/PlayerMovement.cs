using Game;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    private Rigidbody rb;

    [Header("Movement")]
    [Range(0f, 24f)] [SerializeField] private float _currentSpeed = 2f;
    [Range(0f, 24f)] [SerializeField] private float _playerSpeed = 2f;
    [SerializeField] private float _sprintMultiplier = 1.1f;
    [SerializeField] private Vector3 _desiredMoveDirection;
    
    [Header("Ramp Values")]
    [Range(0f, 5f)] [SerializeField] private float _speedRampUp = 12f;
    [Range(0f, 5f)] [SerializeField] private float _speedRampDown = 15f;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce = 20f;
    
    [Header("Sliding")]
    [Range(0f, 5f)] [SerializeField] private float _slideForce = 2.5f;
    
    [Header("Wall Jumping")]
    [SerializeField] private float _wallJumpUpForce = 20f;
    [SerializeField] private float _wallJumpSideForce = 20f;
    
    [Header("Look Rotation Values")]
    [Range(0.1f, 9f)][SerializeField] private float _sensitivity = 2f;
    [Range(0f, 90f)][SerializeField] private float _yRotationLimit = 88f;
    
    private float _xRotation = 0f;
    private float _yRotation = 0f;

    [Header("Is Doing?")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _hasJumped;
    [SerializeField] private bool _hasSlid;
    
    [SerializeField] private bool _canWallJump;
    private bool _playerCanMove;
    
    [Header("Player State Machine")]
    [SerializeField] private PlayerState _playerState = PlayerState.Standing;
    [SerializeField] private LayerMask _groundLayer;

    [SerializeField] private Transform _lastWallJumped;

    private const float RESET_COOLDOWN = 1f;
    
    private Transform _respawnLocation;
    private float _knockbackCooldown;

    public float gravityScale = 5f;
    private Quaternion _desiredRotation;
    
    private enum PlayerState
    {
        Standing,
        Crouching,
        Sliding,
        Running,
        Jumping,
        Falling
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        
        _xRotation = 0f;
        _yRotation = 0f;
        
        _playerCanMove = true;
    }

    void Update()
    {
        RaycastHit hit;
        if (_playerCanMove)
        {
            _isGrounded = Physics.Raycast(playerCamera.transform.position, Vector3.down, out hit, transform.localScale.y + 0.25f, _groundLayer, QueryTriggerInteraction.Ignore);
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z);
            RotatePlayer();
            
            rb.linearDamping = _playerState == PlayerState.Sliding ? 1f : 0f;
            
            if (_knockbackCooldown > 0)
            {
                _knockbackCooldown -= Time.deltaTime;
            }
            else
            {
                
                _desiredMoveDirection = (new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z) * GameManager.Instance.inputHandler._moveDirection.y + playerCamera.transform.right * GameManager.Instance.inputHandler._moveDirection.x).normalized;
                _desiredMoveDirection = new Vector3(_desiredMoveDirection.x, 0, _desiredMoveDirection.z).normalized;
                
                switch (_playerState)
                {
                    case PlayerState.Standing:
                        Standing();
                        break;
                    case PlayerState.Running:
                        Running();
                        break;
                    case PlayerState.Falling:
                        Falling();
                        break;
                    case PlayerState.Crouching:
                        Crouching();
                        break;
                    case PlayerState.Sliding:
                        Sliding();
                        break;
                    case PlayerState.Jumping:
                        Jumping();
                        break;
                }
                
            }
        }
    }

    public void DisablePlayerMovement(bool state)
    {
        _playerCanMove = !state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }
    
    public void SetRespawn(Transform location)
    {
        _respawnLocation = location;
    }
    
    public void ResetPlayer()
    {
        gameObject.SetActive(false);
        _playerCanMove = false;

        _xRotation = 0f;
        _yRotation = 0f;
        
        _canWallJump = false;
        rb.linearVelocity = Vector3.zero;
        
        _playerState = PlayerState.Falling;
        
        _lastWallJumped = null;
        transform.position = _respawnLocation != null ? _respawnLocation.position : new Vector3(-0.552f, 1f, -65f);

        _playerCanMove = true;
        gameObject.SetActive(true);
        
    }

    public void SetKnockback(Vector3 force)
    {
        _knockbackCooldown = RESET_COOLDOWN;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(force, ForceMode.Impulse);
    }

    void AdjustPlayerSpeed(float desiredSpeed)
    {
        if (_desiredMoveDirection != Vector3.zero)
        {
            if (_currentSpeed <= desiredSpeed)
            {
                _currentSpeed += desiredSpeed * _speedRampUp * Time.deltaTime;
            }
            else if (_currentSpeed > desiredSpeed)
            {
                _currentSpeed -= desiredSpeed * _speedRampDown * Time.deltaTime;
            }
        }
        else
        {
            if (_currentSpeed > 0f)
            {
                _currentSpeed -= desiredSpeed * _speedRampDown * Time.deltaTime;
                return;
            }
            _currentSpeed = 0f;
        }
    }
    
    void ProcessMovement(float desiredSpeed)
    {
        AdjustPlayerSpeed(desiredSpeed);
        
        rb.linearVelocity = new Vector3(
            _desiredMoveDirection.x * _currentSpeed,
            rb.linearVelocity.y,
            _desiredMoveDirection.z * _currentSpeed
        );
    }

    private void RotatePlayer()
    {
        float lookX = GameManager.Instance.inputHandler._lookDirection.x * Time.unscaledDeltaTime * _sensitivity;
        float lookY = GameManager.Instance.inputHandler._lookDirection.y * Time.unscaledDeltaTime * _sensitivity;

        _yRotation += lookX;
        _xRotation -= lookY;
        
        _xRotation = Mathf.Clamp(_xRotation, -_yRotationLimit, _yRotationLimit);
        
        playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
    }
    
    void HandleStand()
    {
        transform.localScale = new Vector3(transform.localScale.x, 2f, transform.localScale.z);
    }

    void HandleCrouch()
    {
        if (GameManager.Instance.inputHandler._crouch) 
            transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        else
            if (!Physics.Raycast(playerCamera.transform.position, Vector3.up, 1.25f))
            {
                transform.localScale = new Vector3(transform.localScale.x, 2f, transform.localScale.z);
            }
    }

    void HandleJump()
    {
        _hasJumped = true;
    }

    void HandleSlide()
    {
        _hasSlid = true;
    }

    void CheckFalling()
    {
        if (rb.linearVelocity.y < 0f && !_isGrounded)
        {
            _playerState = PlayerState.Falling;
        }
    }

    void CheckIfMoving()
    {
        RaycastHit hitUpper;
        RaycastHit hitLower;
        
        if (_isGrounded && _playerState != PlayerState.Falling && _playerState != PlayerState.Jumping)
        {
            if (GameManager.Instance.inputHandler._moveDirection.magnitude > 0 
                && !Physics.Raycast(playerCamera.transform.position,  _desiredMoveDirection, out hitUpper, 1f,  _groundLayer, QueryTriggerInteraction.Ignore)
                && !Physics.Raycast(new Vector3(playerCamera.transform.position.x, transform.position.y + .05f, playerCamera.transform.position.z),  _desiredMoveDirection, out hitLower, 1f, _groundLayer, QueryTriggerInteraction.Ignore)
               ) {
                _playerState = PlayerState.Running;
            }
            else
            {
                
                _playerState = PlayerState.Standing;
            }
        }
    }

    void Standing()
    {
        ProcessMovement(_playerSpeed);
        
        HandleStand();
        CheckIfMoving();
        
        _canWallJump = false;
        
        //Jumping Logic
        if (GameManager.Instance.inputHandler._jump)
        {
            _playerState = PlayerState.Jumping; 
            HandleJump();
        }
        
        if(GameManager.Instance.inputHandler._crouch) _playerState = PlayerState.Crouching;
    }
    
    void Running()
    {
        ProcessMovement(_playerSpeed);
        CheckIfMoving();
        CheckFalling();
        
        if (GameManager.Instance.inputHandler._jump)
        {
            _playerState = PlayerState.Jumping; 
            HandleJump();
        }

        if (GameManager.Instance.inputHandler._crouch)
        {
            HandleSlide();
            _playerState = PlayerState.Sliding;
        }
    }
    
    void Crouching()
    {
        ProcessMovement(_playerSpeed / 2);
        
        HandleCrouch();
        if (!GameManager.Instance.inputHandler._crouch)
        {
            if (!Physics.Raycast(playerCamera.transform.position, Vector3.up, 2.25f))
            {
                transform.localScale = new Vector3(transform.localScale.x, 2f, transform.localScale.z);
                _playerState = PlayerState.Standing;
            }
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        }
    }

    void Sliding()
    {
        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        
        if(!_isGrounded) rb.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);

        if (GameManager.Instance.inputHandler._jump) _playerState = PlayerState.Jumping;

        if (rb.linearVelocity.magnitude <= 0.25f)
        {
            if (!GameManager.Instance.inputHandler._crouch)
            {
                _playerState = PlayerState.Standing;
            }
            else
            {
                _playerState = PlayerState.Crouching;
            }
        }
        
        if (GameManager.Instance.inputHandler._jump)
        {
            _playerState = PlayerState.Jumping; 
            HandleJump();
        }
        
        if (!GameManager.Instance.inputHandler._crouch)
        {
            if (!Physics.Raycast(transform.position, Vector3.up, 1f))
            {
                HandleStand();
                _playerState = PlayerState.Standing;
            }
            else
            {
                _playerState = PlayerState.Crouching;
            }
        }
    }

    void Jumping()
    {
        ProcessMovement(_playerSpeed);
        
        CheckFalling();
        HandleCrouch();
        HandleWallJump();
    }
    
    void Falling()
    {
        ProcessMovement(_playerSpeed);
        HandleCrouch();
        
        HandleWallJump();
        
        if (_isGrounded)
        {
            _lastWallJumped = null;
            
            if (GameManager.Instance.inputHandler._crouch)
            {
                _playerState = PlayerState.Crouching;
            }
            else
            {
                _playerState = PlayerState.Standing;
            }
        }
        else
        {
            rb.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);
        }
        
    }

    void HandleWallJump()
    {
        const float wallrunRange = 1.25f;
        
        Ray leftRay = new Ray(playerCamera.transform.position, -playerCamera.transform.right);
        Ray rightRay = new Ray(playerCamera.transform.position, playerCamera.transform.right);
        
        RaycastHit hit;
        if (Physics.Raycast(leftRay, out hit, wallrunRange) || Physics.Raycast(rightRay, out hit, wallrunRange))
        {
            _canWallJump = true;
            
            Debug.DrawRay(playerCamera.transform.position, transform.TransformDirection(-playerCamera.transform.right) * hit.distance, Color.yellow);
            Debug.DrawRay(playerCamera.transform.position, transform.TransformDirection(playerCamera.transform.right) * hit.distance, Color.yellow);
            
            if (GameManager.Instance.inputHandler._jump && !_isGrounded && !hit.collider.CompareTag("Enemy"))
            {
                _playerState = PlayerState.Jumping;
                
                if (hit.transform != _lastWallJumped)
                {
                    rb.linearVelocity = Vector3.zero;
                    Vector3 forceToApply = transform.up * _wallJumpUpForce + playerCamera.transform.forward * _wallJumpSideForce;
                    
                    rb.AddForce(
                        new Vector3(
                            forceToApply.x,
                            Mathf.Clamp(forceToApply.y, 30f, 80f),
                            forceToApply.z
                        ), 
                        ForceMode.Impulse
                    );
                    
                    _lastWallJumped = hit.transform;
                }
            }
        }
        else
        {
            _canWallJump = false;
        }
        
    }

    private void FixedUpdate()
    {
        if (_hasJumped)
        {
            rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
            _hasJumped = false;
        }

        if (_hasSlid)
        {
            rb.AddForce(new Vector3(rb.linearVelocity.x * _slideForce, rb.linearVelocity.y, rb.linearVelocity.z * _slideForce), ForceMode.Impulse);
            _hasSlid = false;
        }
    }
}
