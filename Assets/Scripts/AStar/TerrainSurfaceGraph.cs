using UnityEngine;
using System.Collections.Generic;

//https://www.redblobgames.com/pathfinding/a-star/introduction.html - A* Algorithm
//https://www.geeksforgeeks.org/dsa/a-search-algorithm/
//https://medium.com/@nanda.yugandhar/a-vs-dijkstra-a-visual-guide-to-why-a-sense-of-direction-matters-ef9378d71a53 - Research
//https://www.redblobgames.com/pathfinding/a-star/implementation.html - More on  A* Algorithm

//https://opendsa-server.cs.vt.edu/ODSA/Books/CS3/html/GraphImpl.html - Graph
//https://www.geeksforgeeks.org/dsa/graph-and-its-representations/ 

// The 2d Array which was used to store the surface is now turned into a graph where the cells are nodes ect
public class TerrainSurfaceGraph
{
    private readonly TerrainGrid grid;
    private readonly int maxStepHeight;

    private static readonly Vector2Int[] Directions =
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
    };

    public TerrainSurfaceGraph(TerrainGrid grid, int maxStepHeight = int.MaxValue)
    {
        this.grid = grid;
        this.maxStepHeight = maxStepHeight;
    }

    public bool IsPathable(TerrainCell cell)
    {
        if (cell == null)
            return false;

        return cell.type == TerrainType.Ground ||cell.type == TerrainType.Target ||cell.type == TerrainType.Spawner ||cell.type == TerrainType.Path;
    }

    public IEnumerable<Vector2Int> Neighbors(Vector2Int id)
    {
        TerrainCell current = grid.GetSurfaceCell(id.x, id.y);  // Using the 2d array of the surface
        if (current == null)
            yield break;
        //  Because the 3d terrain surface is 2D array the check only needs to be around the cell and when cell are on -
        // - Diffrent heights it  acts like they are not so no need for diagonal check.
        foreach (Vector2Int dir in Directions) 
        {
            Vector2Int next = id + dir;
            TerrainCell neighborCell = grid.GetSurfaceCell(next.x, next.y);

            if (!IsPathable(neighborCell))
                continue;

            int heightDiff = Mathf.Abs(neighborCell.position.y - current.position.y); //To make sure that the  jump height is not huge this checks the heights
            if (heightDiff > maxStepHeight)
                continue;

            yield return next;
        }
    }

    //Cost for the height
    public double Cost(Vector2Int a, Vector2Int b)
    {
        return 1.0;
    }
}
