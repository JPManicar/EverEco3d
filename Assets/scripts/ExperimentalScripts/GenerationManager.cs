using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
     public enum DrawMode {
        OriginalHeightMap,
        FalloffMap,
        HeightMap,
        TemperatureMap,
        PrecipitationMap,
        BiomeMap
    }
    [SerializeField]ProcGenConfig PCGConfig;
    [SerializeField] Terrain TargetTerrain;
    TexturePainting texturePainter;

    public DrawMode drawMode;
    public float[,] og_heightMap;
    public float[,] fallOffMap;
    public float[,] heightMap;
    public float[,] temperatureMap;
    public float[,] precipitationMap;
    public Color[,] biomeMap;
    public bool autoUpdate; 
    public bool RegenerateLayers;

     [Header("Texture + Object that holds the map")]
    // Texture and object that holds the map
    public Renderer textureRenderer;
    public Texture2D temperatureColorImage;



    // private void Start()
    // {
        
    //     PCGConfig.seed = Random.Range(int.MinValue, int.MaxValue);
    //     //on start of the program a random x and y value will be chosen to randomize the terrain if random offset is toggled on
    //     if(PCGConfig.randomOffset)
        // {
        // PCGConfig.offset.x = Random.Range(0f, 9999f);
        // PCGConfig.offset.y = Random.Range(0f, 9999f);
        // }
    //     GenerateWorld();  
    // }

    // private void Update()
    // {
    //     GenerateWorld();
    //     TargetTerrain.terrainData = GenerateTerrain(TargetTerrain.terrainData);
    // }


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
                    colorMap[j * width + i] = (map[i, j] > PCGConfig.seaLevel) ? Color.Lerp(Color.black, Color.white, value) : Color.black;
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

    public void GenerateWorld()
    {
        og_heightMap = NoiseGeneration.GenerateNoiseMap(PCGConfig.width,PCGConfig.height, PCGConfig.seed, PCGConfig.scale,
                                                        PCGConfig.octaves, PCGConfig.persistance, PCGConfig.lacunarity, PCGConfig.offset);
        fallOffMap = falloffMap.GenerateFalloffMap(PCGConfig.width, PCGConfig.height, PCGConfig.a, PCGConfig.b);
        //recalculates height map with application of fall off map and height multiplier
        heightMap = HeightMap.getHeightMap(og_heightMap, PCGConfig.width, PCGConfig.height, PCGConfig.seaLevel,fallOffMap, PCGConfig.useFalloffMap);
       
        int earliestIndex = HeightMap.getEarliestIndex(heightMap,PCGConfig.width, PCGConfig.height, PCGConfig.seaLevel);
        int latestIndex = HeightMap.getLatestIndex(heightMap,PCGConfig.width, PCGConfig.height, PCGConfig.seaLevel);

        //Climate Maps
        temperatureMap = TempMap.GenerateTemperatureMap(heightMap, PCGConfig.temperatureBias, earliestIndex, latestIndex,
                                                PCGConfig.tempHeight, PCGConfig.tempLoss,PCGConfig.baseTemp, PCGConfig.useTrueEquator);
        precipitationMap = PrecipitationMap.GeneratePrecipitationMap(og_heightMap, temperatureMap, PCGConfig.dewPoint, earliestIndex, latestIndex, 
                                                            PCGConfig.precipitationIntensity, PCGConfig.useTrueEquator, PCGConfig.humidityFlatteningThreshold);
        biomeMap = BiomeMap.GenerateBiomeMap(heightMap, temperatureMap, precipitationMap, PCGConfig.seaLevel, PCGConfig.Biomes, PCGConfig.spread, PCGConfig.spreadThreshold);

        //apply terrain heights
        TargetTerrain.terrainData = GenerateTerrain(TargetTerrain.terrainData);

        //Draw Textures
        if (drawMode == DrawMode.OriginalHeightMap)
            DrawTexture(og_heightMap);
        if (drawMode == DrawMode.FalloffMap)
            DrawTexture(fallOffMap);
        if (drawMode == DrawMode.HeightMap)
            DrawTexture(heightMap);
        if (drawMode == DrawMode.TemperatureMap)
            DrawTexture(temperatureMap);
        if (drawMode == DrawMode.PrecipitationMap)
            DrawTexture(precipitationMap);
        if (drawMode == DrawMode.BiomeMap)
            DrawBiomeTexture(biomeMap);
        

        if(RegenerateLayers)
            RegenerateTextures();
        
        //texturePainter.Perform_GenerateTextureMapping(PCGConfig);
            
    }
    public void RegenerateTextures()
    {
        texturePainter = gameObject.GetComponent<TexturePainting>();
        texturePainter.Perform_LayerSetup(TargetTerrain);
    }
    TerrainData GenerateTerrain (TerrainData terrainData)
    {
        terrainData.heightmapResolution = PCGConfig.width + 1;
        terrainData.size = new Vector3(PCGConfig.width, PCGConfig.depth, PCGConfig.height);
        
        terrainData.SetHeights(0, 0, heightMap);
        return terrainData;
    }

    public ProcGenConfig getConfig()
    {
        return PCGConfig;
    }
    
    
}
