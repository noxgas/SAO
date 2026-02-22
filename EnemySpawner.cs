using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Spawns AIEnemy instances around the player at regular intervals.
/// Maintains a configurable pool cap and respawns when enemies are killed.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerTransform;

    [Header("Spawn Settings")]
    [SerializeField] private int maxEnemies = 10;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float minSpawnDistance = 15f;
    [SerializeField] private float maxSpawnDistance = 40f;
    [Tooltip("Vertical raycast height offset used to place enemies on terrain.")]
    [SerializeField] private float raycastHeight = 100f;
    [SerializeField] private LayerMask terrainLayerMask = ~0;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float spawnTimer;

    private void Start()
    {
        if (playerTransform == null && Camera.main != null)
            playerTransform = Camera.main.transform;

        // Seed initial enemies
        StartCoroutine(InitialSpawn());
    }

    private IEnumerator InitialSpawn()
    {
        yield return null; // wait one frame for terrain colliders to settle
        for (int i = 0; i < Mathf.Min(3, maxEnemies); i++)
            TrySpawnEnemy();
    }

    private void Update()
    {
        // Remove dead entries
        activeEnemies.RemoveAll(e => e == null);

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            if (activeEnemies.Count < maxEnemies)
                TrySpawnEnemy();
        }
    }

    private void TrySpawnEnemy()
    {
        if (enemyPrefab == null || playerTransform == null) return;

        Vector3 spawnPos;
        if (!FindSpawnPosition(out spawnPos)) return;

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        activeEnemies.Add(enemy);
    }

    private bool FindSpawnPosition(out Vector3 position)
    {
        // Try up to 10 random positions
        for (int attempt = 0; attempt < 10; attempt++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float dist  = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 offset = new Vector3(Mathf.Cos(angle) * dist, 0f, Mathf.Sin(angle) * dist);
            Vector3 testPos = playerTransform.position + offset;
            testPos.y += raycastHeight;

            if (Physics.Raycast(testPos, Vector3.down, out RaycastHit hit, raycastHeight * 2f, terrainLayerMask))
            {
                position = hit.point;
                return true;
            }
        }

        // Fallback: flat world
        float a = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float d = Random.Range(minSpawnDistance, maxSpawnDistance);
        position = playerTransform.position + new Vector3(Mathf.Cos(a) * d, 0f, Mathf.Sin(a) * d);
        return true;
    }

    /// <summary>Immediately clear all active enemies (useful for testing).</summary>
    public void DespawnAll()
    {
        foreach (var e in activeEnemies)
        {
            if (e != null) Destroy(e);
        }
        activeEnemies.Clear();
    }
}
