using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VRArtMaking
{
    public class Grocery : MonoBehaviour
    {
        [Header("Item Information")]
        [SerializeField] private string itemName;
        
        [Header("Item Stats")]
        [SerializeField] private float price;
        [SerializeField, Tooltip("Positive = add life, Negative = reduce life")]
        private float lifeExpectancy;
        [SerializeField, Tooltip("Positive = add hunger, Negative = reduce hunger")]
        private float hungerValue;
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI lifeExpectancyText;
        [SerializeField] private TextMeshProUGUI hungerValueText;
        [SerializeField] private TextMeshProUGUI itemNameText;
        
        [Header("UI Sliders")]
        [SerializeField] private Slider priceSlider;
        [SerializeField] private Slider lifeExpectancySlider;
        [SerializeField] private Slider hungerValueSlider;
        
        [Header("Shopping Manager Reference")]
        [SerializeField] private ShoppingManager shoppingManager;
        
        // Public getters
        public string ItemName => itemName;
        public float Price => price;
        public float LifeExpectancy => lifeExpectancy;
        public float HungerValue => hungerValue;
        
        private void Start()
        {
            // Try to find ShoppingManager if not assigned
            if (shoppingManager == null)
            {
                shoppingManager = FindObjectOfType<ShoppingManager>();
            }
            
            UpdateTextDisplays();
            UpdateSliders();
        }
        
        private void OnValidate()
        {
            // Update text displays in editor when values change
            UpdateTextDisplays();
            UpdateSliders();
        }
        
        private void UpdateTextDisplays()
        {
            if (itemNameText != null)
            {
                itemNameText.text = itemName;
            }
            
            if (priceText != null)
            {
                priceText.text = $"${price:F2}";
            }
            
            if (lifeExpectancyText != null)
            {
                if (lifeExpectancy >= 0)
                    lifeExpectancyText.text = $"+{lifeExpectancy:F0}";
                else
                    lifeExpectancyText.text = $"{lifeExpectancy:F0}";
            }
            
            if (hungerValueText != null)
            {
                if (hungerValue >= 0)
                    hungerValueText.text = $"+{hungerValue:F0}";
                else
                    hungerValueText.text = $"-{-hungerValue:F0}";
            }
        }
        
        private void UpdateSliders()
        {
            UpdatePriceSlider();
            UpdateLifeExpectancySlider();
            UpdateHungerValueSlider();
        }
        
        private void UpdatePriceSlider()
        {
            if (priceSlider != null && shoppingManager != null)
            {
                // Normalize price: price / startingMoney from ShoppingManager, clamped to 0-1
                float maxPrice = shoppingManager.StartingMoney;
                float normalizedValue = maxPrice > 0 ? Mathf.Clamp01(price / maxPrice) : 0f;
                priceSlider.value = normalizedValue;
            }
        }
        
        private void UpdateLifeExpectancySlider()
        {
            if (lifeExpectancySlider != null && shoppingManager != null)
            {
                // Normalize life expectancy: absolute value / startingHealth from ShoppingManager, clamped to 0-1
                float maxLifeExpectancy = shoppingManager.StartingHealth;
                float normalizedValue = maxLifeExpectancy > 0 ? Mathf.Clamp01(Mathf.Abs(lifeExpectancy) / maxLifeExpectancy) : 0f;
                lifeExpectancySlider.value = normalizedValue;
            }
        }
        
        private void UpdateHungerValueSlider()
        {
            if (hungerValueSlider != null && shoppingManager != null)
            {
                // Normalize hunger value: absolute value / maxHunger from ShoppingManager, clamped to 0-1
                float maxHungerValue = shoppingManager.MaxHunger;
                float normalizedValue = maxHungerValue > 0 ? Mathf.Clamp01(Mathf.Abs(hungerValue) / maxHungerValue) : 0f;
                hungerValueSlider.value = normalizedValue;
            }
        }
    }
}
