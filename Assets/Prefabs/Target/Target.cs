using UnityEngine;
using System.Collections;

namespace VRArtMaking
{
    /// <summary>
    /// A script for objects that can be targeted by homing throwables.
    /// Handles hit detection and provides visual/audio feedback.
    /// </summary>
    public class Target : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private bool isActive = true;
        [SerializeField] private int hitPoints = 1;
        [SerializeField] private float respawnTime = 3f;
        
        [Header("Visual Effects")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private ParticleSystem hitParticles;
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float hitFlashDuration = 0.2f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private AudioClip destroySound;
        
        [Header("Animation")]
        [SerializeField] private bool useScaleAnimation = true;
        [SerializeField] private float scaleAnimationDuration = 0.3f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 1.2f);
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private int currentHitPoints;
        private bool isDestroyed = false;
        private Vector3 originalScale;
        private Color originalColor;
        private Collider targetCollider;
        
        private void Awake()
        {
            currentHitPoints = hitPoints;
            originalScale = transform.localScale;
            targetCollider = GetComponent<Collider>();
            
            if (targetRenderer != null)
            {
                originalColor = targetRenderer.material.color;
            }
            
            // Set initial state
            SetActiveState(isActive);
        }
        
        private void Start()
        {
            // Ensure this object has the Target tag
            if (!gameObject.CompareTag("Target"))
            {
                Debug.LogWarning($"Target script on {gameObject.name} but object doesn't have 'Target' tag!");
            }
        }
        
        /// <summary>
        /// Called when a homing throwable hits this target
        /// </summary>
        /// <param name="throwable">The homing throwable that hit this target</param>
        public void OnHit(HomingThrowable throwable)
        {
            if (!isActive || isDestroyed)
                return;
            
            if (showDebugInfo)
            {
                Debug.Log($"Target {gameObject.name} hit by {throwable.name}");
            }
            
            // Reduce hit points
            currentHitPoints--;
            
            // Play hit effects
            PlayHitEffects();
            
            // Check if target should be destroyed
            if (currentHitPoints <= 0)
            {
                StartCoroutine(DestroyTarget());
            }
            else
            {
                StartCoroutine(FlashHit());
            }
        }
        
        private void PlayHitEffects()
        {
            // Play hit sound
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
            
            // Create hit effect
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, transform.rotation);
            }
            
            // Play particle effect
            if (hitParticles != null)
            {
                hitParticles.Play();
            }
        }
        
        private IEnumerator FlashHit()
        {
            if (targetRenderer != null)
            {
                // Flash to hit color
                targetRenderer.material.color = hitColor;
                yield return new WaitForSeconds(hitFlashDuration);
                
                // Return to normal color
                targetRenderer.material.color = originalColor;
            }
        }
        
        private IEnumerator DestroyTarget()
        {
            isDestroyed = true;
            
            if (showDebugInfo)
            {
                Debug.Log($"Target {gameObject.name} destroyed!");
            }
            
            // Play destroy sound
            if (audioSource != null && destroySound != null)
            {
                audioSource.PlayOneShot(destroySound);
            }
            
            // Disable collider
            if (targetCollider != null)
            {
                targetCollider.enabled = false;
            }
            
            // Play destroy animation
            if (useScaleAnimation)
            {
                yield return StartCoroutine(ScaleAnimation(originalScale, Vector3.zero, scaleAnimationDuration));
            }
            
            // Hide the target
            SetActiveState(false);
            
            // Respawn after delay if respawn time > 0
            if (respawnTime > 0)
            {
                yield return new WaitForSeconds(respawnTime);
                Respawn();
            }
        }
        
        private IEnumerator ScaleAnimation(Vector3 fromScale, Vector3 toScale, float duration)
        {
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                float curveValue = scaleCurve.Evaluate(t);
                transform.localScale = Vector3.Lerp(fromScale, toScale, curveValue);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            transform.localScale = toScale;
        }
        
        public void Respawn()
        {
            if (showDebugInfo)
            {
                Debug.Log($"Target {gameObject.name} respawned!");
            }
            
            // Reset state
            currentHitPoints = hitPoints;
            isDestroyed = false;
            transform.localScale = originalScale;
            
            // Re-enable collider
            if (targetCollider != null)
            {
                targetCollider.enabled = true;
            }
            
            // Reset color
            if (targetRenderer != null)
            {
                targetRenderer.material.color = originalColor;
            }
            
            // Show the target
            SetActiveState(true);
        }
        
        private void SetActiveState(bool active)
        {
            isActive = active;
            
            // Enable/disable renderer
            if (targetRenderer != null)
            {
                targetRenderer.enabled = active;
            }
            
            // Enable/disable collider
            if (targetCollider != null)
            {
                targetCollider.enabled = active;
            }
        }
        
        // Public methods for external control
        public void SetActive(bool active)
        {
            SetActiveState(active);
        }
        
        public void SetHitPoints(int points)
        {
            hitPoints = points;
            currentHitPoints = points;
        }
        
        public int GetCurrentHitPoints()
        {
            return currentHitPoints;
        }
        
        public bool IsDestroyed()
        {
            return isDestroyed;
        }
        
        public bool IsActive()
        {
            return isActive;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (showDebugInfo)
            {
                // Draw target indicator
                Gizmos.color = isActive ? Color.green : Color.gray;
                Gizmos.DrawWireSphere(transform.position, 0.5f);
                
                // Draw hit points indicator
                Gizmos.color = Color.red;
                for (int i = 0; i < currentHitPoints; i++)
                {
                    Vector3 offset = Vector3.up * (i * 0.2f + 0.5f);
                    Gizmos.DrawWireCube(transform.position + offset, Vector3.one * 0.1f);
                }
            }
        }
    }
}
