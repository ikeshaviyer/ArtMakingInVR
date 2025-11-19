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
        private Transform targetTransform;
        
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
            
            // Find Target tagged object
            GameObject targetObject = GameObject.FindGameObjectWithTag("Target");
            if (targetObject != null)
            {
                targetTransform = targetObject.transform;
            }
            else if (showDebugInfo)
            {
                Debug.LogWarning("Cart: No GameObject with 'Target' tag found in scene!");
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
            
            // Parent grocery to Target tagged object
            if (targetTransform != null)
            {
                grocery.transform.SetParent(targetTransform);
            }
            
            // Lock X and Z axes on rigidbody if it exists
            Rigidbody rb = grocery.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Lock X and Z position (allow Y to move if needed)
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            }
            
            // Subtract money, add life expectancy and hunger when item is added to cart
            if (shoppingManager != null)
            {
                shoppingManager.SubtractMoney(grocery.Price);
                shoppingManager.AddLifeExpectancy(grocery.LifeExpectancy);
                shoppingManager.AddHunger(grocery.HungerValue);
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"Added {grocery.ItemName} to cart. Subtracted Price: ${grocery.Price}, Added LifeExpectancy: {grocery.LifeExpectancy}, Added Hunger: {grocery.HungerValue}");
            }
        }
        
        private void RemoveGroceryFromCart(Grocery grocery)
        {
            groceriesInCart.Remove(grocery);
            
            // Unparent grocery from Target
            grocery.transform.SetParent(null);
            
            // Remove constraints on rigidbody if it exists
            Rigidbody rb = grocery.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Remove X and Z position constraints
                rb.constraints = RigidbodyConstraints.None;
            }
            
            // Add back money, subtract life expectancy and hunger when item is removed from cart
            if (shoppingManager != null)
            {
                shoppingManager.AddMoney(grocery.Price);
                shoppingManager.SubtractLifeExpectancy(grocery.LifeExpectancy);
                shoppingManager.SubtractHunger(grocery.HungerValue);
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"Removed {grocery.ItemName} from cart. Added back Price: ${grocery.Price}, Subtracted LifeExpectancy: {grocery.LifeExpectancy}, Subtracted Hunger: {grocery.HungerValue}");
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
        /// Get total life expectancy of all items in cart
        /// </summary>
        public float GetTotalLifeExpectancy()
        {
            float total = 0f;
            foreach (Grocery grocery in groceriesInCart)
            {
                total += grocery.LifeExpectancy;
            }
            return total;
        }
    }
}
