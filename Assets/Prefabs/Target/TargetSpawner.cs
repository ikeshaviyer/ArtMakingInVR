using UnityEngine;
using System.Collections;

namespace VRArtMaking
{
    public class TargetSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject targetPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private int maxActiveTargets = 5;
        [SerializeField] private float targetLifetime = 10f;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private bool autoSpawn = true;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private int activeTargetCount = 0;
        private Coroutine spawnCoroutine;
        
        private void Start()
        {
            if (autoSpawn)
            {
                StartSpawning();
            }
        }
        
        public void StartSpawning()
        {
            if (spawnCoroutine == null)
            {
                spawnCoroutine = StartCoroutine(SpawnTargets());
            }
        }
        
        public void StopSpawning()
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
        
        private IEnumerator SpawnTargets()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);
                
                if (activeTargetCount < maxActiveTargets)
                {
                    SpawnTarget();
                }
            }
        }
        
        public GameObject SpawnTarget()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogError("No spawn points assigned to TargetSpawner!");
                return null;
            }
            
            if (targetPrefab == null)
            {
                Debug.LogError("No target prefab assigned to TargetSpawner!");
                return null;
            }
            
            // Pick a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            // Spawn the target
            GameObject newTarget = Instantiate(targetPrefab, spawnPoint.position, spawnPoint.rotation);
            
            // Set spawner reference so target can notify us when destroyed
            Target targetComponent = newTarget.GetComponent<Target>();
            if (targetComponent != null)
            {
                targetComponent.SetSpawner(this);
            }
            
            // Start auto-destroy coroutine
            StartCoroutine(AutoDestroyTarget(newTarget, targetLifetime));
            
            activeTargetCount++;
            
            if (showDebugInfo)
            {
                Debug.Log($"Spawned target. Active targets: {activeTargetCount}");
            }
            
            return newTarget;
        }
        
        public void OnTargetDestroyed()
        {
            activeTargetCount--;
            if (activeTargetCount < 0) activeTargetCount = 0;
            
            if (showDebugInfo)
            {
                Debug.Log($"Target destroyed. Active targets: {activeTargetCount}");
            }
        }
        
        public int GetActiveTargetCount()
        {
            return activeTargetCount;
        }
        
        public void SetMaxActiveTargets(int max)
        {
            maxActiveTargets = max;
        }
        
        public void SetTargetLifetime(float lifetime)
        {
            targetLifetime = lifetime;
        }
        
        public void SetSpawnInterval(float interval)
        {
            spawnInterval = interval;
        }
        
        private IEnumerator AutoDestroyTarget(GameObject target, float lifetime)
        {
            yield return new WaitForSeconds(lifetime);
            
            if (target != null)
            {
                OnTargetDestroyed();
                Destroy(target);
            }
        }
        
        public void ClearAllTargets()
        {
            Target[] allTargets = FindObjectsOfType<Target>();
            foreach (Target target in allTargets)
            {
                if (target != null && target.gameObject != null)
                {
                    Destroy(target.gameObject);
                }
            }
            activeTargetCount = 0;
        }
    }
}
