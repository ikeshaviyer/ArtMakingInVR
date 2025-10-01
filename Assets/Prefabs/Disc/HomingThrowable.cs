using UnityEngine;

namespace VRArtMaking
{
    /// <summary>
    /// A script that makes throwable objects home in on targets with a specific tag.
    /// Works with any grab system by detecting velocity changes to determine when objects are thrown.
    /// </summary>
    public class HomingThrowable : MonoBehaviour
    {
        [Header("Homing Settings")]
        [SerializeField] private string targetTag = "Target";
        [SerializeField] private float homingForce = 10f;
        [SerializeField] private float homingRange = 20f;
        [SerializeField] private float homingDelay = 0.5f; // Delay before homing starts
        [SerializeField] private float maxHomingTime = 5f; // Maximum time to home
        [SerializeField] private float throwVelocityThreshold = 2f; // Minimum velocity to consider as thrown
        
        [Header("Visual Effects")]
        [SerializeField] private TrailRenderer homingTrail;
        [SerializeField] private ParticleSystem homingParticles;
        [SerializeField] private AudioSource homingAudioSource;
        [SerializeField] private AudioClip homingSound;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private Rigidbody rb;
        private bool isHoming = false;
        private bool hasBeenThrown = false;
        private bool wasKinematic = false;
        private float throwTime;
        private Transform currentTarget;
        private Vector3 initialThrowVelocity;
        private Vector3 lastPosition;
        private float lastVelocityCheckTime;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            lastPosition = transform.position;
            lastVelocityCheckTime = Time.time;
        }
        
        private void Update()
        {
            // Check if object was just thrown by detecting velocity changes
            if (!hasBeenThrown && rb != null && !rb.isKinematic)
            {
                float currentVelocity = rb.velocity.magnitude;
                
                // If velocity is above threshold, consider it thrown
                if (currentVelocity > throwVelocityThreshold)
                {
                    OnObjectThrown();
                }
            }
            
            // Check if object was just released from kinematic state (grabbed)
            if (wasKinematic && rb != null && !rb.isKinematic)
            {
                // Small delay to let physics settle
                Invoke(nameof(CheckForThrow), 0.1f);
            }
            
            wasKinematic = rb != null && rb.isKinematic;
        }
        
        private void CheckForThrow()
        {
            if (!hasBeenThrown && rb != null && rb.velocity.magnitude > throwVelocityThreshold)
            {
                OnObjectThrown();
            }
        }
        
        private void OnObjectThrown()
        {
            if (!hasBeenThrown)
            {
                hasBeenThrown = true;
                throwTime = Time.time;
                initialThrowVelocity = rb.velocity;
                
                if (showDebugInfo)
                {
                    Debug.Log($"Object thrown with velocity: {initialThrowVelocity.magnitude}");
                }
                
                // Start homing after delay
                Invoke(nameof(StartHoming), homingDelay);
            }
        }
        
        private void StartHoming()
        {
            if (hasBeenThrown && !isHoming)
            {
                currentTarget = FindNearestTarget();
                
                if (currentTarget != null)
                {
                    isHoming = true;
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"Started homing to target: {currentTarget.name}");
                    }
                    
                    // Start visual effects
                    StartHomingEffects();
                    
                    // Play homing sound
                    if (homingAudioSource != null && homingSound != null)
                    {
                        homingAudioSource.PlayOneShot(homingSound);
                    }
                }
                else
                {
                    if (showDebugInfo)
                    {
                        Debug.Log("No target found for homing");
                    }
                }
            }
        }
        
        private Transform FindNearestTarget()
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
            Transform nearestTarget = null;
            float nearestDistance = float.MaxValue;
            
            foreach (GameObject target in targets)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance <= homingRange && distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTarget = target.transform;
                }
            }
            
            return nearestTarget;
        }
        
        private void StartHomingEffects()
        {
            // Enable trail effect
            if (homingTrail != null)
            {
                homingTrail.enabled = true;
                homingTrail.Clear(); // Clear any existing trail
            }
            
            // Start particle effect
            if (homingParticles != null)
            {
                homingParticles.Play();
            }
        }
        
        private void StopHomingEffects()
        {
            // Disable trail effect
            if (homingTrail != null)
            {
                homingTrail.enabled = false;
            }
            
            // Stop particle effect
            if (homingParticles != null)
            {
                homingParticles.Stop();
            }
        }
        
        private void FixedUpdate()
        {
            if (isHoming && currentTarget != null)
            {
                // Check if homing time has expired
                if (Time.time - throwTime > maxHomingTime)
                {
                    StopHoming();
                    return;
                }
                
                // Calculate homing force
                Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;
                Vector3 homingForceVector = directionToTarget * homingForce;
                
                // Apply homing force
                rb.AddForce(homingForceVector, ForceMode.Force);
                
                // Rotate object to face target
                if (rb.velocity.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
                }
                
                // Check if we're close enough to the target
                float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
                if (distanceToTarget < 1f)
                {
                    OnTargetReached();
                }
            }
        }
        
        private void OnTargetReached()
        {
            if (showDebugInfo)
            {
                Debug.Log($"Reached target: {currentTarget.name}");
            }
            
            // Trigger target hit event
            Target targetComponent = currentTarget.GetComponent<Target>();
            if (targetComponent != null)
            {
                targetComponent.OnHit(this);
            }
            
            StopHoming();
        }
        
        private void StopHoming()
        {
            isHoming = false;
            currentTarget = null;
            StopHomingEffects();
        }
        
        private void OnDrawGizmosSelected()
        {
            if (showDebugInfo)
            {
                // Draw homing range
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, homingRange);
                
                // Draw line to current target
                if (currentTarget != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, currentTarget.position);
                }
            }
        }
        
        // Public methods for external control
        public void SetTarget(Transform target)
        {
            currentTarget = target;
        }
        
        public void SetHomingForce(float force)
        {
            homingForce = force;
        }
        
        public void SetHomingRange(float range)
        {
            homingRange = range;
        }
        
        public bool IsHoming()
        {
            return isHoming;
        }
        
        public Transform GetCurrentTarget()
        {
            return currentTarget;
        }
    }
}
