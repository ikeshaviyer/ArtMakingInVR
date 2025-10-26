using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Events;

public class InterdimensionalTransport : MonoBehaviour
{
    [SerializeField] private Material[] materials;
    
    [Header("Unity Events")]
    [SerializeField] private UnityEvent onPortalEnter;
    [SerializeField] private UnityEvent onPortalExit;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var mat in materials)
        {
            mat.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name != "Main Camera" && other.tag != "Player")
            return;
        
        Debug.Log("Player entered portal - showing other side");
        foreach (var mat in materials)
        {
            mat.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
        }
        
        // Invoke Unity Event
        onPortalEnter?.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        // if (other.name != "Main Camera")
        //     return;
        
        // Debug.Log("Player exited portal - showing portal content");
        // foreach (var mat in materials)
        // {
        //     mat.SetInt("_StencilTest", (int)CompareFunction.Equal);
        // }
        
        // // Invoke Unity Event
        // onPortalExit?.Invoke();
    }

    void OnDestroy()
    {
        foreach (var mat in materials)
        {
            mat.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
