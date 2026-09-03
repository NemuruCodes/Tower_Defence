using UnityEngine;
using System.Collections.Generic;

//https://www.redblobgames.com/pathfinding/a-star/introduction.html - A* Algorithm (AStar)
//https://www.geeksforgeeks.org/dsa/a-search-algorithm/ 
//https://medium.com/@nanda.yugandhar/a-vs-dijkstra-a-visual-guide-to-why-a-sense-of-direction-matters-ef9378d71a53 - Research
//https://www.redblobgames.com/pathfinding/a-star/implementation.html - More on  A* Algorithm

//This script is the overall handler of the A* and is the code handles the finding , graph creation , A* and  changes the ground to  path
public class TerrainPathfinder : MonoBehaviour
{
    [Header("References")]
    public TerrainGrid terrainGrid;

    [Header("Step / Jump Height")]
    public int maxStepHeight = int.MaxValue;

    public Dictionary<Vector2Int, List<Vector2Int>> PathsBySpawner { get; private set; }

    public List<List<Vector2Int>> GeneratePathsToTarget()
    {
        var graph = new TerrainSurfaceGraph(terrainGrid, maxStepHeight);
        var allPaths = new List<List<Vector2Int>>();
        PathsBySpawner = new Dictionary<Vector2Int, List<Vector2Int>>();

        Vector2Int? target = FindFirstCellOfType(TerrainType.Target);
        if (target == null)
        {
            Debug.LogWarning("TerrainPathfinder: no Target cell found - skipping path generation.");
            return allPaths;
        }

        List<Vector2Int> spawners = FindAllCellsOfType(TerrainType.Spawner);
        if (spawners.Count == 0)
        {
            Debug.LogWarning("TerrainPathfinder: no Spawner cells found - skipping path generation.");
            return allPaths;
        }

        foreach (Vector2Int spawner in spawners)
        {
            List<Vector2Int> path = TerrainAStar.FindPath(graph, spawner, target.Value);

            if (path == null)
            {
                Debug.LogWarning($"TerrainPathfinder: no path found from spawner {spawner} to target {target.Value}.");
                continue;
            }

            MarkPath(path);
            allPaths.Add(path);
            PathsBySpawner[spawner] = path;
        }

        return allPaths;
    }

    private void MarkPath(List<Vector2Int> path)
    {
        foreach (Vector2Int point in path)
        {
            TerrainCell cell = terrainGrid.GetSurfaceCell(point.x, point.y);
            if (cell == null)
                continue;

            // Leave Target/Spawner cells as-is, only repaint plain Ground
            // so the endpoints keep rendering with their own prefabs.
            if (cell.type == TerrainType.Ground)
            {
                terrainGrid.SetCell(cell.position, TerrainType.Path);
            }
        }
    }

    private Vector2Int? FindFirstCellOfType(TerrainType type)
    {
        TerrainCell[,] surfaceCells = terrainGrid.GetSurfaceCells();

        for (int x = 0; x < terrainGrid.width; x++)
        {
            for (int z = 0; z < terrainGrid.depth; z++)
            {
                TerrainCell cell = surfaceCells[x, z];
                if (cell != null && cell.type == type)
                    return new Vector2Int(x, z);
            }
        }

        return null;
    }

    private List<Vector2Int> FindAllCellsOfType(TerrainType type)
    {
        var results = new List<Vector2Int>();
        TerrainCell[,] surfaceCells = terrainGrid.GetSurfaceCells();

        for (int x = 0; x < terrainGrid.width; x++)
        {
            for (int z = 0; z < terrainGrid.depth; z++)
            {
                TerrainCell cell = surfaceCells[x, z];
                if (cell != null && cell.type == type)
                    results.Add(new Vector2Int(x, z));
            }
        }

        return results;
    }
}
