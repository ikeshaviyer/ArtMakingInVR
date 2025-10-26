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
            
            if (videoPlayer != null)
            {
                if (channel.videoClip != null)
                {
                    videoPlayer.clip = channel.videoClip;
                    videoPlayer.isLooping = true;
                    videoPlayer.Play();
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
                    audioSource.Play();
                }
                else if (videoPlayer != null && videoPlayer.clip != null)
                {
                    // Use video's audio
                    videoPlayer.SetDirectAudioVolume(0, channel.audioVolume);
                }
            }
            
            // Notify game manager of channel change
            var gameManager = FindObjectOfType<TVGameManager>();
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
    }
}
