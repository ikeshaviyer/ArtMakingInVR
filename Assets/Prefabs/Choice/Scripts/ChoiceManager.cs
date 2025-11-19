using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

namespace VRArtMaking
{
    public class ChoiceManager : MonoBehaviour
    {
        [Header("Video Player")]
        [SerializeField] private VideoPlayer videoPlayer;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onVideoOver;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // C# Event
        public System.Action OnVideoOver;
        
        private bool isVideoPlaying = false;
        private bool hasVideoEnded = false;
        
        private void Start()
        {
            // Try to find VideoPlayer if not assigned
            if (videoPlayer == null)
            {
                videoPlayer = GetComponent<VideoPlayer>();
                
                if (videoPlayer == null)
                {
                    videoPlayer = GetComponentInChildren<VideoPlayer>();
                }
                
                if (videoPlayer == null && showDebugInfo)
                {
                    Debug.LogWarning("ChoiceManager: No VideoPlayer found! Please assign one.");
                }
            }
            
            // Subscribe to video player events
            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached += OnVideoEnd;
            }
        }
        
        private void Update()
        {
            if (videoPlayer == null) return;
            
            // Track if video is playing
            bool wasPlaying = isVideoPlaying;
            isVideoPlaying = videoPlayer.isPlaying;
            
            // Detect when video starts playing
            if (!wasPlaying && isVideoPlaying)
            {
                hasVideoEnded = false;
                if (showDebugInfo)
                {
                    Debug.Log("Video started playing");
                }
            }
            
            // Fallback detection for video end (in case loopPointReached doesn't fire)
            if (wasPlaying && !isVideoPlaying && !hasVideoEnded && videoPlayer.clip != null)
            {
                // Check if video reached the end
                if (videoPlayer.time >= videoPlayer.clip.length - 0.1f)
                {
                    OnVideoEnd(videoPlayer);
                }
            }
        }
        
        private void OnVideoEnd(VideoPlayer source)
        {
            if (hasVideoEnded) return;
            
            hasVideoEnded = true;
            
            // Invoke both C# event and Unity Event
            OnVideoOver?.Invoke();
            onVideoOver?.Invoke();
            
            if (showDebugInfo)
            {
                Debug.Log("Video over!");
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached -= OnVideoEnd;
            }
        }
        
        /// <summary>
        /// Manually trigger video end event (for testing)
        /// </summary>
        [ContextMenu("Trigger Video Over")]
        public void TriggerVideoOver()
        {
            OnVideoEnd(videoPlayer);
        }
    }
}

