using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecipitationMap : MonoBehaviour
{
   
    public HeightMap heightMap;
    public TempMap temperatureMap;
    public MapDisplay display;
    // Start is called before the first frame update
    void Start()
    {
        float[,] precipitation = GeneratePrecipitationMap(heightMap.getHeightMap(), temperatureMap.GenerateTempMap(heightMap.getHeightMap()));
        display.DrawNoiseMap(precipitation);
    }

    public float[,] GeneratePrecipitationMap(float[,] elevation, float[,] temperature){
        int mapWidth = heightMap.width;
        int mapHeight = heightMap.height;
        Vector2 offset = new Vector2(2099,2029);
        float[,] precipitation = NoiseGeneration.GenerateNoiseMap(mapWidth, mapHeight, 10021, 50, 3, 0.5f, 2, offset);


        

        return precipitation;

    }



}
