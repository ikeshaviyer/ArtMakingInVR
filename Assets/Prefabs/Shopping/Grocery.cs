using UnityEngine;

namespace VRArtMaking
{
    public class Grocery : MonoBehaviour
    {
        [Header("Item Information")]
        [SerializeField] private string itemName;
        
        [Header("Item Stats")]
        [SerializeField] private float price;
        [SerializeField] private float badHealth;
        [SerializeField] private float hungerValue;
        
        // Public getters
        public string ItemName => itemName;
        public float Price => price;
        public float BadHealth => badHealth;
        public float HungerValue => hungerValue;
    }
}
