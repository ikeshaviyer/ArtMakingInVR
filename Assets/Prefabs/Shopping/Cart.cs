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
        private Dictionary<Grocery, RigidbodyConstraints> originalConstraints = new Dictionary<Grocery, RigidbodyConstraints>();
        private Dictionary<Grocery, Vector3> originalScales = new Dictionary<Grocery, Vector3>();
        
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
            
            // Store original local scale before parenting
            originalScales[grocery] = grocery.transform.localScale;
            
            // Parent grocery to cart
            grocery.transform.SetParent(transform);
            
            // Halve the size
            grocery.transform.localScale = grocery.transform.localScale / 2f;
            
            // Lock X and Z axes on rigidbody if it exists
            Rigidbody rb = grocery.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Store original constraints
                originalConstraints[grocery] = rb.constraints;
                
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
            
            // Double the size back
            if (originalScales.ContainsKey(grocery))
            {
                grocery.transform.localScale = grocery.transform.localScale * 2f;
                originalScales.Remove(grocery);
            }
            
            // Unparent grocery from cart
            grocery.transform.SetParent(null);
            
            // Restore original constraints on rigidbody if it exists
            Rigidbody rb = grocery.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Restore original constraints if we stored them
                if (originalConstraints.ContainsKey(grocery))
                {
                    rb.constraints = originalConstraints[grocery];
                    originalConstraints.Remove(grocery);
                }
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
