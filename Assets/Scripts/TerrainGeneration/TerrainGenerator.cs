using UnityEngine;
//using System;

public class TerrainGenerator : MonoBehaviour
{
    [Header("References")]
    public TerrainGrid terrainGrid;

    [Header("Noise")]
    public float noiseScale = 0.1f;
    public float heightMultiplier = 5f;

    [Header("Seed")]
    public int seed = 0;
    public bool useRandomSeed = true;

    [Header("Water")]
    public int waterHeight = 2;

    [Header("Mountain")]
    public int mountainHeight = 4;

    private float offsetX;
    private float offsetZ;

    private void Start()
    {
        GenerateTerrain(); 
        
        TerrainRenderer renderer = GetComponent<TerrainRenderer>(); 
        
        if (renderer != null) 
        { 
            renderer.RenderTerrain(); 
        }
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

        for (int x = 0; x < terrainGrid.width; x++)
        {
            for (int z = 0; z < terrainGrid.depth; z++)
            {
                //Perlin noise
                //float noise = Mathf.PerlinNoise((x + offsetX) * noiseScale,(z + offsetZ * noiseScale));
                float noise = generateNoise(x, z, noiseScale);

                // Convert noise into a height
                int terrainHeight = Mathf.RoundToInt(noise * heightMultiplier );

                for (int y = 0; y < terrainGrid.height; y++)
                {
                    if (y <= terrainHeight)
                    {
                        
                        if (y <= waterHeight)// Below the water level
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
            }
        }
    }

    private float generateNoise(int x, int z, float detailScale)
    {
        float xNoise = (x + offsetX) * detailScale;
        float zNoise = (z + offsetZ) * detailScale;

        return Mathf.PerlinNoise(xNoise, zNoise);
    }

}


