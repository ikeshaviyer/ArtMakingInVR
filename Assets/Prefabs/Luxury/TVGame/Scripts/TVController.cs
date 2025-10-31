using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

namespace VRArtMaking
{
    public class TVController : MonoBehaviour
    {
        [Header("TV Components")]
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private AudioSource audioSource;
        
        [Header("Channel Settings")]
        [SerializeField] private List<ChannelData> channels = new List<ChannelData>();
        [SerializeField] private int currentChannelIndex = 0;
        [SerializeField] private bool isStaticTV = false;
        
        // Properties
        public bool IsStaticTV => isStaticTV;
        public bool IsOnCorrectChannel => channels.Count > 0 && channels[currentChannelIndex].isCorrectChannel;
        
        private void Start()
        {
            if (channels.Count > 0) SetChannel(0);
        }
        
        public void SetChannel(int channelIndex)
        {
            if (isStaticTV || channelIndex < 0 || channelIndex >= channels.Count) return;
            
            // Check if game is completed
            var gameManager = FindObjectOfType<TVGameManager>();
            if (gameManager != null && gameManager.IsGameCompleted) return;
            
            currentChannelIndex = channelIndex;
            UpdateDisplay();
        }
        
        public void NextChannel()
        {
            if (isStaticTV) return;
            
            // Check if game is completed
            var gameManager = FindObjectOfType<TVGameManager>();
            if (gameManager != null && gameManager.IsGameCompleted) return;
            
            SetChannel((currentChannelIndex + 1) % channels.Count);
        }
        
        public void PreviousChannel()
        {
            if (isStaticTV) return;
            
            // Check if game is completed
            var gameManager = FindObjectOfType<TVGameManager>();
            if (gameManager != null && gameManager.IsGameCompleted) return;
            
            SetChannel(currentChannelIndex == 0 ? channels.Count - 1 : currentChannelIndex - 1);
        }
        
        private void UpdateDisplay()
        {
            if (channels.Count == 0) return;
            
            var channel = channels[currentChannelIndex];
            var gameManager = FindObjectOfType<TVGameManager>();
            
            if (videoPlayer != null)
            {
                if (channel.videoClip != null)
                {
                    videoPlayer.clip = channel.videoClip;
                    videoPlayer.isLooping = true;
                    
                    videoPlayer.Play();
                    
                    // Get synchronized playback time from game manager
                    if (gameManager != null)
                    {
                        float syncTime = gameManager.GetChannelPlaybackTime(channel);
                        videoPlayer.time = syncTime;
                    }
                }
                else
                {
                    videoPlayer.Stop();
                }
            }
            
            if (audioSource != null)
            {
                if (channel.useSeparateAudio && channel.channelAudio != null)
                {
                    // Use separate audio clip
                    audioSource.clip = channel.channelAudio;
                    audioSource.volume = channel.audioVolume;
                    
                    // Sync audio time as well if game manager exists
                    if (gameManager != null)
                    {
                        float syncTime = gameManager.GetChannelPlaybackTime(channel);
                        audioSource.time = syncTime % audioSource.clip.length;
                    }
                    
                    audioSource.Play();
                }
                else if (videoPlayer != null && videoPlayer.clip != null)
                {
                    // Use video's audio
                    videoPlayer.SetDirectAudioVolume(0, channel.audioVolume);
                }
            }
            
            // Notify game manager of channel change
            gameManager?.OnTVChannelChanged();
        }
        
        public void SetStaticTV(bool isStatic)
        {
            isStaticTV = isStatic;
            if (isStatic)
            {
                for (int i = 0; i < channels.Count; i++)
                {
                    if (channels[i].isCorrectChannel)
                    {
                        currentChannelIndex = i;
                        UpdateDisplay();
                        break;
                    }
                }
            }
        }
        
        public void SetChannels(List<ChannelData> newChannels)
        {
            channels = new List<ChannelData>(newChannels);
            if (channels.Count > 0) SetChannel(0);
        }
        
        public bool CheckIfCorrectChannel() => IsOnCorrectChannel;
        
        public ChannelData GetCurrentChannel()
        {
            return channels.Count > 0 && currentChannelIndex >= 0 && currentChannelIndex < channels.Count 
                ? channels[currentChannelIndex] 
                : null;
        }
    }
}
