using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Events;

namespace VRArtMaking
{
    public class ShoppingManager : MonoBehaviour
    {
        [Header("Starting Stats")]
        [SerializeField] private float startingMoney = 100f;
        [SerializeField] private float startingHealth = 100f;
        [SerializeField] private float startingHunger = 0f;
        
        [Header("Hunger Limits")]
        [SerializeField] private float maxHunger = 100f;
        
        [Header("Current Stats")]
        [SerializeField] private float currentMoney;
        [SerializeField] private float currentHealth;
        [SerializeField] private float currentHunger;
        
        [Header("Game State")]
        [SerializeField] private bool isShopping = false;
        [SerializeField] private bool isFat = false;
        [SerializeField] private bool isBroke = false;
        
        [Header("UI Text Displays")]
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI hungerText;
        
        [Header("UI Sliders")]
        [SerializeField] private Slider moneySlider;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider hungerSlider;
        
        [Header("Unity Events")]
        [SerializeField] private UnityEvent onShoppingStarted;
        [SerializeField] private UnityEvent onShoppingEnded;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Events for when stats change
        public event Action<float> OnMoneyChanged;
        public event Action<float> OnHealthChanged;
        public event Action<float> OnHungerChanged;
        public event Action OnOutOfMoney;
        public event Action OnHealthDepleted;
        public event Action OnHungerFull;
        public event Action OnShoppingEnded;
        public event Action OnFat;
        public event Action OnBroke;
        
        public float Money => currentMoney;
        public float Health => currentHealth;
        public float Hunger => currentHunger;
        public float MaxHunger => maxHunger;
        public float StartingMoney => startingMoney;
        public float StartingHealth => startingHealth;
        public bool IsShopping => isShopping;
        
        private void Start()
        {
            // Initialize stats
            currentMoney = startingMoney;
            currentHealth = startingHealth;
            currentHunger = startingHunger;
            
            // Update UI displays
            UpdateMoneyDisplay();
            UpdateHealthDisplay();
            UpdateHungerDisplay();
            
            // Update slider values
            UpdateMoneySlider();
            UpdateHealthSlider();
            UpdateHungerSlider();
            
            if (showDebugInfo)
            {
                Debug.Log($"ShoppingManager initialized. Money: ${currentMoney}, Health: {currentHealth}, Hunger: {currentHunger}");
            }
        }
        
        public void SubtractMoney(float amount)
        {
            currentMoney -= amount;
            
            if (currentMoney < 0)
            {
                OnOutOfMoney?.Invoke();
                
                if (showDebugInfo)
                {
                    Debug.LogWarning("Player ran out of money!");
                }
            }
            
            OnMoneyChanged?.Invoke(currentMoney);
            UpdateMoneySlider();
            
            if (showDebugInfo)
            {
                Debug.Log($"Money subtracted: ${amount}. Current: ${currentMoney}");
            }
        }
        
        public void AddMoney(float amount)
        {
            currentMoney += amount;
            OnMoneyChanged?.Invoke(currentMoney);
            UpdateMoneySlider();
            
            if (showDebugInfo)
            {
                Debug.Log($"Money added: ${amount}. Current: ${currentMoney}");
            }
        }
        
        public void SubtractLifeExpectancy(float amount)
        {
            currentHealth -= amount;
            
            if (currentHealth < 0)
            {
                OnHealthDepleted?.Invoke();
                
                if (showDebugInfo)
                {
                    Debug.LogWarning("Player life expectancy depleted!");
                }
            }
            
            OnHealthChanged?.Invoke(currentHealth);
            UpdateHealthSlider();
            
            if (showDebugInfo)
            {
                Debug.Log($"Life expectancy subtracted: {amount}. Current: {currentHealth}");
            }
        }
        
        public void AddLifeExpectancy(float amount)
        {
            currentHealth += amount;
            
            // Optional: cap at max health
            if (currentHealth > startingHealth)
            {
                currentHealth = startingHealth;
            }
            
            OnHealthChanged?.Invoke(currentHealth);
            UpdateHealthSlider();
            
            if (showDebugInfo)
            {
                Debug.Log($"Life expectancy added: {amount}. Current: {currentHealth}");
            }
        }
        
        public void AddHunger(float amount)
        {
            currentHunger += amount;
            
            if (currentHunger > maxHunger)
            {
                currentHunger = maxHunger;
                OnHungerFull?.Invoke();
                
                if (showDebugInfo)
                {
                    Debug.LogWarning("Player hunger is full!");
                }
            }
            
            OnHungerChanged?.Invoke(currentHunger);
            UpdateHungerSlider();
            
            if (showDebugInfo)
            {
                Debug.Log($"Hunger added: {amount}. Current: {currentHunger}/{maxHunger}");
            }
        }
        
        public void SubtractHunger(float amount)
        {
            currentHunger -= amount;
            
            // Prevent hunger from going negative - clamp to 0
            if (currentHunger < 0)
            {
                currentHunger = 0;
            }
            
            OnHungerChanged?.Invoke(currentHunger);
            UpdateHungerSlider();
            
            if (showDebugInfo)
            {
                Debug.Log($"Hunger subtracted: {amount}. Current: {currentHunger}/{maxHunger}");
            }
        }
        
