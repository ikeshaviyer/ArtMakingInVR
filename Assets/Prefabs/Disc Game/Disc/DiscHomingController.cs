using UnityEngine;

namespace VRArtMaking
{
    public class DiscHomingController : MonoBehaviour
    {
        [Header("Homing Settings")]
        [SerializeField] private string targetTag = "Target";
        [SerializeField] private float homingRange = 20f;
        [SerializeField] private float homingSpeed = 10f;
        [SerializeField] private float homingDelay = 0.5f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool showGizmos = true;
        
        private Rigidbody rb;
        private Transform currentTarget;
        private bool isHoming = false;
        private float homingStartTime;
        
        public bool IsHoming => isHoming;
        public Transform CurrentTarget => currentTarget;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        
        private void Update()
        {
            // Only check for homing if we're not already homing
            if (!isHoming)
            {
                CheckForTargetsInRange();
            }
            else
            {
                // Continue homing to current target
                HomeToTarget();
            }
        }
        
        private void CheckForTargetsInRange()
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
            
            // If we found a target in range, start homing
            if (nearestTarget != null)
            {
                StartHoming(nearestTarget);
            }
        }
        
        private void StartHoming(Transform target)
        {
            currentTarget = target;
            isHoming = true;
            homingStartTime = Time.time;
            
            // Stop all forces and velocity before homing
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"Started homing to target: {target.name}");
            }
        }
        
        private void HomeToTarget()
        {
            if (currentTarget == null)
            {
                StopHoming();
                return;
            }
            
            // Wait for homing delay before starting movement
            if (Time.time - homingStartTime < homingDelay)
            {
                return;
            }
            
            // Calculate direction to target
            Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            
            // Move towards target using lerp
            Vector3 newPosition = Vector3.MoveTowards(transform.position, currentTarget.position, homingSpeed * Time.deltaTime);
            transform.position = newPosition;
            
            // Rotate to face target
            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
            
            // Check if we've reached the target
            if (distanceToTarget < 0.5f)
            {
                OnTargetReached();
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
            
            // Call disc return after hitting target
            DiscController discController = GetComponent<DiscController>();
            if (discController != null)
            {
                discController.Return();
            }
        }
        
        public void StopHoming()
        {
            isHoming = false;
            currentTarget = null;
            
            if (showDebugInfo)
            {
                Debug.Log("Stopped homing");
            }
        }
        
        private void OnDrawGizmos()
        {
            if (showGizmos)
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
        
        public void SetHomingRange(float range)
        {
            homingRange = range;
        }
        
        public void SetHomingSpeed(float speed)
        {
            homingSpeed = speed;
        }
        
        public void SetHomingDelay(float delay)
        {
            homingDelay = delay;
        }
    }
}
