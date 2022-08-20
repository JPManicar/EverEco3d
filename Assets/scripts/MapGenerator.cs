using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public bool autoUpdate;

    public int octaves;
    public float persistence;
    public float lacunarity;

    public void GenerateMap(){
        float[,] noiseMap = noiseGen.GenerateNoiseMap (mapWidth , mapHeight, noiseScale, octaves, persistence, lacunarity);
    
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap (noiseMap);
    }
}
