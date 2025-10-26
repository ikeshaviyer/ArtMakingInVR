using UnityEngine;

namespace VRArtMaking
{
    public class TVInteraction : MonoBehaviour
    {
        [SerializeField] private TVController tvController;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip channelChangeSound;
        
        private void Start()
        {
            tvController = GetComponent<TVController>();
            audioSource = GetComponent<AudioSource>();
        }
        
        public void NextChannel()
        {
            if (!tvController.IsStaticTV)
            {
                tvController.NextChannel();
                PlayChannelChangeSound();
            }
        }
        
        public void PreviousChannel()
        {
            if (!tvController.IsStaticTV)
            {
                tvController.PreviousChannel();
                PlayChannelChangeSound();
            }
        }
        
        private void PlayChannelChangeSound()
        {
            audioSource.PlayOneShot(channelChangeSound);
        }
    }
}
