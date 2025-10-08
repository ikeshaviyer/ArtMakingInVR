using UnityEngine;
using System.Collections.Generic;

namespace VRArtMaking
{
    public class Cart : MonoBehaviour
    {
        [Header("Shopping Manager Reference")]
        [SerializeField] private ShoppingManager shoppingManager;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private List<Grocery> groceriesInCart = new List<Grocery>();
        
        public List<Grocery> GroceriesInCart => groceriesInCart;
        
        private void Start()
        {
            // Try to find ShoppingManager if not assigned
            if (shoppingManager == null)
            {
                shoppingManager = FindObjectOfType<ShoppingManager>();
                
                if (shoppingManager == null && showDebugInfo)
                {
                    Debug.LogWarning("Cart: No ShoppingManager found in scene!");
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            Grocery grocery = other.GetComponent<Grocery>();
            
            if (grocery != null && !groceriesInCart.Contains(grocery))
            {
                AddGroceryToCart(grocery);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            Grocery grocery = other.GetComponent<Grocery>();
            
            if (grocery != null && groceriesInCart.Contains(grocery))
            {
                RemoveGroceryFromCart(grocery);
            }
        }
        
        private void AddGroceryToCart(Grocery grocery)
        {
            groceriesInCart.Add(grocery);
            
            // Subtract money and health, add hunger when item is added to cart
            if (shoppingManager != null)
            {
                shoppingManager.SubtractMoney(grocery.Price);
                shoppingManager.SubtractHealth(grocery.BadHealth);
                shoppingManager.AddHunger(grocery.HungerValue);
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"Added {grocery.ItemName} to cart. Subtracted Price: ${grocery.Price}, BadHealth: {grocery.BadHealth}, Added Hunger: {grocery.HungerValue}");
            }
        }
        
        private void RemoveGroceryFromCart(Grocery grocery)
        {
            groceriesInCart.Remove(grocery);
            
            // Add back money and health, subtract hunger when item is removed from cart
            if (shoppingManager != null)
            {
                shoppingManager.AddMoney(grocery.Price);
                shoppingManager.AddHealth(grocery.BadHealth);
                shoppingManager.SubtractHunger(grocery.HungerValue);
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"Removed {grocery.ItemName} from cart. Added back Price: ${grocery.Price}, BadHealth: {grocery.BadHealth}, Subtracted Hunger: {grocery.HungerValue}");
            }
        }
        
        
        /// <summary>
        /// Clear all items from cart
        /// </summary>
        public void ClearCart()
        {
            groceriesInCart.Clear();
            
            if (showDebugInfo)
            {
                Debug.Log("Cart cleared");
            }
        }
        
        /// <summary>
        /// Get total price of all items in cart
        /// </summary>
        public float GetTotalPrice()
        {
            float total = 0f;
            foreach (Grocery grocery in groceriesInCart)
            {
                total += grocery.Price;
            }
            return total;
        }
        
        /// <summary>
        /// Get total badHealth of all items in cart
        /// </summary>
        public float GetTotalBadHealth()
        {
            float total = 0f;
            foreach (Grocery grocery in groceriesInCart)
            {
                total += grocery.BadHealth;
            }
            return total;
        }
    }
}
