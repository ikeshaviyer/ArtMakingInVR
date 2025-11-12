using UnityEngine;
using System.Collections.Generic;

namespace VRArtMaking
{
    public class GrocerySpawnController : MonoBehaviour
    {
        [Header("Spawn Points")]
        [SerializeField, Tooltip("List of Transform positions where grocery items will spawn")]
        private List<Transform> spawnPoints;
        
        [Header("Grocery Item Prefabs")]
        [SerializeField, Tooltip("List of grocery item prefabs that can be spawned")]
        private List<GameObject> groceryItemPrefabs;
        
        [Header("Spawn Settings")]
        [SerializeField, Tooltip("If true, randomly selects from prefab list. If false, cycles through sequentially")]
        private bool randomSpawn = true;
        
        [SerializeField, Tooltip("If true, spawns items on Start")]
        private bool spawnOnStart = true;
        
        [SerializeField, Tooltip("If true, clears existing items before respawning")]
        private bool clearBeforeRespawn = true;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private List<GameObject> spawnedItems = new List<GameObject>();
        
        private void Awake()
        {
            if (spawnPoints == null)
                spawnPoints = new List<Transform>();
            
            if (groceryItemPrefabs == null)
                groceryItemPrefabs = new List<GameObject>();
        }
        
        private void Start()
        {
            if (spawnOnStart)
            {
                SpawnAllItems();
            }
        }
        
        public void SpawnAllItems()
        {
            if (clearBeforeRespawn)
            {
                ClearSpawnedItems();
            }
            
            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                if (showDebugInfo)
                {
                    Debug.LogWarning("GrocerySpawnController: No spawn points assigned!");
                }
                return;
            }
            
            if (groceryItemPrefabs == null || groceryItemPrefabs.Count == 0)
            {
                if (showDebugInfo)
                {
                    Debug.LogWarning("GrocerySpawnController: No grocery item prefabs assigned!");
                }
                return;
            }
            
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint == null)
                {
                    if (showDebugInfo)
                    {
                        Debug.LogWarning("GrocerySpawnController: One or more spawn points are null!");
                    }
                    continue;
                }
                
                GameObject prefabToSpawn = GetPrefabToSpawn();
                if (prefabToSpawn == null)
                {
                    if (showDebugInfo)
                    {
                        Debug.LogWarning("GrocerySpawnController: No valid prefab to spawn!");
                    }
                    continue;
                }
                
                GameObject spawnedItem = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation, spawnPoint);
                spawnedItems.Add(spawnedItem);
                
                if (showDebugInfo)
                {
                    Debug.Log($"Spawned {prefabToSpawn.name} at {spawnPoint.name}");
                }
            }
        }
        
        private GameObject GetPrefabToSpawn()
        {
            if (groceryItemPrefabs.Count == 0)
                return null;
            
            if (randomSpawn)
            {
                return groceryItemPrefabs[Random.Range(0, groceryItemPrefabs.Count)];
            }
            else
            {
                // Cycle through prefabs sequentially
                int index = spawnedItems.Count % groceryItemPrefabs.Count;
                return groceryItemPrefabs[index];
            }
        }
        
        public void ClearSpawnedItems()
        {
            foreach (GameObject item in spawnedItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            spawnedItems.Clear();
            
            if (showDebugInfo)
            {
                Debug.Log("Cleared all spawned grocery items");
            }
        }
        
        public void RespawnAllItems()
        {
            ClearSpawnedItems();
            SpawnAllItems();
        }
        
        public void AddSpawnPoint(Transform spawnPoint)
        {
            if (spawnPoint != null && !spawnPoints.Contains(spawnPoint))
            {
                spawnPoints.Add(spawnPoint);
            }
        }
        
        public void RemoveSpawnPoint(Transform spawnPoint)
        {
            if (spawnPoints.Contains(spawnPoint))
            {
                spawnPoints.Remove(spawnPoint);
            }
        }
        
        public List<GameObject> GetSpawnedItems()
        {
            return new List<GameObject>(spawnedItems);
        }
        
        private void OnValidate()
        {
            // Initialize lists if null
            if (spawnPoints == null)
                spawnPoints = new List<Transform>();
            
            if (groceryItemPrefabs == null)
                groceryItemPrefabs = new List<GameObject>();
        }
    }
}

