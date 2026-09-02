using UnityEngine;

public class TerrainCell 
{
    public Vector3Int position;
    public TerrainType type; 
    public bool IsWalkable() // Overkill right now as  only ground is walkable might add more.
    { 
        return type == TerrainType.Ground; 
    }

    public TerrainCell(Vector3Int position, TerrainType type) 
    { 
        this.position = position;
        this.type = type; 
    }
}
