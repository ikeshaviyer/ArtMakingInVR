using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton Scene Manager for VR applications.
/// Handles scene loading, transitions, and state management.
/// </summary>
public class VRSceneManager : MonoBehaviour
{
    private static VRSceneManager _instance;
    public static VRSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("VRSceneManager");
                _instance = go.AddComponent<VRSceneManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [Header("Fade Settings")]
    [SerializeField] private bool useFadeTransitions = true;
    [SerializeField] private float defaultFadeDuration = 1f;
    [SerializeField] private Color fadeColor = Color.black;

    [Header("Audio Settings")]
    [SerializeField] private bool muteAudioDuringTransition = true;
    
    [Header("Debug Settings")]
    [SerializeField] private bool debugMode = true;

    private bool isTransitioning = false;
    private Canvas fadeCanvas;
    private CanvasGroup fadeCanvasGroup;
    private UnityEngine.UI.Image fadeImage;

    public bool IsTransitioning => isTransitioning;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        SetupFadeCanvas();
    }

    /// <summary>
    /// Sets up the fade canvas for scene transitions
    /// </summary>
    private void SetupFadeCanvas()
    {
        // Create canvas for fade effect
        GameObject canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.SetParent(transform);
        
        fadeCanvas = canvasObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 9999; // Ensure it's on top
        
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        fadeCanvasGroup = canvasObj.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
        
        // Create image for fade
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        
        fadeImage = imageObj.AddComponent<UnityEngine.UI.Image>();
        fadeImage.color = fadeColor;
        
        RectTransform rectTransform = imageObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        
        canvasObj.SetActive(false);
    }

    /// <summary>
    /// Load a scene by name with optional fade transition
    /// </summary>
    public void LoadScene(string sceneName, float delay = 0f, bool useFade = true)
    {
        if (isTransitioning)
        {
            if (debugMode) Debug.LogWarning("Scene transition already in progress!");
            return;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty!");
            return;
        }

        StartCoroutine(LoadSceneCoroutine(sceneName, delay, useFade));
    }

    /// <summary>
    /// Load a scene by build index with optional fade transition
    /// </summary>
    public void LoadScene(int sceneIndex, float delay = 0f, bool useFade = true)
    {
        if (isTransitioning)
        {
            if (debugMode) Debug.LogWarning("Scene transition already in progress!");
            return;
        }

        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"Invalid scene index: {sceneIndex}");
            return;
        }

        StartCoroutine(LoadSceneCoroutine(sceneIndex, delay, useFade));
    }

    /// <summary>
    /// Reload the current scene
    /// </summary>
    public void ReloadCurrentScene(float delay = 0f, bool useFade = true)
    {
        LoadScene(SceneManager.GetActiveScene().name, delay, useFade);
    }

    /// <summary>
    /// Load the next scene in build settings
    /// </summary>
    public void LoadNextScene(float delay = 0f, bool useFade = true)
    {
        int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        LoadScene(nextSceneIndex, delay, useFade);
    }

    /// <summary>
    /// Load the previous scene in build settings
    /// </summary>
    public void LoadPreviousScene(float delay = 0f, bool useFade = true)
    {
        int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        if (previousSceneIndex < 0) previousSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
        LoadScene(previousSceneIndex, delay, useFade);
    }

    /// <summary>
    /// Coroutine to load scene by name
    /// </summary>
    private IEnumerator LoadSceneCoroutine(string sceneName, float delay, bool useFade)
    {
        isTransitioning = true;

        if (debugMode) Debug.Log($"Loading scene: {sceneName}");

        // Wait for delay
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        // Clean up VR-specific components before transition
        CleanupBeforeSceneTransition();

        // Fade out
        if (useFade && useFadeTransitions)
        {
            yield return StartCoroutine(FadeOut(defaultFadeDuration));
        }

        // Load the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade in
        if (useFade && useFadeTransitions)
        {
            yield return StartCoroutine(FadeIn(defaultFadeDuration));
        }

        isTransitioning = false;

        if (debugMode) Debug.Log($"Scene loaded: {sceneName}");
    }

    /// <summary>
    /// Coroutine to load scene by index
    /// </summary>
    private IEnumerator LoadSceneCoroutine(int sceneIndex, float delay, bool useFade)
    {
        isTransitioning = true;

        if (debugMode) Debug.Log($"Loading scene index: {sceneIndex}");

        // Wait for delay
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        // Clean up VR-specific components before transition
        CleanupBeforeSceneTransition();

        // Fade out
        if (useFade && useFadeTransitions)
        {
            yield return StartCoroutine(FadeOut(defaultFadeDuration));
        }

        // Load the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade in
        if (useFade && useFadeTransitions)
        {
            yield return StartCoroutine(FadeIn(defaultFadeDuration));
        }

        isTransitioning = false;

        if (debugMode) Debug.Log($"Scene loaded: {SceneManager.GetActiveScene().name}");
    }

    /// <summary>
    /// Fade out effect
    /// </summary>
    private IEnumerator FadeOut(float duration)
    {
        if (fadeCanvas != null)
        {
            fadeCanvas.gameObject.SetActive(true);
            fadeCanvasGroup.blocksRaycasts = true;
            
            float elapsed = 0f;
            float previousVolume = AudioListener.volume;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                fadeCanvasGroup.alpha = t;

                if (muteAudioDuringTransition)
                {
                    AudioListener.volume = Mathf.Lerp(previousVolume, 0f, t);
                }

                yield return null;
            }

            fadeCanvasGroup.alpha = 1f;
            if (muteAudioDuringTransition)
            {
                AudioListener.volume = 0f;
            }
        }
    }

    /// <summary>
    /// Fade in effect
    /// </summary>
    private IEnumerator FadeIn(float duration)
    {
        if (fadeCanvas != null)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                fadeCanvasGroup.alpha = 1f - t;

                if (muteAudioDuringTransition)
                {
                    AudioListener.volume = Mathf.Lerp(0f, 1f, t);
                }

                yield return null;
            }

            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
            fadeCanvas.gameObject.SetActive(false);

            if (muteAudioDuringTransition)
            {
                AudioListener.volume = 1f;
            }
        }
    }

    /// <summary>
    /// Clean up VR-specific components before scene transition
    /// </summary>
    private void CleanupBeforeSceneTransition()
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

        // You can add more cleanup logic here for other VR components
    }

    /// <summary>
    /// Quit the application
    /// </summary>
    public void QuitApplication(float delay = 0f)
    {
        StartCoroutine(QuitCoroutine(delay));
    }

    private IEnumerator QuitCoroutine(float delay)
    {
        if (delay > 0f)
        {
            if (useFadeTransitions)
            {
                yield return StartCoroutine(FadeOut(defaultFadeDuration));
            }
            yield return new WaitForSeconds(delay);
        }

        if (debugMode) Debug.Log("Quitting application...");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    /// <summary>
    /// Get the current scene name
    /// </summary>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Get the current scene build index
    /// </summary>
    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    /// <summary>
    /// Check if a scene exists in build settings
    /// </summary>
    public bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}


