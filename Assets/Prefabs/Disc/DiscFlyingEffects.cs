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
            
            // Start with effects disabled
            if (flyingTrail != null)
            {
                flyingTrail.enabled = false;
            }
        }
        
        private void Update()
        {
            bool isMoving = rb != null && !rb.isKinematic;
            
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
            // Start trail effect
            if (flyingTrail != null)
            {
                flyingTrail.enabled = true;
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
                Debug.Log("Started flying effects - disc is moving (not kinematic)");
            }
        }
        
        private void StopFlyingEffects()
        {
            // Stop trail effect
            if (flyingTrail != null)
            {
                flyingTrail.enabled = false;
            }
            
            // Stop flying sound
            if (flyingAudioSource != null && isPlayingFlyingSound)
            {
                flyingAudioSource.Stop();
                isPlayingFlyingSound = false;
            }
            
            if (showDebugInfo)
            {
                Debug.Log("Stopped flying effects - disc is now kinematic");
            }
        }
        
        public void SetTrailRenderer(TrailRenderer trail)
        {
            flyingTrail = trail;
            if (trail != null && !wasFlying)
            {
                trail.enabled = false;
            }
        }
        
        public void SetFlyingSound(AudioClip sound)
        {
            flyingSound = sound;
        }
        
        public void SetAudioSource(AudioSource source)
        {
            flyingAudioSource = source;
        }
    }
}
