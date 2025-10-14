using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DomeSceneManager : MonoBehaviour
{
    [SerializeField] private float loadDelay = 0.5f; // Optional short delay before async load

    // Load a scene asynchronously by build index (callable from UnityEvent)
    public void LoadSceneByIndex(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneIndex));
    }

    // Load a scene asynchronously by name (also callable from UnityEvent)
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    private IEnumerator LoadSceneAsyncCoroutine(int sceneIndex)
    {
        Debug.Log($"Starting async load for scene index {sceneIndex}...");
        yield return new WaitForSeconds(loadDelay);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = true;

        // Optionally wait until loading completes
        while (!asyncLoad.isDone)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress * 100f}%");
            yield return null;
        }

        Debug.Log($"Scene index {sceneIndex} loaded.");
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        Debug.Log($"Starting async load for scene '{sceneName}'...");
        yield return new WaitForSeconds(loadDelay);

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
}
