using UnityEngine;
using UnityEngine.Events;

namespace VRArtMaking
{
    public class PlayerTrigger : MonoBehaviour
    {
        [Header("Player Detection")]
        [SerializeField] private string playerTag = "Player";
        
        [Header("Events")]
        [SerializeField] private UnityEvent onPlayerEnter;
        [SerializeField] private UnityEvent onPlayerExit;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private bool playerInside = false;
        
        public bool PlayerInside => playerInside;
        
        private void OnTriggerEnter(Collider other)
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
        
        private void OnTriggerExit(Collider other)
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
