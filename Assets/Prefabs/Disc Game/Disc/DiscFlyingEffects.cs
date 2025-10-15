using UnityEngine;

namespace VRArtMaking
{
    public class DiscFlyingEffects : MonoBehaviour
    {
        [Header("Visual Effects")]
        [SerializeField] private GameObject trailObject;
        
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
            
            // Initialize trail object - start disabled
            if (trailObject != null)
            {
                trailObject.SetActive(false);
            }
        }
        
        private void Update()
        {
            // Use the state system instead of kinematic check
            DiscStateManager stateManager = GetComponent<DiscStateManager>();
            bool isMoving = stateManager != null && 
                           (stateManager.CurrentState == DiscStateManager.DiscState.Homing ||
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
        
        public void StartFlyingEffects()
        {
            // Enable trail object
            if (trailObject != null)
            {
                trailObject.SetActive(true);
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
        
        public void StopFlyingEffects()
        {
            // Disable trail object
            if (trailObject != null)
            {
                trailObject.SetActive(false);
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
