using UnityEngine;

public class TerrainCell 
{
    public Vector3Int position;
    public TerrainType type;
    public bool isSurface;

    public bool isOnPath;
    public bool IsWalkable() // Overkill right now as  only ground is walkable might add more. - Not overkill anymore
    {
        return type == TerrainType.Ground || type == TerrainType.Path || type == TerrainType.Target ||
            type == TerrainType.Spawner || type == TerrainType.Slow || type == TerrainType.Fast;
    }

    public TerrainCell(Vector3Int position, TerrainType type) 
    { 
        this.position = position;
        this.type = type; 
    }
}
