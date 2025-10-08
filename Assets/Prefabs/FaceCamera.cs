using UnityEngine;

namespace VRArtMaking
{
    public class FaceCamera : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool reverseDirection = false;
        [SerializeField] private bool lockX = false;
        [SerializeField] private bool lockY = false;
        [SerializeField] private bool lockZ = false;
        
        private Camera mainCamera;
        
        private void Start()
        {
            // Get the main camera
            mainCamera = Camera.main;
            
            if (mainCamera == null)
            {
                Debug.LogWarning("FaceCamera: No main camera found in scene!");
            }
        }
        
        private void LateUpdate()
        {
            if (mainCamera == null)
                return;
            
            // Get direction to camera
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            
            // Reverse if needed (useful for UI)
            if (reverseDirection)
            {
                directionToCamera = -directionToCamera;
            }
            
            // Apply locks
            if (lockX) directionToCamera.x = 0;
            if (lockY) directionToCamera.y = 0;
            if (lockZ) directionToCamera.z = 0;
            
            // Only rotate if we have a valid direction
            if (directionToCamera != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(directionToCamera);
            }
        }
    }
}
