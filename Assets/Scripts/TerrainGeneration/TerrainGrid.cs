using UnityEngine;

//https://learn.microsoft.com/en-us/dotnet/api/system.array?view=net-10.0 - Arrays
//https://youtube.com/playlist?list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&si=hIrlMtp2-izvw6W3 - Procedural Terrain Generation


public class TerrainGrid : MonoBehaviour
{
    [Header("Grid Size")]
    public int width = 30;
    public int height = 10;
    public int depth = 30;

    private TerrainCell[,,] grid;
    private int[,] surfaceHeight;

    private TerrainCell[,] surfaceCells;


    // Creates the empty grid
    public void CreateGrid()
    {
        grid = new TerrainCell[width, height, depth];

        surfaceHeight = new int[width, depth];

        surfaceCells = new TerrainCell[width, depth];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                surfaceHeight[x, z] = -1; // default: nothing solid yet
            }
        }

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


    public void MarkSurface(int x, int y, int z)
    {
        if (!InBounds(x, y, z))
            return;

        TerrainCell cell = grid[x, y, z];
        cell.isSurface = true;

        surfaceHeight[x, z] = y;
        surfaceCells[x, z] = cell; // reference, not a copy - stays in sync with grid
    }


    // Get a cell from the grid
    public TerrainCell GetCell(int x, int y, int z)
    {
        if (!InBounds(x, y, z))
            return null;

        return grid[x, y, z];
    }


    public int GetSurfaceHeight(int x, int z)
    {
        if (x < 0 || x >= width || z < 0 || z >= depth)
            return -1;
        return surfaceHeight[x, z];
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


    // This Changes the type of a cell
    public void SetCell(int x, int y, int z, TerrainType type)
    {
        if (!InBounds(x, y, z))
            return;

        grid[x, y, z].type = type;
    }

    public void SetCell(Vector3Int position, TerrainType type)
    {
        SetCell(position.x, position.y, position.z, type);
    }



    public TerrainCell GetSurfaceCell(int x, int z)
    {
        if (x < 0 || x >= width || z < 0 || z >= depth)
            return null;
        return surfaceCells[x, z];
    }

    public TerrainCell[,] GetSurfaceCells()
    {
        return surfaceCells;
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



