using UnityEngine;

namespace VRArtMaking
{
    public class DiscFlyingEffects : MonoBehaviour
    {
        [Header("Visual Effects")]
        [SerializeField] public TrailRenderer flyingTrail;
        
        [Header("Audio Effects")]
        [SerializeField] private AudioSource flyingAudioSource;
        [SerializeField] private AudioClip flyingSound;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private Rigidbody rb;
        private bool wasFlying = false;
        private bool isPlayingFlyingSound = false;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            
            // Don't modify the trail renderer - let it stay as configured in inspector
            // We'll use Clear() to start/stop the trail effect
        }
        
        private void Update()
        {
            // Use the state system instead of kinematic check
            DiscStateManager stateManager = GetComponent<DiscStateManager>();
            bool isMoving = stateManager != null && 
                           (stateManager.CurrentState == DiscStateManager.DiscState.Flying ||
                            stateManager.CurrentState == DiscStateManager.DiscState.Homing ||
                            stateManager.CurrentState == DiscStateManager.DiscState.Returning);
            
            // Check if movement state changed
            if (isMoving != wasFlying)
            {
                if (isMoving)
                {
                    StartFlyingEffects();
                }
                else
                {
                    StopFlyingEffects();
                }
                wasFlying = isMoving;
            }
        }
        
        private void StartFlyingEffects()
        {
            // Start trail effect - just clear it to start fresh
            if (flyingTrail != null)
            {
                flyingTrail.Clear(); // This starts a fresh trail without disabling the component
            }
            
            // Play flying sound
            if (flyingAudioSource != null && flyingSound != null && !isPlayingFlyingSound)
            {
                flyingAudioSource.clip = flyingSound;
                flyingAudioSource.loop = true;
                flyingAudioSource.Play();
                isPlayingFlyingSound = true;
            }
            
            if (showDebugInfo)
            {
                Debug.Log("Started flying effects - disc is moving");
            }
        }
        
        private void StopFlyingEffects()
        {
            // Stop trail effect - just clear it, don't disable the component
            if (flyingTrail != null)
            {
                flyingTrail.Clear(); // This stops the trail without disabling the component
            }
            
            // Stop flying sound
            if (flyingAudioSource != null && isPlayingFlyingSound)
            {
                flyingAudioSource.Stop();
                isPlayingFlyingSound = false;
            }
            
            if (showDebugInfo)
            {
                Debug.Log("Stopped flying effects - disc stopped moving");
            }
        }
    }
}
