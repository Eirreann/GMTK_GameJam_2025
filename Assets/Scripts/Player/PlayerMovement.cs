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
    [SerializeField] private Transform leanTransform;
    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float currentSpeed = 2f;
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] private float sprintMultiplier = 1.1f;
    [SerializeField] private Vector3 desiredMoveDirection;

    private bool playerCanMove;
    
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 20f;
    
    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpUpForce = 20f;
    [SerializeField] private float wallJumpSideForce = 20f;
    
    [Header("Look Rotation Values")]
    [Range(0.1f, 9f)][SerializeField] private float _sensitivity = 2f;
    [Range(0f, 90f)][SerializeField] private float _yRotationLimit = 88f;
    
    private float _xRotation = 0f;
    private float _yRotation = 0f;

    [Header("Is Doing?")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool canWallJump;
    [SerializeField] private bool isJumping;
    
    [Header("Player State Machine")]
    [SerializeField] private PlayerState _playerState = PlayerState.Standing;

    [SerializeField] private Transform lastWallJumped;

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
        
        rb.linearDamping = 1f;
    }

    void Update()
    {
        if (playerCanMove) RotatePlayer();
        switch (_playerState)
        {
            case PlayerState.Sliding:
                Debug.Log("No movement during slide.");
                break;
            case PlayerState.Crouching:
                ProcessMovement(playerSpeed / 2);
                break;
            default:
                ProcessMovement(playerSpeed);
                break;
        }
        
        if (playerCanMove)
        {
            if (_knockbackCooldown > 0)
            {
                _knockbackCooldown -= Time.deltaTime;
            }
            else
            {
                
                desiredMoveDirection = (new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z) * GameManager.Instance.inputHandler._moveDirection.y + playerCamera.transform.right * GameManager.Instance.inputHandler._moveDirection.x).normalized;
                desiredMoveDirection = new Vector3(desiredMoveDirection.x, 0, desiredMoveDirection.z).normalized;
                
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
        
        canWallJump = false;
        rb.linearVelocity = Vector3.zero;
        _playerState = PlayerState.Standing;

        lastWallJumped = null;
        
        transform.position = _respawnLocation != null ? _respawnLocation.position : new Vector3(-0.552f, 1f, -65f);

        rb.linearDamping = 9999f;
        
        isJumping = false;
        gameObject.SetActive(true);
    }

    public void SetKnockback(Vector3 force)
    {
        _knockbackCooldown = RESET_COOLDOWN;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(force, ForceMode.Impulse);
    }
    
    
    
    void ProcessMovement(float desiredSpeed)
    {
        currentSpeed = desiredSpeed;

        if (GameManager.Instance.Player.wallHandler.IsDrawing) rb.linearDamping = 2f;
        
        rb.AddForce(desiredMoveDirection * (currentSpeed * 10f), ForceMode.Force);
        
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (flatVel.magnitude > currentSpeed)
        {
            Vector3 limitVel = flatVel.normalized * currentSpeed;
            rb.linearVelocity = new Vector3(limitVel.x, rb.linearVelocity.y, limitVel.z);
        }
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
        if (GameManager.Instance.inputHandler._crouch) transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
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
            && !Physics.Raycast(playerCamera.transform.position,  desiredMoveDirection, out hit, 1f, 0, QueryTriggerInteraction.Ignore)
            && !Physics.Raycast(new Vector3(playerCamera.transform.position.x, transform.position.y + .25f, playerCamera.transform.position.z),  desiredMoveDirection, out hit, 1f, 0, QueryTriggerInteraction.Ignore)
        )
        {
            Debug.DrawRay(playerCamera.transform.position, desiredMoveDirection * hit.distance, Color.red);
            _playerState = PlayerState.Running;
        }
        else
        {
            _playerState = PlayerState.Standing;
        }
    }

    void Standing()
    {
        rb.linearDamping = 7f;
        HandleStand();
        CheckIfMoving();
        
        canWallJump = false;
        
        //Jumping Logic
        if (GameManager.Instance.inputHandler._jump) _playerState = PlayerState.Jumping;
        
        if(GameManager.Instance.inputHandler._crouch) _playerState = PlayerState.Crouching;
    }
    
    void Crouching()
    {
        rb.linearDamping = 2f;
        
        RaycastHit hit;
        HandleCrouch();
        
        if (!GameManager.Instance.inputHandler._crouch)
        {
            if (!Physics.Raycast(playerCamera.transform.position, Vector3.up, out hit, 2.25f))
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
        rb.linearDamping = 1f;
        
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
        rb.linearDamping = 0.25f;
        
        RaycastHit hit;
        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        
        rb.AddForce(-rb.linearVelocity, ForceMode.Force);

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
            if (!Physics.Raycast(transform.position, Vector3.up, out hit, 2.25f))
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
        rb.linearDamping = 0f;

        if (!isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }

        CheckFalling();
        HandleCrouch();
        HandleWallJump();
    }
    
    void Falling()
    {
        rb.linearDamping = 0.2f;
        
        HandleCrouch();
        HandleWallJump();
        
        RaycastHit fallHit;
        if (Physics.Raycast(transform.position, Vector3.down, out fallHit, .05f))
        {
            if (GameManager.Instance.inputHandler._crouch)
            {
                _playerState = PlayerState.Crouching;
            }
            else
            {
                _playerState = PlayerState.Standing;
            }
            
            isJumping = false;
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
            canWallJump = true;
            
            Debug.DrawRay(playerCamera.transform.position, transform.TransformDirection(-playerCamera.transform.right) * hit.distance, Color.yellow);
            Debug.DrawRay(playerCamera.transform.position, transform.TransformDirection(playerCamera.transform.right) * hit.distance, Color.yellow);
            
            if (GameManager.Instance.inputHandler._jump && isJumping)
            {
                _playerState = PlayerState.Jumping;
                if (hit.transform != lastWallJumped)
                {
                    rb.linearVelocity = Vector3.zero;
                    Vector3 forceToApply = transform.up * wallJumpUpForce + playerCamera.transform.forward * wallJumpSideForce;
                    rb.AddForce(
                        new Vector3(
                            forceToApply.x,
                            Mathf.Clamp(forceToApply.y, 45f, 80f),
                            forceToApply.z
                        ), 
                        ForceMode.Impulse
                    );
                
                    lastWallJumped = hit.transform;
                }
            }
        }
        else
        {
            canWallJump = false;
        }
        
    }

    void FixedUpdate()
    {
        
    }
}
