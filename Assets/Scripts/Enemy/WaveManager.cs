using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;


// Owns wave timing and delegates actual spawning
// Alive count tracking to Enemy Spawner
public class WaveManager : MonoBehaviour
{
    [Header("References")]
    public TerrainPathfinder terrainPathfinder;
    public EnemySpawner enemySpawner;
    public UIManager uiManager;
    public TextMeshProUGUI waveCounterText;

    [Header("Waves")]
    [Tooltip("Assign WaveData assets in order. Max 5 for this project.")]
    public List<WaveData> waves = new List<WaveData>();

    [Header("Timing")]
    public float delayBetweenWaves = 5f;
    public float initialDelay = 2f;

    [Header("Events")]
    public Action<int> OnWaveStarted;
    public Action<int> OnWaveCompleted;
    public Action OnAllWavesCompleted;

    private Dictionary<Vector2Int, List<Vector2Int>> pathsBySpawner;
    private int currentWave = 0;

    public int CurrentWave => currentWave;
    public int WaveCount => waves.Count;
    public bool IsSpawning { get; private set; }

    public void BeginSpawning()
    {
        pathsBySpawner = terrainPathfinder.PathsBySpawner;

        if (pathsBySpawner == null || pathsBySpawner.Count == 0)
        {
            Debug.LogWarning("WaveManager: no spawner paths available - nothing to spawn.");
            return;
        }

        if (waves.Count == 0)
        {
            Debug.LogWarning("WaveManager: no WaveData assigned.");
            return;
        }

        UpdateWaveCounter();
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        IsSpawning = true;
        yield return new WaitForSeconds(initialDelay);

        for (currentWave = 1; currentWave <= waves.Count; currentWave++)
        {
            WaveData wave = waves[currentWave - 1];

            yield return new WaitForSeconds(wave.delayBeforeWave);

            OnWaveStarted?.Invoke(currentWave);
            UpdateWaveCounter();

            List<Coroutine> spawnRoutines = new List<Coroutine>();
            foreach (WaveSpawnEntry entry in wave.spawnEntries)
            {
                spawnRoutines.Add(StartCoroutine(SpawnEntryAcrossPaths(entry)));
            }
            foreach (Coroutine routine in spawnRoutines)
                yield return routine;

            yield return new WaitUntil(() => enemySpawner.AliveCount <= 0);

            OnWaveCompleted?.Invoke(currentWave);

            if (currentWave < waves.Count)
                yield return new WaitForSeconds(delayBetweenWaves);
            else
                uiManager.WonGame();
        }

        IsSpawning = false;
        OnAllWavesCompleted?.Invoke();
    }

    // Round-robins one entry's enemies across all available spawner paths
    // so a single wave's enemy type isn't dumped down just one path.
    private IEnumerator SpawnEntryAcrossPaths(WaveSpawnEntry entry)
    {
        List<Vector2Int> spawnerCells = new List<Vector2Int>(pathsBySpawner.Keys);
        int pathIndex = 0;

        for (int i = 0; i < entry.count; i++)
        {
            Vector2Int spawnerCell = spawnerCells[pathIndex];
            List<Vector2Int> path = pathsBySpawner[spawnerCell];

            enemySpawner.SpawnEnemy(entry.enemyType, spawnerCell, path);

            pathIndex = (pathIndex + 1) % spawnerCells.Count;
            yield return new WaitForSeconds(entry.spawnInterval);
        }
    }

    private void UpdateWaveCounter()
    {
        if (waveCounterText != null)
            waveCounterText.text = $"Wave: {CurrentWave} / {waves.Count}";
    }
}
