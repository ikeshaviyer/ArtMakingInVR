using UnityEngine;
using System.Collections;

namespace VRArtMaking
{
    public class Target : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private bool isActive = true;
        
        [Header("Visual Effects")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private ParticleSystem hitParticles;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private AudioClip destroySound;
        
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Reference to spawner (set by TargetSpawner when spawning)
        private TargetSpawner spawner;
        
        private bool isDestroyed = false;
        private bool wasHitByPlayer = false;
        
        private void Awake()
        {
        }
        
        private void Start()
        {
            // Ensure this object has the Target tag
            if (!gameObject.CompareTag("Target"))
            {
                Debug.LogWarning($"Target script on {gameObject.name} but object doesn't have 'Target' tag!");
            }
        }
        
        public void OnHit(MonoBehaviour throwable)
        {
            if (!isActive || isDestroyed)
                return;
            
            if (showDebugInfo)
            {
                Debug.Log($"Target {gameObject.name} hit by {throwable.name}");
            }
            
            // Mark as hit by player
            wasHitByPlayer = true;
            
            // Play hit effects and destroy immediately
            PlayHitEffects();
            Destroy();
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
        
        public void Destroy()
        {
            if (isDestroyed)
                return;
                
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
            
            // Add score to game manager only if hit by player
            if (DiscGameManager.Instance != null && wasHitByPlayer)
            {
                DiscGameManager.Instance.AddScore(1);
            }
            
            // Play hit effects
            PlayHitEffects();
            
            // Notify spawner if this target was spawned
            if (spawner != null)
            {
                spawner.OnTargetDestroyed();
            }
            
            // Destroy the GameObject
            Destroy(gameObject);
        }
        
        
        // Public methods for external control
        public bool IsDestroyed()
        {
            return isDestroyed;
        }
        
        public bool IsActive()
        {
            return isActive;
        }
        
        public void SetSpawner(TargetSpawner targetSpawner)
        {
            spawner = targetSpawner;
        }
    }
}
