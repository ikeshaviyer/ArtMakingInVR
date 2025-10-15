using UnityEngine;
using System;

namespace VRArtMaking
{
    public class DiscController : MonoBehaviour
    {
        [Header("Throw Settings")]
        [SerializeField] private float launchForce = 10f;
        [SerializeField] private GameObject aimLineObject;
        
        [Header("Return Settings")]
        [SerializeField] private float returnSpeed = 15f;
        [SerializeField] private float flyTimeout = 10f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        public static event Action OnDiscFirstGrabbed;
        
        private Rigidbody rb;
        private bool hasBeenGrabbedBefore = false;
        private Vector3 spawnPosition;
        
        // Component references
        private DiscHomingController homingController;
        private DiscFlyingEffects flyingEffects;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            homingController = GetComponent<DiscHomingController>();
            flyingEffects = GetComponent<DiscFlyingEffects>();
            
            // Set spawn position
            spawnPosition = transform.position;
            transform.position = spawnPosition;
        }
        
        // PUBLIC METHODS - Call these via Unity Events
        
        public void Grab()
        {
            // Stop homing if active
            homingController.StopHoming();
            
            // Make disc face the same direction as the aim line
            if (aimLineObject != null)
            {
                transform.rotation = aimLineObject.transform.rotation;
            }
            
            // Check if this is the first grab
            if (!hasBeenGrabbedBefore)
            {
                hasBeenGrabbedBefore = true;
                OnDiscFirstGrabbed?.Invoke();
                
                if (showDebugInfo)
                {
                    Debug.Log("Disc grabbed for the first time - starting game!");
                }
            }
            
            if (showDebugInfo)
            {
                Debug.Log("Disc grabbed");
            }
        }
        
        public void Throw()
        {
            // Launch the disc straight forward in the direction it's facing
            rb.AddForce(transform.forward * launchForce, ForceMode.Impulse);
            
            // Start flying effects
            flyingEffects.StartFlyingFX();
            
            // Start fly timeout coroutine
            StartCoroutine(FlyTimeout());
            
            if (showDebugInfo)
            {
                Debug.Log("Disc thrown");
            }
        }
        
        public void Return()
        {
            // Stop all coroutines (including fly timeout)
            StopAllCoroutines();
            
            // Stop homing if active
            homingController.StopHoming();
            
            // Stop all movement for smooth return
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            // Start return coroutine
            StartCoroutine(ReturnToSpawn());
            
            if (showDebugInfo)
            {
                Debug.Log("Disc returning to spawn");
            }
        }
        
        private System.Collections.IEnumerator ReturnToSpawn()
        {
            float distanceToSpawn = Vector3.Distance(transform.position, spawnPosition);
            
            while (distanceToSpawn > 0.1f)
            {
                // Smoothly move towards spawn position
                Vector3 directionToSpawn = (spawnPosition - transform.position).normalized;
                Vector3 newPosition = Vector3.MoveTowards(transform.position, spawnPosition, returnSpeed * Time.deltaTime);
                transform.position = newPosition;
                
                // Rotate to face spawn location
                Quaternion targetRotation = Quaternion.LookRotation(directionToSpawn);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                
                distanceToSpawn = Vector3.Distance(transform.position, spawnPosition);
                yield return null;
            }
            
            // Complete return
            transform.position = spawnPosition;
            transform.rotation = Quaternion.identity;
            
            // Stop flying effects
            flyingEffects.StopFlyingFX();
            
            if (showDebugInfo)
            {
                Debug.Log("Disc returned to spawn");
            }
        }
        
        private System.Collections.IEnumerator FlyTimeout()
        {
            // Wait for the specified timeout duration
            yield return new WaitForSeconds(flyTimeout);
            
            // Timeout reached - automatically return the disc
            if (showDebugInfo)
            {
                Debug.Log($"Fly timeout reached ({flyTimeout}s) - auto returning disc");
            }
            
            // Stop homing if active
            homingController.StopHoming();
            
            // Stop all movement for smooth return
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            // Start return coroutine
            StartCoroutine(ReturnToSpawn());
        }
        
        public void SetSpawnLocation(Vector3 position)
        {
            spawnPosition = position;
        }
        
        public void SetLaunchForce(float force)
        {
            launchForce = force;
        }
        
        public void SetReturnSpeed(float speed)
        {
            returnSpeed = speed;
        }
        
        public void SetFlyTimeout(float timeout)
        {
            flyTimeout = timeout;
        }
    }
}


