using UnityEngine;
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
        
        // Public getters
        public string ItemName => itemName;
        public float Price => price;
        public float LifeExpectancy => lifeExpectancy;
        public float HungerValue => hungerValue;
        
        private void Start()
        {
            UpdateTextDisplays();
        }
        
        private void OnValidate()
        {
            // Update text displays in editor when values change
            UpdateTextDisplays();
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
                    lifeExpectancyText.text = $"Life: +{lifeExpectancy:F0}";
                else
                    lifeExpectancyText.text = $"Life: {lifeExpectancy:F0}";
            }
            
            if (hungerValueText != null)
            {
                if (hungerValue >= 0)
                    hungerValueText.text = $"Hunger: +{hungerValue:F0}";
                else
                    hungerValueText.text = $"Hunger: -{-hungerValue:F0}";
            }
        }
    }
}
