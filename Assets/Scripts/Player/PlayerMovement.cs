using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Game;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float _currentSpeed = 2f;
    [SerializeField] private float _playerSpeed = 2f;
    [SerializeField] private float _sprintMultiplier = 1.1f;
    [SerializeField] private Vector3 _desiredMoveDirection;

    private bool playerCanMove;
    
    [Header("Jumping")]
    [SerializeField] private float _jumpForce = 20f;
    
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
    [SerializeField] private bool _canWallJump;
    [SerializeField] private bool _isJumping;
    
    [Header("Player State Machine")]
    [SerializeField] private PlayerState _playerState = PlayerState.Standing;

    [SerializeField] private Transform _lastWallJumped;

    private const float RESET_COOLDOWN = 1f;
    
    private Transform _respawnLocation;
    private float _knockbackCooldown = 0f;

    public float gravityScale = 5f;
    private Quaternion desiredRotation;
    
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
        playerCanMove = true;
        
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (playerCanMove) 
        
        
        if (playerCanMove)
        {
            if (_knockbackCooldown > 0)
            {
                _knockbackCooldown -= Time.deltaTime;
            }
            else
            {
                if(!_isGrounded) _isGrounded = Physics.Raycast(transform.position, Vector3.down, .05f);
                
                RotatePlayer();
                
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
        playerCanMove = !state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }
    
    public void SetRespawn(Transform location)
    {
        _respawnLocation = location;
    }
    
    public void ResetPlayer()
    {
        gameObject.SetActive(false);
        
        _canWallJump = false;
        rb.linearVelocity = Vector3.zero;
        _playerState = PlayerState.Standing;


        _lastWallJumped = null;
        
        transform.position = _respawnLocation != null ? _respawnLocation.position : new Vector3(-0.552f, 1f, -65f);
       
        _isJumping = false;
        gameObject.SetActive(true);
        
        playerCamera.transform.rotation = transform.rotation;
        playerCamera.transform.localRotation = transform.localRotation;
    }

    public void SetKnockback(Vector3 force)
    {
        _knockbackCooldown = RESET_COOLDOWN;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(force, ForceMode.Impulse);
    }
    
    void ProcessMovement(float desiredSpeed)
    {
        _currentSpeed = desiredSpeed;
        
        rb.linearVelocity = new Vector3(
            _desiredMoveDirection.x * _currentSpeed,
            rb.linearVelocity.y,
            _desiredMoveDirection.z * _currentSpeed
        );

        // _currentSpeed = desiredSpeed;
        //
        // if (GameManager.Instance.Player.wallHandler._isDrawing) rb.linearDamping = 2f;
        // rb.AddForce(_desiredMoveDirection * (_currentSpeed * 10f), ForceMode.Force);
        //
        // Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        //
        // if (flatVel.magnitude > _currentSpeed)
        // {
        //     Vector3 limitVel = flatVel.normalized * _currentSpeed;
        //     rb.linearVelocity = new Vector3(limitVel.x, rb.linearVelocity.y, limitVel.z);
        // }
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
            if (!Physics.Raycast(playerCamera.transform.position, Vector3.up, 2.25f))
            {
                transform.localScale = new Vector3(transform.localScale.x, 2f, transform.localScale.z);
            }
    }

    void HandleJump()
    {
        if (_isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            _isJumping = true;
            _isGrounded = false;
        }
    }

    void CheckFalling()
    {
        if (rb.linearVelocity.y < -0.1f)
        {
            _playerState = PlayerState.Falling;
        }
    }

    void CheckIfMoving()
    {
        RaycastHit hit;
        if (GameManager.Instance.inputHandler._moveDirection.magnitude > 0 
            && !Physics.Raycast(playerCamera.transform.position,  _desiredMoveDirection, out hit, 1f)
            && !Physics.Raycast(new Vector3(playerCamera.transform.position.x, transform.position.y + .25f, playerCamera.transform.position.z),  _desiredMoveDirection, out hit, 1f)
        )
        {
            Debug.DrawRay(playerCamera.transform.position, _desiredMoveDirection * hit.distance, Color.red);
            _playerState = PlayerState.Running;
        }
        else
        {
            _playerState = PlayerState.Standing;
        }
    }

    void Standing()
    {
        ProcessMovement(_playerSpeed);
        
        HandleStand();
        CheckIfMoving();
        
        _canWallJump = false;
        
        //Jumping Logic
        if (GameManager.Instance.inputHandler._jump) _playerState = PlayerState.Jumping;
        
        if(GameManager.Instance.inputHandler._crouch) _playerState = PlayerState.Crouching;
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

    void Running()
    {
        ProcessMovement(_playerSpeed);
        CheckIfMoving();
        CheckFalling();
        
        if (GameManager.Instance.inputHandler._jump) _playerState = PlayerState.Jumping;

        if (GameManager.Instance.inputHandler._crouch)
        {
            rb.AddForce(new Vector3(rb.linearVelocity.x * 5f, rb.linearVelocity.y, rb.linearVelocity.z * 5f), ForceMode.Impulse);
            _playerState = PlayerState.Sliding;
        }
    }
    
    void Sliding()
    {
        RaycastHit hit;
        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        
        rb.AddForce(-rb.linearVelocity * (_currentSpeed * 0.25f), ForceMode.Force);

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
        
        if (!GameManager.Instance.inputHandler._crouch)
        {
            if (!Physics.Raycast(transform.position, Vector3.up, out hit, 1f))
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
        HandleJump();
        HandleWallJump();
    }
    
    void Falling()
    {
        ProcessMovement(_playerSpeed);
        HandleCrouch();
        HandleWallJump();
        
        if (_isGrounded)
        {
            
            if (GameManager.Instance.inputHandler._crouch)
            {
                _playerState = PlayerState.Crouching;
            }
            else
            {
                _playerState = PlayerState.Standing;
            }
            
            _isJumping = false;
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
            
            if (GameManager.Instance.inputHandler._jump && !_isGrounded)
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

    void FixedUpdate()
    {
        if (_isJumping)
        {
            rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
            _isJumping = false;
        }
    }
}
