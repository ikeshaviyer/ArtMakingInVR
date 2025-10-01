using UnityEngine;
using UnityEngine.SceneManagement;
using Oculus.Interaction;

/// <summary>
/// Simple scene transition trigger that activates when the object is grabbed in VR.
/// Works with Oculus Interaction Grabbable components.
/// </summary>
public class GrabSceneTransition : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load when this object is grabbed")]
    [SerializeField] private string sceneToLoad = "NextScene";
    
    [Tooltip("Delay before starting the scene transition (in seconds)")]
    [SerializeField] private float transitionDelay = 0.5f;
    
    [Header("Transition Settings")]
    [Tooltip("Use fade transition effect (requires VRSceneManager)")]
    [SerializeField] private bool useFadeTransition = true;
    
    [Tooltip("Use the VRSceneManager for advanced transitions")]
    [SerializeField] private bool useSceneManager = true;
    
    [Tooltip("Allow only one trigger per object")]
    [SerializeField] private bool triggerOnce = true;
    
    private Grabbable grabbable;
    private bool hasTriggered = false;
    
    void Start()
    {
        // Get the Grabbable component
        grabbable = GetComponent<Grabbable>();
        
        if (grabbable == null)
        {
            Debug.LogError($"GrabSceneTransition: No Grabbable component found on {gameObject.name}. " +
                          "Please add an Oculus Grabbable component.", this);
            enabled = false;
            return;
        }
        
        // Validate scene name
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning($"GrabSceneTransition: Scene name is empty on {gameObject.name}. " +
                           "Please set the scene name in the inspector.", this);
        }
        
        // Subscribe to grab events
        grabbable.WhenPointerEventRaised += OnGrabEvent;
    }
    
    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (grabbable != null)
        {
            grabbable.WhenPointerEventRaised -= OnGrabEvent;
        }
    }
    
    /// <summary>
    /// Handle grab events from Oculus Interaction
    /// </summary>
    private void OnGrabEvent(PointerEvent pointerEvent)
    {
        // Check if this is a grab event (not release)
        if (pointerEvent.Type == PointerEventType.Select && (!triggerOnce || !hasTriggered))
        {
            hasTriggered = true;
            Debug.Log($"Object '{gameObject.name}' grabbed! Transitioning to scene: {sceneToLoad}");
            
            // Start transition with optional delay
            if (transitionDelay > 0)
            {
                Invoke(nameof(LoadScene), transitionDelay);
            }
            else
            {
                LoadScene();
            }
        }
    }
    
    /// <summary>
    /// Load the scene
    /// </summary>
    private void LoadScene()
    {
        // Validate scene name
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("Scene name is empty! Please set the scene name in the inspector.", this);
            return;
        }
        
        // Clean up VR components before transition
        CleanupVRComponents();
        
        // Use VRSceneManager if available and requested
        if (useSceneManager && VRSceneManager.Instance != null)
        {
            VRSceneManager.Instance.LoadScene(sceneToLoad, 0f, useFadeTransition);
        }
        else
        {
            // Fallback to basic Unity scene loading
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    
    /// <summary>
    /// Clean up VR-specific components before scene transition
    /// </summary>
    private void CleanupVRComponents()
    {
        // Disable GrabAndLocate components to prevent null reference errors
        var grabAndLocates = FindObjectsOfType<Meta.XR.MRUtilityKit.BuildingBlocks.GrabAndLocate>();
        foreach (var component in grabAndLocates)
        {
            if (component != null)
            {
                component.enabled = false;
            }
        }
    }
    
    /// <summary>
    /// Public method to manually trigger the transition (useful for debugging)
    /// </summary>
    public void ManuallyTriggerTransition()
    {
        if (!triggerOnce || !hasTriggered)
        {
            hasTriggered = true;
            LoadScene();
        }
    }
    
    /// <summary>
    /// Reset the trigger state (useful if triggerOnce is enabled)
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    #if UNITY_EDITOR
    /// <summary>
    /// Draw gizmo in editor to show this is a transition trigger
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
    #endif
}


