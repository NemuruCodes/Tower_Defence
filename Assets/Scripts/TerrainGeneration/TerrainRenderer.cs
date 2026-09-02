using UnityEngine;

public class TerrainRenderer : MonoBehaviour
{
    [Header("References")]
    public TerrainGrid terrainGrid;

    [Header("Rendering")]
    public GameObject groundPrefab;
    public GameObject waterPrefab;
    public GameObject mountainPrefab;

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

