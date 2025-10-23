using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 2f;
    
    [Header("Input Settings")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    
    private void Update()
    {
        HandleMovement();
    }
    
    private void HandleMovement()
    {
        // Get input for X and Z movement
        float horizontal = Input.GetAxis("Horizontal"); // A/D keys
        float vertical = Input.GetAxis("Vertical");     // W/S keys
        
        // Calculate movement direction (only X and Z)
        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        
        // Normalize to prevent faster diagonal movement
        movement = movement.normalized;
        
        // Apply sprint multiplier if sprint key is held
        float currentSpeed = moveSpeed;
        if (Input.GetKey(sprintKey))
        {
            currentSpeed *= sprintMultiplier;
        }
        
        // Apply movement
        transform.Translate(movement * currentSpeed * Time.deltaTime, Space.World);
    }
}
