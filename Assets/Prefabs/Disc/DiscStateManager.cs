using UnityEngine;
using System;

namespace VRArtMaking
{
    public class DiscStateManager : MonoBehaviour
    {
        [Header("Throw Settings")]
        [SerializeField] private float throwVelocityThreshold = 2f;
        
        public static event Action OnDiscFirstGrabbed;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private Rigidbody rb;
        private bool hasBeenThrown = false;
        private bool isGrabbed = true; // Start in grabbed state
        private bool hasBeenGrabbedBefore = false; // Track if disc has been grabbed at least once
        private Vector3 spawnPosition;
        
        // Component references
        private DiscHomingHandler homingHandler;
        private DiscReturnHandler returnHandler;
        
        public bool IsGrabbed => isGrabbed;
        public bool HasBeenThrown => hasBeenThrown;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            homingHandler = GetComponent<DiscHomingHandler>();
            returnHandler = GetComponent<DiscReturnHandler>();
            
            // Set spawn position
            spawnPosition = transform.position;
            
            // Start at spawn position
            transform.position = spawnPosition;
            
            // Make sure it starts kinematic (grabbed state)
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        
        private void Start()
        {
            // Initialize return handler with spawn position
            if (returnHandler != null)
            {
                returnHandler.Initialize(spawnPosition);
            }
        }
        
        private void Update()
        {
            // Update grabbed state based on kinematic status
            bool currentlyKinematic = rb != null && rb.isKinematic;
            
            // Check if disc was just grabbed for the first time
            if (!hasBeenGrabbedBefore && currentlyKinematic)
            {
                hasBeenGrabbedBefore = true;
                OnDiscFirstGrabbed?.Invoke();
                
                if (showDebugInfo)
                {
                    Debug.Log("Disc grabbed for the first time - starting game!");
                }
            }
            
            isGrabbed = currentlyKinematic;
            
            // Check for throwing
            CheckForThrow();
            
            // Check if should return (time-based) - return no matter what if flying too long
            if (returnHandler != null && returnHandler.ShouldReturn(!isGrabbed, isGrabbed))
            {
                returnHandler.StartReturn();
                
                // Stop homing if it's happening - time's up!
                if (homingHandler != null && homingHandler.IsHoming)
                {
                    homingHandler.StopHoming();
                }
            }
        }
        
        private void CheckForThrow()
        {
            // Only check for throwing when disc is not grabbed and not returning
            if (!isGrabbed && (returnHandler == null || !returnHandler.IsReturning) && !hasBeenThrown && rb != null)
            {
                float currentVelocity = rb.velocity.magnitude;
                
                // If velocity is above threshold, consider it thrown
                if (currentVelocity > throwVelocityThreshold)
                {
                    OnObjectThrown();
                }
            }
        }
        
        private void OnObjectThrown()
        {
            if (!hasBeenThrown && !isGrabbed)
            {
                hasBeenThrown = true;
                
                if (showDebugInfo)
                {
                    Debug.Log($"Object thrown with velocity: {rb.velocity.magnitude}");
                }
                
                // Homing will start automatically when disc is flying
            }
        }
        
        public void OnTargetHit()
        {
            // Target was hit, start returning
            if (returnHandler != null)
            {
                returnHandler.StartReturn();
            }
        }
        
        public void OnHomingFailed()
        {
            // Homing failed, start returning
            if (returnHandler != null)
            {
                returnHandler.StartReturn();
            }
        }
        
        public void OnReturnComplete()
        {
            // Reset state for next throw
            hasBeenThrown = false;
            
            if (showDebugInfo)
            {
                Debug.Log("Disc ready for next throw");
            }
        }
        
        public void SetSpawnLocation(Vector3 position)
        {
            spawnPosition = position;
            if (returnHandler != null)
            {
                returnHandler.Initialize(spawnPosition);
            }
        }
        
        public void SetThrowVelocityThreshold(float threshold)
        {
            throwVelocityThreshold = threshold;
        }
    }
}
