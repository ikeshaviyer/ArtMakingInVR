using UnityEngine;
using System;
using Oculus.Interaction;

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
        private Grabbable grabbable;
        private bool hasBeenThrown = false;
        private bool isGrabbed = true; // Start in grabbed state
        private bool hasBeenGrabbedBefore = false; // Track if disc has been grabbed at least once
        private Vector3 spawnPosition;
        
        // Component references
        private DiscHomingHandler homingHandler;
        private DiscReturnHandler returnHandler;
        
        // State tracking
        public enum DiscState { Grabbed, Flying, Homing, Returning }
        private DiscState currentState = DiscState.Grabbed;
        
        public bool IsGrabbed => isGrabbed;
        public bool HasBeenThrown => hasBeenThrown;
        public DiscState CurrentState => currentState;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            grabbable = GetComponent<Grabbable>();
            homingHandler = GetComponent<DiscHomingHandler>();
            returnHandler = GetComponent<DiscReturnHandler>();
            
            // Set spawn position
            spawnPosition = transform.position;
            
            // Start at spawn position
            transform.position = spawnPosition;
            
            // Let Meta XR Grabbable handle kinematic state
            // It will automatically set rb.isKinematic = true when grabbed
        }
        
        private void Start()
        {
            // Initialize return handler with spawn position
            if (returnHandler != null)
            {
                returnHandler.Initialize(spawnPosition);
            }
            
            // Subscribe to Meta XR Grabbable events
            if (grabbable != null)
            {
                grabbable.WhenPointerEventRaised += OnGrabbableEvent;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (grabbable != null)
            {
                grabbable.WhenPointerEventRaised -= OnGrabbableEvent;
            }
        }
        
        private void OnGrabbableEvent(PointerEvent evt)
        {
            switch (evt.Type)
            {
                case PointerEventType.Select:
                    // Disc is being grabbed
                    isGrabbed = true;
                    currentState = DiscState.Grabbed;
                    
                    if (showDebugInfo)
                    {
                        Debug.Log("Disc state: Grabbed");
                    }
                    break;
                case PointerEventType.Unselect:
                    // Disc is being released - let Meta XR handle the throw
                    isGrabbed = false;
                    currentState = DiscState.Flying;
                    if (showDebugInfo)
                    {
                        Debug.Log("Disc state: Flying (thrown)");
                    }
                    break;
                case PointerEventType.Cancel:
                    // Grab was cancelled
                    isGrabbed = false;
                    currentState = DiscState.Flying;
                    if (showDebugInfo)
                    {
                        Debug.Log("Disc state: Flying (cancelled)");
                    }
                    break;
            }
        }
        
        private void Update()
        {
            // Check if disc was just grabbed for the first time
            if (!hasBeenGrabbedBefore && isGrabbed)
            {
                hasBeenGrabbedBefore = true;
                OnDiscFirstGrabbed?.Invoke();
                
                if (showDebugInfo)
                {
                    Debug.Log("Disc grabbed for the first time - starting game!");
                }
            }
            
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
            currentState = DiscState.Returning;
            if (returnHandler != null)
            {
                returnHandler.StartReturn();
            }
            if (showDebugInfo)
            {
                Debug.Log("Disc state: Returning (target hit)");
            }
        }
        
        public void OnHomingFailed()
        {
            // Homing failed, start returning
            currentState = DiscState.Returning;
            if (returnHandler != null)
            {
                returnHandler.StartReturn();
            }
            if (showDebugInfo)
            {
                Debug.Log("Disc state: Returning (homing failed)");
            }
        }
        
        public void OnReturnComplete()
        {
            // Reset state for next throw
            hasBeenThrown = false;
            currentState = DiscState.Grabbed;
            
            // Ensure the disc is properly positioned and stopped
            if (rb != null)
            {
                // The return handler already set it to kinematic and reset position/rotation
                // We just need to make sure it's ready for grabbing
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            
            if (showDebugInfo)
            {
                Debug.Log("Disc state: Grabbed (ready for next throw)");
            }
        }
        
        public void SetHomingState()
        {
            if (currentState == DiscState.Flying)
            {
                currentState = DiscState.Homing;
                if (showDebugInfo)
                {
                    Debug.Log("Disc state: Homing");
                }
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
