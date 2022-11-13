using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class ProcGen : MonoBehaviour
{
    public enum DrawMode {
        OriginalHeightMap,
        FalloffMap,
        HeightMap,
        TemperatureMap,
        PrecipitationMap,
        BiomeMap
    }

    public DrawMode drawMode;

    [SerializeField]
    public Biome[] biomes;

    public float[,] og_heightMap;
    public float[,] falloffMap;
    public float[,] heightMap;
    public float[,] temperatureMap;
    public float[,] precipitationMap;
    public Color[,] biomeMap;

    // Variables regarding the base height map generation (og_heightMap)
    [Header("Base Height Map Generation")]
    public int mapWidth;
    public int mapHeight;

    public float scale;
    [Range(1, 20)]
    public int octaves;
    public float persistance;
    [Range(0, 10.172911f)]
    public float lacunarity;
    public int seed;
    [Range(0f, 1f)]
    public float seaLevel;

    [Header("Falloff map variables")]
    public float a = 1.86f;
    public float b = 2.81f;

    [Header("Booleans")]
    public bool autoUpdate; 
    public bool useTrueEquator;

    // Variables related to temperature calculations
    [Header("Temperature map related variables")]
    public float temperatureBias;
    public float tempHeight;
    [Range(0f, 0.4f)]
    public float tempLoss;
    public float baseTemp;
    public float spread;
    public float spreadThreshold;

    // Variables related to humidity/precipitation calculations
    [Header("Humidity/Precipitation related variables")]
    public float dewPoint;
    public float precipitationIntensity = 1f;
    [Range(0f, 1f)]
    public float humidityFlatteningThreshold;

    [Header("Texture + Object that holds the map")]
    // Texture and object that holds the map
    public Renderer textureRenderer;
    public Texture2D temperatureColorImage;

    private void OnValidate() {
        if (a < 0)
            a = 0;
        if (b < 0)
            b = 0;
        if (tempLoss < 0)
            tempLoss = 0;
        if (precipitationIntensity < 0.001f)
            precipitationIntensity = 0.001f;
    }

    private void Start() {
        seed = Random.Range(int.MinValue, int.MaxValue);
        GenerateTerrain();
    }

    public void GenerateTerrain() {
        // Generate the base height map and the falloff Map
        og_heightMap = NoiseGeneration.GenerateNoiseMap(mapWidth, mapHeight, seed, scale, octaves, persistance, lacunarity, Vector2.zero);
        falloffMap = MapUtilities.GenerateFalloffMap(mapWidth, mapHeight, a, b);

        // Create a new height map by subtracting the original height map from the falloff map
        heightMap = new float[mapWidth, mapHeight];
        for (int i = 0; i < mapWidth; i++) {
            for (int j = 0; j < mapHeight; j++) {
                float value = Mathf.Clamp01(og_heightMap[i, j] - falloffMap[i, j]);
                heightMap[i, j] = (value > seaLevel) ? value : seaLevel;
            }
        }

        // Calculate the topmost and bottommost latittude for the false-equator
        int earliestIndex = 0;
        int latestIndex = mapHeight - 1;
        for (int j = 0; j < mapHeight; j++) {
            for (int i = 0; i < mapWidth; i++) {
                if (heightMap[i, j] == seaLevel)
                    continue;
                else {
                    earliestIndex = j;
                    break;
                }
            }
        }
        for (int j = mapHeight - 1; j > 0; j--) {
            for (int i = 0; i < mapWidth; i++) {
                if (heightMap[i, j] == seaLevel)
                    continue;
                else {
                    latestIndex = j;
                    break;
                }
            }
        }

        // Generate the temperature map, precipitation map, and the biome map
        temperatureMap = MapUtilities.GenerateTemperatureMap(heightMap, temperatureBias, earliestIndex, latestIndex, tempHeight, tempLoss, baseTemp, useTrueEquator);
        precipitationMap = MapUtilities.GeneratePrecipitationMap(og_heightMap, temperatureMap, dewPoint, earliestIndex, latestIndex, precipitationIntensity, useTrueEquator, humidityFlatteningThreshold);
        biomeMap = MapUtilities.GenerateBiomeMap(heightMap, temperatureMap, precipitationMap, seaLevel, biomes, spread, spreadThreshold);


        if (drawMode == DrawMode.OriginalHeightMap)
            DrawTexture(og_heightMap);
        if (drawMode == DrawMode.FalloffMap)
            DrawTexture(falloffMap);
        if (drawMode == DrawMode.HeightMap)
            DrawTexture(heightMap);
        if (drawMode == DrawMode.TemperatureMap)
            DrawTexture(temperatureMap);
        if (drawMode == DrawMode.PrecipitationMap)
            DrawTexture(precipitationMap);
        if (drawMode == DrawMode.BiomeMap)
            DrawBiomeTexture(biomeMap);
            
    }

    public void DrawTexture(float[,] map) {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        Color[] colorMap = new Color[width * height];

        if (drawMode == DrawMode.TemperatureMap) {
            for (int j = 0; j < height; j++) {
                for (int i = 0; i < width; i++) {
                    int x = (int)(Mathf.Clamp01(map[i, j]) * temperatureColorImage.width);
                    int y = temperatureColorImage.height / 2;
                    colorMap[j * width + i] = temperatureColorImage.GetPixel(x, y);
                }
            }
        }
        else {
            for (int j = 0; j < height; j++) {
                for (int i = 0; i < width; i++) {
                    float value = (drawMode == DrawMode.PrecipitationMap) ? map[i, j] / 100f : map[i, j];
                    colorMap[j * width + i] = (map[i, j] > seaLevel) ? Color.Lerp(Color.black, Color.white, value) : Color.black;
                }
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();
        //texture.filterMode = FilterMode.Point;

        textureRenderer.material.mainTexture = texture;
        //textureRenderer.transform.localScale = new Vector3(width, 0, height);  //this line changes the plane's size to the size of the grid  maps
    }

    public void DrawBiomeTexture(Color[,] map) {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        Color[] colorMap = new Color[width * height];

        for (int j = 0; j < height; j++) {
            for (int i = 0; i < width; i++) {
                colorMap[j * width + i] = map[i, j];
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();
        //texture.filterMode = FilterMode.Point;

        textureRenderer.material.mainTexture = texture;
       // textureRenderer.transform.localScale = new Vector3(width, 0, height);
    }

    [System.Serializable]
    public struct Biome {
        public string name;

        public float minTemperature;
        public float maxTemperature;

        public float minPrec;
        public float maxPrec;

        public Color color;
    }
}
