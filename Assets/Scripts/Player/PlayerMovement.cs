using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float currentSpeed = 2f;
    
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] private float sprintMultiplier = 1.1f;
    
    [SerializeField] private float jumpForce = 20f;
    
    [Range(0.1f, 9f)][SerializeField] private float _sensitivity = 2f;
    [Range(0f, 90f)][SerializeField] private float _yRotationLimit = 88f;
    Vector2 rotation = Vector2.zero;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private Vector3 desiredMoveDirection;

    private bool jump;
    private bool isGrounded = true;

    private float gravity = 1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        rotation.x += GameManager.Instance.inputHandler._lookDirection.x * _sensitivity;
        rotation.y += GameManager.Instance.inputHandler._lookDirection.y * _sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -_yRotationLimit, _yRotationLimit);

        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);
        
        playerCamera.transform.localRotation = xQuat * yQuat;

        if (GameManager.Instance.inputHandler._isJumping) jump = true;

        if (transform.position.y < -2)
        {
            transform.position = new Vector3(0, 3, 0);
        }

        if (GameManager.Instance.inputHandler._run)
        {
            currentSpeed = playerSpeed * sprintMultiplier;
        }
        else
        {
            currentSpeed = playerSpeed;
        }

        if (GameManager.Instance.inputHandler._crouch)
        {
            transform.localScale = new Vector3(transform.localScale.x, .5f, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        }
    }

    void FixedUpdate()
    {
        if (isGrounded)
        {
            desiredMoveDirection = (new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z) * GameManager.Instance.inputHandler._moveDirection.y + playerCamera.transform.right * GameManager.Instance.inputHandler._moveDirection.x).normalized;
            rb.linearVelocity = new Vector3(
                desiredMoveDirection.x * currentSpeed,
                rb.linearVelocity.y,
                desiredMoveDirection.z * currentSpeed
            );
            
            if(jump) rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            jump = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}
