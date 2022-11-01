using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMap : MonoBehaviour
{
    public int width = 512;
	public int height = 512;
	public float scale;

	public int octaves;
	[Range(0,1)]
	public float persistance = 0.4f;
	public float lacunarity = 2f;

    public int depth = 20;
	public int seed;
	public Vector2 offset;
    public bool randomOffset = true;
    private void Start()
    {
        //on start of the program a random x and y value will be chosen to randomize the terrain if random offset is toggled on
        if(randomOffset)
        {
        offset.x = Random.Range(0f, 9999f);
        offset.y = Random.Range(0f, 9999f);
        }
    }

    private void Update()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
        displayMap();
    }

    public float[,] getHeightMap()
    {
        float[,] heightMap = NoiseGeneration.GenerateNoiseMap(width, height, seed, scale, octaves, persistance, lacunarity, offset);
        return heightMap;
    }

    TerrainData GenerateTerrain (TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        
        float[,] heightMap = getHeightMap();
        terrainData.SetHeights(0, 0, heightMap);
        return terrainData;
    }
    public void displayMap() {
        float[,] heightMap = getHeightMap();
		MapDisplay display = FindObjectOfType<MapDisplay> ();
		display.DrawNoiseMap (heightMap);
	}


    // void OnValidate() {
	// 	if (mapWidth < 1) {
	// 		mapWidth = 1;
	// 	}
	// 	if (mapHeight < 1) {
	// 		mapHeight = 1;
	// 	}
	// 	if (lacunarity < 1) {
	// 		lacunarity = 1;
	// 	}
	// 	if (octaves < 0) {
	// 		octaves = 0;
	// 	}
	// }
    
}
