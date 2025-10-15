using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace VRArtMaking
{
    public class DiscGameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private float gameTime = 60f;
        [SerializeField] private int startingScore = 0;
        
        [Header("References")]
        [SerializeField] private TargetSpawner targetSpawner;
        [SerializeField] private Text timerText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text gameStatusText;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool startOnPlay = false;
        
        private float currentTime;
        private int currentScore;
        private bool gameActive = false;
        private bool gameEnded = false;
        
        public static DiscGameManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        
        private void Start()
        {
            InitializeGame();
            
            // Subscribe to disc grab event
            DiscStateManager.OnDiscFirstGrabbed += StartGame;
        }
        
        private void Update()
        {
            if (gameActive && !gameEnded)
            {
                UpdateTimer();
            }
        }
        
        public void InitializeGame()
        {
            currentTime = gameTime;
            currentScore = startingScore;
            gameActive = false;
            gameEnded = false;
            
            UpdateUI();
            
            
            
            if (showDebugInfo)
            {
                Debug.Log("Game initialized. Ready to start!");
            }
            
            // Auto-start for debugging
            if (startOnPlay)
            {
                StartGame();
            }
        }
        
        public void StartGame()
        {
            if (gameEnded) return;
            
            gameActive = true;
            gameEnded = false;
            
            if (targetSpawner != null)
            {
                targetSpawner.StartSpawning();
            }
            
            if (gameStatusText != null)
            {
                gameStatusText.text = "Game Active!";
            }
            
            if (showDebugInfo)
            {
                Debug.Log("Game started!");
            }
        }
        
        public void EndGame()
        {
            if (gameEnded) return;
            
            gameActive = false;
            gameEnded = true;
            
            if (targetSpawner != null)
            {
                targetSpawner.StopSpawning();
                targetSpawner.ClearAllTargets();
            }
            
            if (gameStatusText != null)
            {
                gameStatusText.text = "Game Over!";
            }
            
            
            if (showDebugInfo)
            {
                Debug.Log($"Game ended! Final Score: {currentScore}");
            }
        }
        
        public void RestartGame()
        {
            InitializeGame();
            StartGame();
        }
        
        private void UpdateTimer()
        {
            currentTime -= Time.deltaTime;
            
            if (currentTime <= 0)
            {
                currentTime = 0;
                EndGame();
            }
            
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(currentTime / 60f);
                int seconds = Mathf.FloorToInt(currentTime % 60f);
                timerText.text = $"Time: {minutes:00}:{seconds:00}";
            }
            
            if (scoreText != null)
            {
                scoreText.text = $"Score: {currentScore}";
            }
        }
        
        public void AddScore(int points)
        {
            currentScore += points;
            UpdateUI();
            
            if (showDebugInfo)
            {
                Debug.Log($"Score added: +{points}. Total: {currentScore}");
            }
        }
        
        public void SetScore(int score)
        {
            currentScore = score;
            UpdateUI();
        }
        
        public int GetScore()
        {
            return currentScore;
        }
        
        public float GetTimeRemaining()
        {
            return currentTime;
        }
        
        public bool IsGameActive()
        {
            return gameActive && !gameEnded;
        }
        
        public bool IsGameEnded()
        {
            return gameEnded;
        }
        
        public void SetGameTime(float time)
        {
            gameTime = time;
            if (!gameActive)
            {
                currentTime = gameTime;
                UpdateUI();
            }
        }
        
        public void PauseGame()
        {
            if (gameActive && !gameEnded)
            {
                gameActive = false;
                if (targetSpawner != null)
                {
                    targetSpawner.StopSpawning();
                }
            }
        }
        
        public void ResumeGame()
        {
            if (!gameActive && !gameEnded)
            {
                gameActive = true;
                if (targetSpawner != null)
                {
                    targetSpawner.StartSpawning();
                }
            }
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            
            // Unsubscribe from disc grab event
            DiscStateManager.OnDiscFirstGrabbed -= StartGame;
        }
    }
}
