using UnityEngine;

//https://youtube.com/playlist?list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&si=hIrlMtp2-izvw6W3 - Procedural Terrain Generation playlist

public class TerrainRenderer : MonoBehaviour
{
    [Header("References")]
    public TerrainGrid terrainGrid;

    [Header("Rendering")]
    public GameObject groundPrefab;
    public GameObject waterPrefab;
    public GameObject mountainPrefab;
    public GameObject targetPrefab;  
    public GameObject spawnerPrefab; 
    public GameObject pathPrefab;
    public GameObject slowPrefab;
    public GameObject fastPrefab;

    [Header("Path Markers")]
    public GameObject slowPathMarkerPrefab;
    public GameObject fastPathMarkerPrefab;
    public GameObject mountainPathMarkerPrefab;
    public Vector3 pathMarkerOffset = new Vector3(0f, 0.55f, 0f);

    public void RenderTerrain()
    {
        ClearTerrain();

        for (int x = 0; x < terrainGrid.width; x++)
        {
        
            for (int y = 0; y < terrainGrid.height; y++)
            {
                for (int z = 0; z < terrainGrid.depth; z++)
                {
                    TerrainCell cell = terrainGrid.GetCell(x, y, z);

                    if (cell == null)
                        continue;

                    if (!terrainGrid.IsExposedToAir(x, y, z))
                        continue;

                    GameObject prefab = GetPrefab(cell.type);

                    if (prefab == null)
                        continue;

                    Instantiate(prefab,cell.position,Quaternion.identity,transform);

                    if (cell.isOnPath)
                    {
                        GameObject marker = null;

                        if (cell.type == TerrainType.Slow) marker = slowPathMarkerPrefab;
                        else if (cell.type == TerrainType.Fast) marker = fastPathMarkerPrefab;
                        else if (cell.type == TerrainType.Mountain) marker = mountainPathMarkerPrefab;

                        if (marker != null)
                            Instantiate(marker, (Vector3)cell.position + pathMarkerOffset, Quaternion.identity, transform);
                    }
                }
            }
        }
    }


    private GameObject GetPrefab(TerrainType type)
    {
        switch (type)
        {
            case TerrainType.Ground:
                return groundPrefab;

            case TerrainType.Water:
                return waterPrefab;

            case TerrainType.Mountain:
                return mountainPrefab;

            case TerrainType.Target:
                return targetPrefab;

            case TerrainType.Spawner:
                return spawnerPrefab;

            case TerrainType.Path:
                return pathPrefab;

            case TerrainType.Slow:
                return slowPrefab;

            case TerrainType.Fast:
                return fastPrefab;

            default:
                return null;
        }
    }


    private void ClearTerrain()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}

