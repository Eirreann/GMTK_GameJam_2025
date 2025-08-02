using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
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
    Vector2 rotation = Vector2.zero;

    [Header("Is Doing?")]
    [SerializeField] private bool isSliding = false;
    [SerializeField] private bool isGrounded = true;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isFalling = false;
    
    [Header("Player State Machine")]
    [SerializeField] private PlayerState _playerState = PlayerState.Standing;

    [SerializeField] private Transform lastWallJumped;

    private const float RESET_COOLDOWN = 5f;
    
    private Transform _respawnLocation;
    private float _resetCooldown = 0f;
    
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

    public void DisablePlayerMovement()
    {
        playerCanMove = false;
        Cursor.lockState = CursorLockMode.None;
    }
    
    public void SetRespawn(Transform location)
    {
        _respawnLocation = location;
    }
    
    public void ResetPlayer()
    {
        gameObject.SetActive(false);
        
        rb.linearVelocity = Vector3.zero;
        _playerState = PlayerState.Standing;
        
        transform.position = _respawnLocation != null ? _respawnLocation.position : new Vector3(-0.552f, 1f, -65f);
        
        isJumping = false;
        gameObject.SetActive(true);
    }
    
    void Update()
    {
        if (transform.position.y < -2 || transform.position.y > 25)
        {
            ResetPlayer();
            GameManager.Instance.CurrentLevel.ReturnRope();
        }
        
        playerCamera.transform.rotation = Quaternion.Euler(Mathf.Clamp(rb.linearVelocity.x, -0.2f, 0.2f), 0, 0);

        if (playerCanMove)
        {
            //Look Logic
            rotation.x += GameManager.Instance.inputHandler._lookDirection.x * _sensitivity;
            rotation.y += GameManager.Instance.inputHandler._lookDirection.y * _sensitivity;
            rotation.y = Mathf.Clamp(rotation.y, -_yRotationLimit, _yRotationLimit);

            var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

            playerCamera.transform.localRotation = xQuat * yQuat;
        
            desiredMoveDirection = (new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z) * GameManager.Instance.inputHandler._moveDirection.y + playerCamera.transform.right * GameManager.Instance.inputHandler._moveDirection.x).normalized;
        
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
    
    void ProcessMovement(float desiredSpeed)
    {
        currentSpeed = desiredSpeed;
        rb.linearVelocity = new Vector3(
            desiredMoveDirection.x * currentSpeed,
            rb.linearVelocity.y,
            desiredMoveDirection.z * currentSpeed
        );
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
        if (rb.linearVelocity.y < 0)
        {
            _playerState = PlayerState.Falling;
        }
    }

    void Standing()
    {
        ProcessMovement(playerSpeed);
        HandleStand();
        CheckFalling();
        
        //Jumping Logic
        if (GameManager.Instance.inputHandler._jump) _playerState = PlayerState.Jumping;
        
        if(GameManager.Instance.inputHandler._crouch) _playerState = PlayerState.Crouching;

        if (GameManager.Instance.inputHandler._moveDirection.magnitude > 0 && rb.linearVelocity.magnitude > (playerSpeed / 2))
        {
            _playerState = PlayerState.Running;
        }

    }
    
    void Crouching()
    {
        RaycastHit hit;
        ProcessMovement(playerSpeed / 2);
        HandleCrouch();
        
        if (!GameManager.Instance.inputHandler._crouch)
        {
            if (!Physics.Raycast(transform.position, Vector3.up, out hit, 1.5f))
            {
                transform.localScale = new Vector3(transform.localScale.x, 2f, transform.localScale.z);
                _playerState = PlayerState.Standing;
            }
        }
    }

    void Running()
    {
        ProcessMovement(playerSpeed);
        CheckFalling();
        
        if (GameManager.Instance.inputHandler._jump) _playerState = PlayerState.Jumping;

        if (GameManager.Instance.inputHandler._crouch)
        {
            rb.AddForce(new Vector3(rb.linearVelocity.x * 5f, rb.linearVelocity.y, rb.linearVelocity.z * 5f), ForceMode.Impulse);
            _playerState = PlayerState.Sliding;
        }
        
        if (GameManager.Instance.inputHandler._moveDirection.magnitude < 0.1f) _playerState = PlayerState.Standing;
    }
    
    void Sliding()
    {
        RaycastHit hit;
        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);

        if (GameManager.Instance.inputHandler._jump) _playerState = PlayerState.Jumping;

        if (rb.linearVelocity.magnitude <= 0)
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
            if (!Physics.Raycast(transform.position, Vector3.up, out hit, 1.5f))
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
        ProcessMovement(playerSpeed);

        if (!isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }

        CheckFalling();
        HandleCrouch();
        
        Ray leftRay = new Ray(playerCamera.transform.position, -playerCamera.transform.right);
        Ray rightRay = new Ray(playerCamera.transform.position, playerCamera.transform.right);
        
        RaycastHit hit;
        if (GameManager.Instance.inputHandler._jump && isJumping)
        {
            const float wallrunRange = 3f;
            if (Physics.Raycast(leftRay, out hit, wallrunRange) || Physics.Raycast(rightRay, out hit, wallrunRange))
            {
                if (hit.transform != lastWallJumped)
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow); 
                    
                    rb.linearVelocity = Vector3.zero;
                    Vector3 forceToApply = transform.up * wallJumpUpForce + playerCamera.transform.forward * wallJumpSideForce;
                    rb.AddForce(forceToApply, ForceMode.Impulse);
                    
                    lastWallJumped = hit.transform;
                }
            }
        }
    }
    
    void Falling()
    {
        ProcessMovement(playerSpeed);
        HandleCrouch();
        
        RaycastHit fallHit;
        if (Physics.Raycast(transform.position, Vector3.down, out fallHit, 1.25f))
        {
            Debug.DrawRay(fallHit.point, fallHit.normal, Color.red);
            if (GameManager.Instance.inputHandler._crouch)
            {
                lastWallJumped = null;

                _playerState = PlayerState.Crouching;
                isJumping = false;
            }
            else
            {
                lastWallJumped = null;

                _playerState = PlayerState.Standing;
                isJumping = false;
            }
        }
    }

    void FixedUpdate()
    {
        
    }
}
