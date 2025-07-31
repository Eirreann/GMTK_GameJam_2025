using System;
using System.Collections;
using System.Collections.Generic;
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
    
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 20f;
    
    [Header("Look Rotation Values")]
    [Range(0.1f, 9f)][SerializeField] private float _sensitivity = 2f;
    [Range(0f, 90f)][SerializeField] private float _yRotationLimit = 88f;
    Vector2 rotation = Vector2.zero;

    [Header("Is Doing?")]
    [SerializeField] private bool isSliding = false;
    [SerializeField] private bool isGrounded = true;
    [SerializeField] private bool isJumping = false;
    
    [Header("Player State Machine")]
    [SerializeField] private PlayerState _playerState = PlayerState.Standing;
    
    private enum PlayerState
    {
        Standing,
        Crouching,
        Sprinting,
        Sliding,
        Running,
        Jumping
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    
    void Update()
    {
        if (transform.position.y < -2 || transform.position.y > 10)
        {
            transform.position = new Vector3(0, 3, 0);
            rb.linearVelocity = Vector3.zero;
        }
        
        playerCamera.transform.rotation = Quaternion.Euler(Mathf.Clamp(rb.linearVelocity.x, -0.2f, 0.2f), 0, 0);
            
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
            case PlayerState.Crouching:
                Crouching();
                break;
            case PlayerState.Sprinting:
                Sprinting();
                break;
            case PlayerState.Sliding:
                Sliding();
                break;
            case PlayerState.Jumping:
                Jumping();
                break;
        }
    }

    void Standing()
    {
        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        
        //Jumping Logic
        if (GameManager.Instance.inputHandler._isJumping) _playerState = PlayerState.Jumping;
        
        if(GameManager.Instance.inputHandler._crouch) _playerState = PlayerState.Crouching;

        if (GameManager.Instance.inputHandler._moveDirection.magnitude > 0)
        {
            _playerState = PlayerState.Running;
        }
    }
    
    void Crouching()
    {
        RaycastHit hit;
        rb.linearVelocity = new Vector3(
            desiredMoveDirection.x * currentSpeed / 2,
            rb.linearVelocity.y,
            desiredMoveDirection.z * currentSpeed / 2
        );
        
        transform.localScale = new Vector3(transform.localScale.x, .5f, transform.localScale.z);
        if (!GameManager.Instance.inputHandler._crouch)
        {
            if (!Physics.Raycast(transform.position, Vector3.up, out hit, 1.5f))
            {
                transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
                _playerState = PlayerState.Standing;
            }
        }
    }

    void Running()
    {
        currentSpeed = playerSpeed;
        rb.linearVelocity = new Vector3(
            desiredMoveDirection.x * currentSpeed,
            rb.linearVelocity.y,
            desiredMoveDirection.z * currentSpeed
        );
        
        if(GameManager.Instance.inputHandler._run) _playerState = PlayerState.Sprinting;
        
        if (GameManager.Instance.inputHandler._isJumping) _playerState = PlayerState.Jumping;

        if (GameManager.Instance.inputHandler._crouch)
        {
            rb.AddForce(new Vector3(rb.linearVelocity.x * 5f, rb.linearVelocity.y, rb.linearVelocity.z * 5f), ForceMode.Impulse);
            _playerState = PlayerState.Sliding;
        }
        if (GameManager.Instance.inputHandler._moveDirection.magnitude < 0.1f) _playerState = PlayerState.Standing;
    }
    
    void Sprinting()
    {
        currentSpeed = playerSpeed * sprintMultiplier;
        rb.linearVelocity = new Vector3(
            desiredMoveDirection.x * currentSpeed,
            rb.linearVelocity.y,
            desiredMoveDirection.z * currentSpeed
        );
        
        if (GameManager.Instance.inputHandler._crouch)
        {
            rb.AddForce(new Vector3(rb.linearVelocity.x * 5f, rb.linearVelocity.y, rb.linearVelocity.z * 5f), ForceMode.Impulse);
            _playerState = PlayerState.Sliding;
        }
        
        if (GameManager.Instance.inputHandler._isJumping) _playerState = PlayerState.Jumping;
        if(!GameManager.Instance.inputHandler._run) _playerState = PlayerState.Running;
    }
    
    void Sliding()
    {
        RaycastHit hit;
        transform.localScale = new Vector3(transform.localScale.x, .5f, transform.localScale.z);

        if (GameManager.Instance.inputHandler._isJumping) _playerState = PlayerState.Jumping;
        
        if (rb.linearVelocity.magnitude < 0.1f) _playerState = PlayerState.Crouching;
        
        if (!GameManager.Instance.inputHandler._crouch)
        {
            if (!Physics.Raycast(transform.position, Vector3.up, out hit, 1.5f))
            {
                transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
                _playerState = PlayerState.Standing;
            }
        }
    }

    void Jumping()
    {
        if(!isJumping) rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJumping = true;
        
        RaycastHit hit;
        if (rb.linearVelocity.y < 0)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
            {
                if (GameManager.Instance.inputHandler._crouch)
                {
                    _playerState = PlayerState.Sliding;
                    isJumping = false;
                }
                else
                {
                    _playerState = PlayerState.Standing;
                    isJumping = false;
                }
                    
            }
        }
    }

    void FixedUpdate()
    {
        
    }
}
