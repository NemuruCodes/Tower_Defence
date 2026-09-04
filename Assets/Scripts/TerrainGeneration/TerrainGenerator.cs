using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using System;

//https://youtube.com/playlist?list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&si=hIrlMtp2-izvw6W3 - Procedural Terrain Generation playlist
//https://www.youtube.com/watch?v=EvqdcyTgZNg -  Events
//https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html-  Perlin  Noise

public class TerrainGenerator : MonoBehaviour
{
    [Header("References")]
    public TerrainGrid terrainGrid;
    public TerrainPathfinder terrainPathfinder;

    [Header("Noise")]
    public float noiseScale = 0.1f;
    public float heightMultiplier = 5f;

    [Header("Tower Plateau")]
    public bool generateTowerPlateau = true;
    public int towerPlateauRadius = 5;
    public int towerFlatHeight = 3; // constant height of the flat disc

    private Vector2Int towerCenter;

    [Header("Water")]
    public int waterHeight = 2;

    [Header("Mountain")]
    public int mountainHeight = 4;

    [Header("Spawners")]
    public int numSpawners = 4;
    public int edgeMargin = 2;


    private float offsetX;
    private float offsetZ;

    public event Action<Vector3> OnTowerSpawnPointReady;

    public Vector3 GetTowerSpawnPosition()
    {
        return new Vector3(towerCenter.x, towerFlatHeight + 1, towerCenter.y);
    }

    private void Start()
    {
        GenerateTerrain();
        PlaceObjectives();
        terrainPathfinder.GeneratePathsToTarget();

        TerrainRenderer renderer = GetComponent<TerrainRenderer>(); 
        
        if (renderer != null) 
        { 
            renderer.RenderTerrain(); 
        }

        EnemySpawner spawner = GetComponent<EnemySpawner>();
        if (spawner != null)
            spawner.BeginSpawning();

        OnTowerSpawnPointReady?.Invoke(GetTowerSpawnPosition());
    }

    public void GenerateTerrain()
    {
        //Random random = new Random();

       // seed  =  random.Next(1,7)

       // if (useRandomSeed)
        //{
            //seed = Random.Range(int.MinValue, int.MaxValue);
       // }

       
       System.Random randomNum = new System.Random(); //Not sure difrence betwween System and unity radom
        offsetX = randomNum.Next(1, 1000);
        offsetZ = randomNum.Next(1, 1000);

        //offsetX = (offsetX / 8);
        //offsetZ = (offsetX / 8);


        terrainGrid.CreateGrid();

        towerCenter = new Vector2Int(terrainGrid.width / 2, terrainGrid.depth / 2);

        for (int x = 0; x < terrainGrid.width; x++)
        {
            for (int z = 0; z < terrainGrid.depth; z++)
            {
                //Perlin noise
                //float noise = Mathf.PerlinNoise((x + offsetX) * noiseScale,(z + offsetZ * noiseScale));
                //float noise = generateNoise(x, z, noiseScale);

                // Convert noise into a height
                //int terrainHeight = Mathf.RoundToInt(noise * heightMultiplier );

                int terrainHeight;

                if (generateTowerPlateau && IsInsidePlateau(x, z)) // Reusing the hill stamp idea from ice task
                {
                    terrainHeight = towerFlatHeight;
                }
                else
                {
                    float noise = generateNoise(x, z, noiseScale);
                    terrainHeight = Mathf.RoundToInt(noise * heightMultiplier);
                }


                for (int y = 0; y < terrainGrid.height; y++)
                {
                    if (y <= terrainHeight)
                    {
                        if (generateTowerPlateau && IsInsidePlateau(x, z))
                        {
                            terrainGrid.SetCell(x, y, z, TerrainType.Ground);
                        }

                        else if (y <= waterHeight)// Below the water level
                        {
                            terrainGrid.SetCell(x,y,z,TerrainType.Water);
                        }
                        else if (y >= mountainHeight)// Above the Mountain level
                        {
                            terrainGrid.SetCell(x, y, z, TerrainType.Mountain);
                        }
                        else
                        {
                            terrainGrid.SetCell(x, y, z, TerrainType.Ground);
                        }
                    }
                    else
                    {
                        terrainGrid.SetCell(x,y,z,TerrainType.Air);
                    }
                }

                if (terrainHeight >= 0 && terrainHeight < terrainGrid.height)
                {
                    terrainGrid.MarkSurface(x, terrainHeight, z);
                }
            }
        }
    }

    private float generateNoise(int x, int z, float detailScale)
    {
        float xNoise = (x + offsetX) * detailScale;
        float zNoise = (z + offsetZ) * detailScale;

        return Mathf.PerlinNoise(xNoise, zNoise);
    }

    private bool IsInsidePlateau(int x, int z)
    {
        float distSq = (towerCenter.x - x) * (towerCenter.x - x) +
                        (towerCenter.y - z) * (towerCenter.y - z);
        return distSq <= towerPlateauRadius * towerPlateauRadius;
    }


    public void PlaceObjectives()
    {
        PlaceTarget();
        PlaceEdgeSpawners();
    }


    //The plateau center is the center for the tower
    private void PlaceTarget()
    {
        TerrainCell centerCell = terrainGrid.GetSurfaceCell(towerCenter.x, towerCenter.y);
        if (centerCell == null)
            return;

        terrainGrid.SetCell(centerCell.position, TerrainType.Target);
    }


    private void PlaceEdgeSpawners()
    {
        System.Random rng = new System.Random();

        TryPlaceSpawnerOnLine(rng, edgeMargin, null);                          // west
        TryPlaceSpawnerOnLine(rng, terrainGrid.width - 1 - edgeMargin, null);  // east
        TryPlaceSpawnerOnLine(rng, null, edgeMargin);                          // south
        TryPlaceSpawnerOnLine(rng, null, terrainGrid.depth - 1 - edgeMargin);  // north
    }

    private void TryPlaceSpawnerOnLine(System.Random rng, int? fixedX, int? fixedZ)
    {
        List<TerrainCell> candidates = new List<TerrainCell>();

        if (fixedX.HasValue)
        {
            for (int z = 0; z < terrainGrid.depth; z++)
            {
                TerrainCell cell = terrainGrid.GetSurfaceCell(fixedX.Value, z);
                if (IsValidSpawnerCell(cell))
                    candidates.Add(cell);
            }
        }
        else if (fixedZ.HasValue)
        {
            for (int x = 0; x < terrainGrid.width; x++)
            {
                TerrainCell cell = terrainGrid.GetSurfaceCell(x, fixedZ.Value);
                if (IsValidSpawnerCell(cell))
                    candidates.Add(cell);
            }
        }

        if (candidates.Count == 0)
            return; 

        TerrainCell chosen = candidates[rng.Next(candidates.Count)];
        terrainGrid.SetCell(chosen.position, TerrainType.Spawner);
    }

    private bool IsValidSpawnerCell(TerrainCell cell)
    {
        if (cell == null)
            return false;
        return cell.type != TerrainType.Water && cell.type != TerrainType.Mountain;
    }

}


