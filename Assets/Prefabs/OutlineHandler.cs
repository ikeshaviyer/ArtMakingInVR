using UnityEngine;

public class OutlineHandler : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private Color outlineColor = Color.magenta;
    [SerializeField] private float outlineWidth = 7.0f;
    
    private Outline outline;
    
    private void Awake()
    {
        // Get or add Outline component (searches self and children)
        outline = GetComponentInChildren<Outline>();
        
        // Configure outline
        outline.OutlineColor = outlineColor;
        outline.OutlineWidth = outlineWidth;
        outline.enabled = false;
    }
    
    // PUBLIC METHODS - Called from GrabbableEvents in Inspector
    public void EnableOutline()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }
    
    public void DisableOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}

