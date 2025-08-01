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
    
    [Header("Player State Machine")]
    [SerializeField] private PlayerState _playerState = PlayerState.Standing;

    [SerializeField] private Transform lastWallJumped;
    
    private enum PlayerState
    {
        Standing,
        Crouching,
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
        if (transform.position.y < -2 || transform.position.y > 25)
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
        currentSpeed = playerSpeed;
        
        rb.linearVelocity = new Vector3(
            desiredMoveDirection.x * currentSpeed,
            rb.linearVelocity.y,
            desiredMoveDirection.z * currentSpeed
        );
        
        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        
        //Jumping Logic
        if (GameManager.Instance.inputHandler._isJumping) _playerState = PlayerState.Jumping;
        
        if(GameManager.Instance.inputHandler._crouch) _playerState = PlayerState.Crouching;

        if (GameManager.Instance.inputHandler._moveDirection.magnitude > 0 && rb.linearVelocity.magnitude > (playerSpeed / 2))
        {
            _playerState = PlayerState.Running;
        }
    }
    
    void Crouching()
    {
        currentSpeed = playerSpeed / 2;
        
        RaycastHit hit;
        rb.linearVelocity = new Vector3(
            desiredMoveDirection.x * currentSpeed,
            rb.linearVelocity.y,
            desiredMoveDirection.z * currentSpeed
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
        
        if (GameManager.Instance.inputHandler._isJumping) _playerState = PlayerState.Jumping;

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
        transform.localScale = new Vector3(transform.localScale.x, .5f, transform.localScale.z);

        if (GameManager.Instance.inputHandler._isJumping) _playerState = PlayerState.Jumping;

        if (rb.linearVelocity.magnitude < 0.1)
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
                transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
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
        rb.linearVelocity = new Vector3(
            desiredMoveDirection.x * currentSpeed,
            rb.linearVelocity.y,
            desiredMoveDirection.z * currentSpeed
        );
        
        if(!isJumping) rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJumping = true;
        
        if(GameManager.Instance.inputHandler._crouch) transform.localScale = new Vector3(transform.localScale.x, .5f, transform.localScale.z);
        
        Ray leftRay = new Ray(playerCamera.transform.position, -playerCamera.transform.right);
        Ray rightRay = new Ray(playerCamera.transform.position, playerCamera.transform.right);
        
        RaycastHit HitInfo;
        if (GameManager.Instance.inputHandler._isJumping && isJumping)
        {
            if (Physics.Raycast(leftRay, out HitInfo, 2f) || Physics.Raycast(rightRay, out HitInfo, 2f))
            {
                if (HitInfo.transform != lastWallJumped)
                {
                    rb.linearVelocity = Vector3.zero;
                    Vector3 forceToApply = transform.up * wallJumpUpForce + playerCamera.transform.forward * wallJumpSideForce;
                    rb.AddForce(forceToApply, ForceMode.Impulse);
                    
                    lastWallJumped = HitInfo.transform;
                }
            }
        }

        StartCoroutine("JumpingRoutine");
    }

    IEnumerator JumpingRoutine()
    {
        RaycastHit hit;
        yield return new WaitForSeconds(0.2f);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
        {
            if (GameManager.Instance.inputHandler._crouch)
            {
                lastWallJumped = null;

                _playerState = PlayerState.Sliding;
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
