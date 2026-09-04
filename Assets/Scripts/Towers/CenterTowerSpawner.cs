using UnityEngine;

public class CenterTowerSpawner : MonoBehaviour
{
    [Header("References")]
    public TerrainGenerator terrainGenerator;
    public GameObject towerPrefab;
    public UIManager uiManager;

    public OrbitCameraController orbitCamera;

    private Tower currentTower;

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

        currentTower = tower.GetComponent<Tower>();
        if (currentTower != null)
        {
            currentTower.isCenterTower = true;
            currentTower.OnDeath += HandleCenterTowerDeath;
        }
    }

    private void HandleCenterTowerDeath()
    {
        currentTower.OnDeath -= HandleCenterTowerDeath; 
        if (uiManager != null)
        {
            uiManager.LostGame();
        }
    }
}
