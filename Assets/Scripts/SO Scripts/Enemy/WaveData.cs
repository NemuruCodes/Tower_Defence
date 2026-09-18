using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WaveSpawnEntry
{
    public EnemyData enemyType;
    public int count = 5;
    public float spawnInterval = 1.5f;
}

[CreateAssetMenu(menuName = "TowerDefense/WaveData", fileName = "NewWaveData")]
public class WaveData : ScriptableObject
{
    [Header("Identity")]
    public string waveName;

    [Header("Composition")]
    public List<WaveSpawnEntry> spawnEntries = new List<WaveSpawnEntry>();

    [Header("Timing")]
    [Tooltip("Delay after the previous wave fully clears before this wave starts spawning.")]
    public float delayBeforeWave = 5f;
}
