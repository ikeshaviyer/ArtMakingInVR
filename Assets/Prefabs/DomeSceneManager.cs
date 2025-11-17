using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DomeSceneManager : MonoBehaviour
{
    [SerializeField] private float loadDelay = 3f; // Optional short delay before async load
    [SerializeField] private CanvasGroup blackoutPanel; // CanvasGroup that covers the screen for blackout effect
    [SerializeField] private float fadeDuration = 0.5f; // Duration of fade to black

    private void Start()
    {
        // Fade to clear when scene loads
        StartCoroutine(FadeToClear());
    }

    // Load a scene asynchronously by name (callable from UnityEvent)
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    // Load a scene asynchronously by name without delay (callable from UnityEvent)
    public void LoadSceneByNameNoDelay(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncNoDelayCoroutine(sceneName));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        Debug.Log($"Starting async load for scene '{sceneName}'...");
        yield return new WaitForSeconds(loadDelay);

        // Fade to black before loading
        yield return StartCoroutine(FadeToBlack());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        // Optionally wait until loading completes
        while (!asyncLoad.isDone)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress * 100f}%");
            yield return null;
        }

        Debug.Log($"Scene '{sceneName}' loaded.");
    }

    private IEnumerator LoadSceneAsyncNoDelayCoroutine(string sceneName)
    {
        Debug.Log($"Starting async load for scene '{sceneName}'...");

        // Fade to black before loading
        yield return StartCoroutine(FadeToBlack());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        // Optionally wait until loading completes
        while (!asyncLoad.isDone)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress * 100f}%");
            yield return null;
        }

        Debug.Log($"Scene '{sceneName}' loaded.");
    }

    private IEnumerator FadeToBlack()
    {
        if (blackoutPanel == null)
        {
            Debug.LogWarning("Blackout panel not assigned. Skipping fade to black.");
            yield break;
        }

        // Ensure the panel is active and set to transparent initially
        blackoutPanel.gameObject.SetActive(true);
        blackoutPanel.alpha = 0f;

        // Fade to black
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            blackoutPanel.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        // Ensure fully black
        blackoutPanel.alpha = 1f;
    }

    private IEnumerator FadeToClear()
    {
        if (blackoutPanel == null)
        {
            yield break;
        }

        // Ensure the panel is active
        blackoutPanel.gameObject.SetActive(true);
        
        // Start from current alpha (or fully black if not set)
        float startAlpha = blackoutPanel.alpha;
        
        // If already clear, no need to fade
        if (startAlpha <= 0f)
        {
            blackoutPanel.alpha = 0f;
            yield break;
        }

        // Fade to clear
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            blackoutPanel.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        // Ensure fully clear
        blackoutPanel.alpha = 0f;
    }
}
