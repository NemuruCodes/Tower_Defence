using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public TerrainGrid terrainGrid;
    public TerrainPathfinder terrainPathfinder;
    public GameObject enemyPrefab;
    public UIManager uiManager;

    [Header("Spawning")]
    public float spawnInterval = 1.5f;
    public int enemiesPerSpawner = 5;

    [Header("Wave Settings")]
    public int waveCount = 5;
    public int enemiesPerWave = 5;
    //public float spawnInterval = 1.5f;
    public float delayBetweenWaves = 5f;
    public float initialDelay = 2f;

    [Header("Events")]
    public Action<int> OnWaveStarted;      
    public Action<int> OnWaveCompleted;    
    public Action OnAllWavesCompleted;

    private Dictionary<Vector2Int, List<Vector2Int>> pathsBySpawner;
    private int currentWave = 0;
    private int aliveEnemies = 0;

    public int CurrentWave => currentWave;
    public bool IsSpawning { get; private set; }

    public TextMeshProUGUI waveCounterText;

    public void BeginSpawning()
    {
        pathsBySpawner = terrainPathfinder.PathsBySpawner;

        if (pathsBySpawner == null || pathsBySpawner.Count == 0)
        {
            Debug.LogWarning("EnemySpawner: no spawner paths available - nothing to spawn.");
            return;
        }

        UpdateWaveCounter();
        StartCoroutine(RunWaves());

        
    }

    private IEnumerator RunWaves()
    {
       
        IsSpawning = true;
        yield return new WaitForSeconds(initialDelay);

        

        for (currentWave = 1; currentWave <= waveCount; currentWave++)
        {
            OnWaveStarted?.Invoke(currentWave);
            UpdateWaveCounter();

            List<Coroutine> waveRoutines = new List<Coroutine>();
            foreach (KeyValuePair<Vector2Int, List<Vector2Int>> kvp in pathsBySpawner)
            {
                waveRoutines.Add(StartCoroutine(SpawnWaveAtSpawner(kvp.Key, kvp.Value)));
            }
            foreach (Coroutine routine in waveRoutines)
                yield return routine;

            yield return new WaitUntil(() => aliveEnemies <= 0);

            OnWaveCompleted?.Invoke(currentWave);

            if (currentWave < waveCount)
            {
                yield return new WaitForSeconds(delayBetweenWaves);
            }
            else if (currentWave >= waveCount)
            {
                uiManager.WonGame(); 
            }
            
        }

        IsSpawning = false;
        OnAllWavesCompleted?.Invoke();
    }

    private IEnumerator SpawnWaveAtSpawner(Vector2Int spawnerCell, List<Vector2Int> path)
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy(spawnerCell, path);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    /*
    private void SpawnEnemy(Vector2Int spawnerCell, List<Vector2Int> path)
    {
        if (enemyPrefab == null || path == null || path.Count == 0)
            return;

        float y = terrainGrid.GetSurfaceHeight(spawnerCell.x, spawnerCell.y) + 1f;
        Vector3 spawnPos = new Vector3(spawnerCell.x, y, spawnerCell.y);

        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        EnemyStateMachine stateMachine = enemyObj.GetComponent<EnemyStateMachine>();
        if (stateMachine == null)
            stateMachine = enemyObj.AddComponent<EnemyStateMachine>();

        stateMachine.Initialize(terrainGrid, path);
        //stateMachine.OnTargetReached += () => HandleEnemyReachedTarget(enemyObj);
        stateMachine.OnDeath += () => HandleEnemyDeath(enemyObj);
    }
    */

    private void SpawnEnemy(Vector2Int spawnerCell, List<Vector2Int> path)
    {
        if (enemyPrefab == null || path == null || path.Count == 0)
            return;

        float y = terrainGrid.GetSurfaceHeight(spawnerCell.x, spawnerCell.y) + 1f;
        Vector3 spawnPos = new Vector3(spawnerCell.x, y, spawnerCell.y);

        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        EnemyStateMachine stateMachine = enemyObj.GetComponent<EnemyStateMachine>();
        if (stateMachine == null)
            stateMachine = enemyObj.AddComponent<EnemyStateMachine>();

        stateMachine.Initialize(terrainGrid, path);
        aliveEnemies++;

        //stateMachine.OnTargetReached += () => HandleEnemyReachedTarget(enemyObj);
        stateMachine.OnDeath += () => HandleEnemyDeath(enemyObj);
    }


    private void HandleEnemyDeath(GameObject enemyObj)
    {
        aliveEnemies--;
        //Destroy(enemyObj);
    }

    private void UpdateWaveCounter()
    {
        waveCounterText.text = $"Wave: {CurrentWave} / {waveCount}";
    }

}
