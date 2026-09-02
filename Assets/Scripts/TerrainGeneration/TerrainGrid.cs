using UnityEngine;


public class TerrainGrid : MonoBehaviour
{
    [Header("Grid Size")]
    public int width = 30;
    public int height = 10;
    public int depth = 30;

    private TerrainCell[,,] grid;


    // Creates the empty grid
    public void CreateGrid()
    {
        grid = new TerrainCell[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3Int position = new Vector3Int(x, y, z);

                    grid[x, y, z] = new TerrainCell(position,TerrainType.Air);
                }
            }
        }
    }


    // Get a cell from the grid
    public TerrainCell GetCell(int x, int y, int z)
    {
        if (!InBounds(x, y, z))
            return null;

        return grid[x, y, z];
    }


    // Get a cell using Vector3Int
    public TerrainCell GetCell(Vector3Int position)
    {
        return GetCell(
            position.x,
            position.y,
            position.z
        );
    }


    // Change the type of a cell
    public void SetCell(int x, int y, int z, TerrainType type)
    {
        if (!InBounds(x, y, z))
            return;

        grid[x, y, z].type = type;
    }


    // Check whether a position exists in the grid
    public bool InBounds(int x, int y, int z)
    {
        return x >= 0 && x < width &&
               y >= 0 && y < height &&
               z >= 0 && z < depth;
    }


    public bool InBounds(Vector3Int position)
    {
        return InBounds(
            position.x,
            position.y,
            position.z
        );
    }


    // Check whether a cell can be walked on
    public bool IsWalkable(int x, int y, int z)
    {
        TerrainCell cell = GetCell(x, y, z);

        if (cell == null)
            return false;

        return cell.IsWalkable();
    }


    public bool IsWalkable(Vector3Int position)
    {
        TerrainCell cell = GetCell(position);

        if (cell == null)
            return false;

        return cell.IsWalkable();
    }

    private bool IsAirOrOutOfBounds(int x, int y, int z)
    {
        if (!InBounds(x, y, z))
            return true;
        return grid[x, y, z].type == TerrainType.Air;
    }

    // Checking around the block to see if block is air
    public bool IsExposedToAir(int x, int y, int z) 
    {
        return IsAirOrOutOfBounds(x + 1, y, z) ||
               IsAirOrOutOfBounds(x - 1, y, z) ||
               IsAirOrOutOfBounds(x, y + 1, z) ||
               IsAirOrOutOfBounds(x, y - 1, z) ||
               IsAirOrOutOfBounds(x, y, z + 1) ||
               IsAirOrOutOfBounds(x, y, z - 1);
    }

    public bool IsExposedToAir(Vector3Int position)
    {
        return IsExposedToAir(position.x, position.y, position.z);
    }
}



