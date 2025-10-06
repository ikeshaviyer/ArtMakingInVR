using UnityEngine;

namespace VRArtMaking
{
    public class DiscReturnHandler : MonoBehaviour
    {
        [Header("Return Settings")]
        [SerializeField] public float returnSpeed = 15f;
        [SerializeField] private float maxFlyingTime = 10f;
        [SerializeField] private float curveHeight = 10f;
        [SerializeField] private float curveForce = 20f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private Rigidbody rb;
        private bool isReturning = false;
        private Vector3 spawnPosition;
        private float flyingStartTime;
        private Vector3 returnStartPosition;
        private float returnStartTime;
        
        public bool IsReturning => isReturning;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        
        public void Initialize(Vector3 spawnPos)
        {
            spawnPosition = spawnPos;
        }
        
        public void StartReturn()
        {
            if (!isReturning)
            {
                isReturning = true;
                returnStartPosition = transform.position;
                returnStartTime = Time.time;
                
                // Make sure rigidbody is not kinematic for physics forces
                if (rb != null && rb.isKinematic)
                {
                    rb.isKinematic = false;
                }
                
                if (showDebugInfo)
                {
                    Debug.Log("Disc is returning to spawn location with Tron-style curve");
                }
            }
        }
        
        public void StopReturn()
        {
            isReturning = false;
        }
        
        public bool ShouldReturn(bool isFlying, bool isGrabbed)
        {
            if (isFlying && !isReturning && !isGrabbed)
            {
                // Start tracking flying time when disc starts flying
                if (flyingStartTime == 0f)
                {
                    flyingStartTime = Time.time;
                }
                
                // Check if disc has been flying too long - return no matter what
                float flyingTime = Time.time - flyingStartTime;
                return flyingTime > maxFlyingTime;
            }
            else
            {
                // Reset flying time when not flying
                flyingStartTime = 0f;
            }
            return false;
        }
        
        private void FixedUpdate()
        {
            if (isReturning)
            {
                // Calculate Tron-style curved return path
                float returnProgress = (Time.time - returnStartTime) * 0.5f; // Speed of return
                
                if (returnProgress < 1f)
                {
                    // Calculate curve waypoint (high point in the arc)
                    Vector3 midPoint = Vector3.Lerp(returnStartPosition, spawnPosition, 0.5f);
                    midPoint.y += curveHeight;
                    
                    // Calculate position on the curve using quadratic Bezier
                    Vector3 curvePosition = CalculateBezierPoint(returnProgress, returnStartPosition, midPoint, spawnPosition);
                    
                    // Apply force towards the curve position
                    Vector3 directionToCurve = (curvePosition - transform.position).normalized;
                    Vector3 returnForce = directionToCurve * curveForce;
                    
                    // Apply the force
                    if (rb != null)
                    {
                        rb.AddForce(returnForce, ForceMode.Force);
                    }
                    
                    // Rotate to face movement direction
                    if (rb != null && rb.velocity.magnitude > 0.1f)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
                    }
                }
                else
                {
                    // Curve is complete - now smoothly lerp to exact spawn position
                    HandleFinalApproach();
                }
            }
        }
        
        private void HandleFinalApproach()
        {
            float distanceToSpawn = Vector3.Distance(transform.position, spawnPosition);
            
            if (distanceToSpawn > 0.1f)
            {
                // Make rigidbody kinematic for smooth lerping
                if (rb != null && !rb.isKinematic)
                {
                    rb.isKinematic = true;
                }
                
                // Smoothly lerp to spawn position
                Vector3 directionToSpawn = (spawnPosition - transform.position).normalized;
                Vector3 newPosition = Vector3.MoveTowards(transform.position, spawnPosition, returnSpeed * Time.fixedDeltaTime);
                transform.position = newPosition;
                
                // Rotate to face spawn location
                Quaternion targetRotation = Quaternion.LookRotation(directionToSpawn);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
            }
            else
            {
                // Close enough - complete return
                OnReturnedToSpawn();
            }
        }
        
        private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            // Quadratic Bezier curve: B(t) = (1-t)²P₀ + 2(1-t)tP₁ + t²P₂
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;
            
            Vector3 point = uu * p0 + 2f * u * t * p1 + tt * p2;
            return point;
        }
        
        private void OnReturnedToSpawn()
        {
            if (showDebugInfo)
            {
                Debug.Log("Disc returned to spawn location");
            }
            
            // Stop returning
            isReturning = false;
            
            // Ensure rigidbody is kinematic
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
            
            // Reset position to spawn location
            transform.position = spawnPosition;
            
            // Notify that return is complete
            DiscStateManager discStateManager = GetComponent<DiscStateManager>();
            if (discStateManager != null)
            {
                discStateManager.OnReturnComplete();
            }
        }
        
        public void SetReturnSpeed(float speed)
        {
            returnSpeed = speed;
        }
        
        public void SetMaxFlyingTime(float time)
        {
            maxFlyingTime = time;
        }
        
        public void SetCurveHeight(float height)
        {
            curveHeight = height;
        }
        
        public void SetCurveForce(float force)
        {
            curveForce = force;
        }
    }
}
