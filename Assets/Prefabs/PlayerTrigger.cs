using UnityEngine;
using UnityEngine.Events;

namespace VRArtMaking
{
    public class PlayerTrigger : MonoBehaviour
    {
        [Header("Player Detection")]
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private Collider triggerCollider;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onPlayerEnter;
        [SerializeField] private UnityEvent onPlayerExit;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private bool playerInside = false;
        
        public bool PlayerInside => playerInside;
        
        private void Start()
        {
            // If a specific collider is assigned, ensure it's set up as a trigger
            if (triggerCollider != null)
            {
                if (!triggerCollider.isTrigger)
                {
                    Debug.LogWarning($"Assigned collider on {gameObject.name} is not set as a trigger. Setting it now.");
                    triggerCollider.isTrigger = true;
                }
            }
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                playerInside = true;
                
                if (showDebugInfo)
                {
                    Debug.Log($"Player entered trigger: {gameObject.name}");
                }
                
                onPlayerEnter?.Invoke();
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                playerInside = false;
                
                if (showDebugInfo)
                {
                    Debug.Log($"Player exited trigger: {gameObject.name}");
                }
                
                onPlayerExit?.Invoke();
            }
        }
    }
}
