using UnityEngine;

namespace VRArtMaking
{
    public class DiscHomingHandler : MonoBehaviour
    {
        [Header("Homing Settings")]
        [SerializeField] private string targetTag = "Target";
        [SerializeField] private float homingForce = 10f;
        [SerializeField] private float homingRange = 20f;
        [SerializeField] private float homingDelay = 0.5f;
        [SerializeField] private float maxHomingTime = 5f;
        
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private Rigidbody rb;
        private bool isHoming = false;
        private float throwTime;
        private Transform currentTarget;
        
        public bool IsHoming => isHoming;
        public Transform CurrentTarget => currentTarget;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
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
        
        public void StopHoming()
        {
            isHoming = false;
            currentTarget = null;
        }
        
        
        private void FixedUpdate()
        {
            // Check if we should be homing (not kinematic and not returning)
            bool shouldHoming = rb != null && !rb.isKinematic;
            DiscReturnHandler returnHandler = GetComponent<DiscReturnHandler>();
            if (returnHandler != null && returnHandler.IsReturning)
            {
                shouldHoming = false;
            }
            
            if (shouldHoming)
            {
                // Find a target if we don't have one
                if (currentTarget == null)
                {
                    currentTarget = FindNearestTarget();
                    if (currentTarget != null)
                    {
                        isHoming = true;
                        throwTime = Time.time;
                        
                        if (showDebugInfo)
                        {
                            Debug.Log($"Started homing to target: {currentTarget.name}");
                        }
                    }
                }
                
                // If we have a target, home to it
                if (isHoming && currentTarget != null)
                {
                    // Check if homing time has expired
                    if (Time.time - throwTime > maxHomingTime)
                    {
                        StopHoming();
                        
                        // Notify that homing failed
                        DiscStateManager discStateManager = GetComponent<DiscStateManager>();
                        if (discStateManager != null)
                        {
                            discStateManager.OnHomingFailed();
                        }
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
            else
            {
                // Not flying or returning - stop homing
                if (isHoming)
                {
                    StopHoming();
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
            
            // Notify that target was hit
            DiscStateManager discStateManager = GetComponent<DiscStateManager>();
            if (discStateManager != null)
            {
                discStateManager.OnTargetHit();
            }
        }
        
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
    }
}
