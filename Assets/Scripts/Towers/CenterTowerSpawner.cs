using UnityEngine;

public class CenterTowerSpawner : MonoBehaviour
{
    [Header("References")]
    public TerrainGenerator terrainGenerator;
    public GameObject towerPrefab;

    public OrbitCameraController orbitCamera;

    private void OnEnable()
    {
        terrainGenerator.OnTowerSpawnPointReady += SpawnTower;
    }

    private void OnDisable()
    {
        terrainGenerator.OnTowerSpawnPointReady -= SpawnTower;
    }

    private void SpawnTower(Vector3 spawnPos)
    {
        if (towerPrefab == null) return;

        GameObject tower = Instantiate(towerPrefab, spawnPos, Quaternion.identity);

        if (orbitCamera != null)
        {
            orbitCamera.SetTarget(tower.transform);
        }
    }
}
