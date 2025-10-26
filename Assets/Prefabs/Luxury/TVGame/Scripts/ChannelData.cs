using UnityEngine;

namespace VRArtMaking
{
    [CreateAssetMenu(fileName = "New Channel Data", menuName = "TV Game/Channel Data")]
    public class ChannelData : ScriptableObject
    {
        [Header("Channel Information")]
        public int channelNumber;
        public string channelName;
        public string description;
        
        [Header("Visual Content")]
        public UnityEngine.Video.VideoClip videoClip;
        
        [Header("Audio")]
        public bool useSeparateAudio = false;
        public AudioClip channelAudio;
        [Range(0f, 1f)]
        public float audioVolume = 1f;
        
        [Header("Game Settings")]
        public bool isCorrectChannel = false;
    }
}
