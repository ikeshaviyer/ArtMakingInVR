using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace VRArtMaking
{
    public class TVGameManager : MonoBehaviour
    {
        [Header("TV Setup")]
        [SerializeField] private List<TVController> allTVs = new List<TVController>();
        [SerializeField] private List<ChannelData> allChannels = new List<ChannelData>();
        
        [Header("Game Settings")]
        [SerializeField] private bool startOnAwake = true;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Events
        public System.Action OnGameCompleted;
        
        // Private variables
        private TVController staticTV;
        private List<TVController> controllableTVs = new List<TVController>();
        private ChannelData correctChannel;
        private bool gameStarted = false;
        private bool gameCompleted = false;
        
        // Properties
        public bool IsGameStarted => gameStarted;
        public bool IsGameCompleted => gameCompleted;
        
        private void Start()
        {
            // Find all TVs if not assigned
            if (allTVs.Count == 0)
            {
                allTVs = FindObjectsOfType<TVController>().ToList();
            }
            
            // Find correct channel
            correctChannel = allChannels.FirstOrDefault(ch => ch.isCorrectChannel);
            if (correctChannel == null)
            {
                Debug.LogError("No correct channel found!");
                return;
            }
            
            // Separate TVs
            controllableTVs = allTVs.Where(tv => !tv.IsStaticTV).ToList();
            staticTV = allTVs.FirstOrDefault(tv => tv.IsStaticTV) ?? allTVs[0];
            
            if (staticTV == allTVs[0])
            {
                staticTV.SetStaticTV(true);
                controllableTVs.Remove(staticTV);
            }
            
            // Distribute channels
            DistributeChannels();
            
            // Start game if enabled
            if (startOnAwake)
            {
                StartGame();
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"TV Game initialized with {allTVs.Count} TVs ({controllableTVs.Count} controllable, 1 static)");
            }
        }
        
        private void DistributeChannels()
        {
            var availableChannels = allChannels.Where(ch => ch != correctChannel).ToList();
            int channelsPerTV = Mathf.Max(1, availableChannels.Count / controllableTVs.Count);
            
            // Create channel pool with repeats if needed
            var channelPool = new List<ChannelData>();
            for (int i = 0; i < channelsPerTV * controllableTVs.Count; i++)
            {
                channelPool.Add(availableChannels[i % availableChannels.Count]);
            }
            channelPool = channelPool.OrderBy(x => Random.Range(0f, 1f)).ToList();
            
            // Assign to controllable TVs
            int index = 0;
            foreach (var tv in controllableTVs)
            {
                var tvChannels = channelPool.Skip(index).Take(channelsPerTV).ToList();
                tvChannels.Add(correctChannel);
                tvChannels = tvChannels.OrderBy(x => Random.Range(0f, 1f)).ToList();
                tv.SetChannels(tvChannels);
                index += channelsPerTV;
            }
            
            // Assign to static TV
            var staticChannels = availableChannels.Take(channelsPerTV).ToList();
            staticChannels.Add(correctChannel);
            staticChannels = staticChannels.OrderBy(x => Random.Range(0f, 1f)).ToList();
            staticTV.SetChannels(staticChannels);
            staticTV.SetStaticTV(true);
        }
        
        public void StartGame()
        {
            if (gameStarted) return;
            gameStarted = true;
            gameCompleted = false;
            if (showDebugInfo) Debug.Log("TV Game started!");
        }
        
        public void CheckGameCompletion()
        {
            if (gameCompleted || !gameStarted) return;
            
            if (controllableTVs.All(tv => tv.CheckIfCorrectChannel()))
            {
                gameCompleted = true;
                OnGameCompleted?.Invoke();
                if (showDebugInfo) Debug.Log("All TVs matched! Game completed!");
            }
        }
        
        [ContextMenu("Start Game")]
        public void StartGameFromContextMenu() => StartGame();
    }
}