        public void StartShopping()
        {
            isShopping = true;
<<<<<<< HEAD
=======
            shoppingStartTime = Time.time;
>>>>>>> parent of 883311d (Grocery Timer added)
            
            // Invoke Unity Event
            onShoppingStarted?.Invoke();
            
            if (showDebugInfo)
            {
                Debug.Log("Shopping started!");
            }
        }
        
        public void EndShopping()
        {
            // Check if all conditions are met to end shopping
            bool hungerIsFull = currentHunger >= maxHunger;
            bool moneyIsNonNegative = currentMoney >= 0;
            bool healthIsNonNegative = currentHealth >= 0;
            
            if (hungerIsFull && moneyIsNonNegative && healthIsNonNegative)
            {
                isShopping = false;
                
                // Set end game state based on life expectancy
                if (currentHealth < 50)
                {
                    isFat = true;
                    isBroke = false;
                    OnFat?.Invoke();
                }
                else
                {
                    isFat = false;
                    isBroke = true;
                    OnBroke?.Invoke();
                }
                
                // Invoke both C# event and Unity Event
                OnShoppingEnded?.Invoke();
                onShoppingEnded?.Invoke();
                
                if (showDebugInfo)
                {
                    Debug.Log("Shopping ended! All conditions met: Hunger is full and all values are non-negative.");
                }
            }
            else
            {
                if (showDebugInfo)
                {
                    string issues = "";
                    if (!hungerIsFull) issues += $"Hunger not full ({currentHunger}/{maxHunger})";
                    if (!moneyIsNonNegative) issues += (issues.Length > 0 ? ", " : "") + $"Money is negative (${currentMoney:F2})";
                    if (!healthIsNonNegative) issues += (issues.Length > 0 ? ", " : "") + $"Health is negative ({currentHealth:F1})";
                    
                    Debug.LogWarning($"Cannot end shopping yet. Issues: {issues}");
                }
            }
        }
        
        public void ResetStats()
        {
            currentMoney = startingMoney;
            currentHealth = startingHealth;
            currentHunger = startingHunger;
            
            OnMoneyChanged?.Invoke(currentMoney);
            OnHealthChanged?.Invoke(currentHealth);
            OnHungerChanged?.Invoke(currentHunger);
            
            // Update UI displays
            UpdateMoneyDisplay();
            UpdateHealthDisplay();
            UpdateHungerDisplay();
            
            // Update slider values
            UpdateMoneySlider();
            UpdateHealthSlider();
            UpdateHungerSlider();
            
            if (showDebugInfo)
            {
                Debug.Log("ShoppingManager stats reset to starting values");
            }
        }
        
        private void UpdateMoneyDisplay()
        {
            if (moneyText != null)
            {
                moneyText.text = $"${currentMoney:F2}";
            }
        }
        
        private void UpdateHealthDisplay()
        {
            if (healthText != null)
            {
                healthText.text = $"{currentHealth:F1}";
            }
        }
        
        private void UpdateHungerDisplay()
        {
            if (hungerText != null)
            {
                hungerText.text = $"{currentHunger:F1}/{maxHunger:F1}";
            }
        }
        
        private void UpdateMoneySlider()
        {
            if (moneySlider != null)
            {
                // Normalize money: currentMoney / startingMoney, clamped to 0-1
                // If money goes negative, show 0. If it exceeds starting, show 1.
                float normalizedValue = startingMoney > 0 ? Mathf.Clamp01(currentMoney / startingMoney) : 0f;
                moneySlider.value = normalizedValue;
            }
        }
        
        private void UpdateHealthSlider()
        {
            if (healthSlider != null)
            {
                // Normalize health: currentHealth / startingHealth, clamped to 0-1
                float normalizedValue = startingHealth > 0 ? Mathf.Clamp01(currentHealth / startingHealth) : 0f;
                healthSlider.value = normalizedValue;
            }
        }
        
        private void UpdateHungerSlider()
        {
            if (hungerSlider != null)
            {
                // Normalize hunger: currentHunger / maxHunger, clamped to 0-1
                float normalizedValue = maxHunger > 0 ? Mathf.Clamp01(currentHunger / maxHunger) : 0f;
                hungerSlider.value = normalizedValue;
            }
        }
        
        private void OnEnable()
        {
            // Subscribe to events to update UI text displays
            // Note: Sliders are updated directly in the methods, not through events
            OnMoneyChanged += (value) => UpdateMoneyDisplay();
            OnHealthChanged += (value) => UpdateHealthDisplay();
            OnHungerChanged += (value) => UpdateHungerDisplay();
        }
        
<<<<<<< HEAD
=======
        private void Update()
        {
            // Check if shopping is active and time limit has been reached
            if (isShopping && shoppingTimeLimit > 0)
            {
                float elapsedTime = Time.time - shoppingStartTime;
                if (elapsedTime >= shoppingTimeLimit)
                {
                    ForceEndShopping($"Time limit reached ({shoppingTimeLimit} seconds)");
                }
            }
        }
        
>>>>>>> parent of 883311d (Grocery Timer added)
        private void OnDisable()
        {
            // Unsubscribe from events
            OnMoneyChanged -= (value) => UpdateMoneyDisplay();
            OnHealthChanged -= (value) => UpdateHealthDisplay();
            OnHungerChanged -= (value) => UpdateHungerDisplay();
        }
    }
}
