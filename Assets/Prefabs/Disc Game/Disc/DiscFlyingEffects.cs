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
        
        private bool isPlayingFlyingSound = false;
        
        private void Awake()
        {
            // Initialize trail object - start disabled
            trailObject.SetActive(false);
        }
        
        public void StartFlyingFX()
        {
            // Enable trail object
            trailObject.SetActive(true);
            
            // Play flying sound
            if (!isPlayingFlyingSound)
            {
                flyingAudioSource.clip = flyingSound;
                flyingAudioSource.loop = true;
                flyingAudioSource.Play();
                isPlayingFlyingSound = true;
            }
            
            if (showDebugInfo)
            {
                Debug.Log("Started flying FX");
            }
        }
        
        public void StopFlyingFX()
        {
            // Disable trail object
            trailObject.SetActive(false);
            
            // Stop flying sound
            if (isPlayingFlyingSound)
            {
                flyingAudioSource.Stop();
                isPlayingFlyingSound = false;
            }
            
            if (showDebugInfo)
            {
                Debug.Log("Stopped flying FX");
            }
        }
    }
}
