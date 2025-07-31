using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] private float jumpForce = 20f;
    
    [Range(0.1f, 9f)][SerializeField] private float _sensitivity = 2f;
    [Range(0f, 90f)][SerializeField] private float _yRotationLimit = 88f;
    Vector2 rotation = Vector2.zero;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotation.x = GameManager.Instance.inputHandler._lookDirection.x * _sensitivity;
        rotation.y = GameManager.Instance.inputHandler._lookDirection.y * _sensitivity;

        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);
        
        transform.localRotation = xQuat * yQuat;
    }

    void FixedUpdate()
    {
        
    }
}
