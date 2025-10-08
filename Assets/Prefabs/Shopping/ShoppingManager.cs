using UnityEngine;
using System;

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
                currentMoney = 0;
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
        
        public void SubtractHealth(float amount)
        {
            currentHealth -= amount;
            
            if (currentHealth < 0)
            {
                currentHealth = 0;
                OnHealthDepleted?.Invoke();
                
                if (showDebugInfo)
                {
                    Debug.LogWarning("Player health depleted!");
                }
            }
            
            OnHealthChanged?.Invoke(currentHealth);
            
            if (showDebugInfo)
            {
                Debug.Log($"Health subtracted: {amount}. Current: {currentHealth}");
            }
        }
        
        public void AddHealth(float amount)
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
                Debug.Log($"Health added: {amount}. Current: {currentHealth}");
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
            
            if (showDebugInfo)
            {
                Debug.Log("Shopping started!");
            }
        }
        
        public void EndShopping()
        {
            if (currentHunger >= maxHunger)
            {
                isShopping = false;
                OnShoppingEnded?.Invoke();
                
                if (showDebugInfo)
                {
                    Debug.Log("Shopping ended! Hunger is full.");
                }
            }
            else
            {
                if (showDebugInfo)
                {
                    Debug.LogWarning($"Cannot end shopping yet. Hunger: {currentHunger}/{maxHunger}");
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
            
            if (showDebugInfo)
            {
                Debug.Log("ShoppingManager stats reset to starting values");
            }
        }
    }
}
