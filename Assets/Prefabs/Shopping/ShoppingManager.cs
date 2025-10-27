using UnityEngine;
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
        
        [Header("UI Text Displays")]
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI hungerText;
        
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
        
        public float Money => currentMoney;
        public float Health => currentHealth;
        public float Hunger => currentHunger;
        public float MaxHunger => maxHunger;
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
            
            if (showDebugInfo)
            {
                Debug.Log($"Money subtracted: ${amount}. Current: ${currentMoney}");
            }
        }
        
        public void AddMoney(float amount)
        {
            currentMoney += amount;
            OnMoneyChanged?.Invoke(currentMoney);
            
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
            
            if (showDebugInfo)
            {
                Debug.Log($"Hunger subtracted: {amount}. Current: {currentHunger}/{maxHunger}");
            }
        }
        
        public void StartShopping()
        {
            isShopping = true;
            
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
            
            if (showDebugInfo)
            {
                Debug.Log("ShoppingManager stats reset to starting values");
            }
        }
        
        private void UpdateMoneyDisplay()
        {
            if (moneyText != null)
            {
                moneyText.text = $"Money: ${currentMoney:F2}";
            }
        }
        
        private void UpdateHealthDisplay()
        {
            if (healthText != null)
            {
                healthText.text = $"Health: {currentHealth:F1}";
            }
        }
        
        private void UpdateHungerDisplay()
        {
            if (hungerText != null)
            {
                hungerText.text = $"Hunger: {currentHunger:F1}/{maxHunger:F1}";
            }
        }
        
        private void OnEnable()
        {
            // Subscribe to events to update UI
            OnMoneyChanged += (value) => UpdateMoneyDisplay();
            OnHealthChanged += (value) => UpdateHealthDisplay();
            OnHungerChanged += (value) => UpdateHungerDisplay();
        }
        
        private void OnDisable()
        {
            // Unsubscribe from events
            OnMoneyChanged -= (value) => UpdateMoneyDisplay();
            OnHealthChanged -= (value) => UpdateHealthDisplay();
            OnHungerChanged -= (value) => UpdateHungerDisplay();
        }
    }
}
