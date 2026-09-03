using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public TerrainGrid terrainGrid;
    public TerrainPathfinder terrainPathfinder;
    public GameObject enemyPrefab;

    [Header("Spawning")]
    public float spawnInterval = 1.5f;
    public int enemiesPerSpawner = 5;

    private Dictionary<Vector2Int, List<Vector2Int>> pathsBySpawner;

    public void BeginSpawning()
    {
        pathsBySpawner = terrainPathfinder.PathsBySpawner;

        if (pathsBySpawner == null || pathsBySpawner.Count == 0)
        {
            Debug.LogWarning("EnemySpawner: no spawner paths available - nothing to spawn.");
            return;
        }

        foreach (KeyValuePair<Vector2Int, List<Vector2Int>> kvp in pathsBySpawner)
        {
            StartCoroutine(SpawnWave(kvp.Key, kvp.Value));
        }
    }

    private IEnumerator SpawnWave(Vector2Int spawnerCell, List<Vector2Int> path)
    {
        for (int i = 0; i < enemiesPerSpawner; i++)
        {
            SpawnEnemy(spawnerCell, path);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy(Vector2Int spawnerCell, List<Vector2Int> path)
    {
        if (enemyPrefab == null || path == null || path.Count == 0)
            return;

        float y = terrainGrid.GetSurfaceHeight(spawnerCell.x, spawnerCell.y) + 1f;
        Vector3 spawnPos = new Vector3(spawnerCell.x, y, spawnerCell.y);

        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        EnemyMover mover = enemyObj.GetComponent<EnemyMover>();
        if (mover == null)
            mover = enemyObj.AddComponent<EnemyMover>();

        mover.Initialize(terrainGrid, path);
        mover.OnTargetReached += () => HandleEnemyReachedTarget(enemyObj);
    }

    private void HandleEnemyReachedTarget(GameObject enemyObj)
    {
        // Hook up damage-to-target / scoring logic here before removing the enemy.
        Destroy(enemyObj);
    }
}
