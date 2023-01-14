using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    Dictionary<TextureConfig, int> BiomeTextureToTerrainLayerIndex = new Dictionary<TextureConfig, int>();

    public DrawMode drawMode;
    public float[,] og_heightMap;
    public float[,] fallOffMap;
    public float[,] heightMap;
    public float[,] temperatureMap;
    public float[,] precipitationMap;
    public int[,] biomeMap;
    private Color[] biomeColorMap;
    public bool autoUpdate; 
    public bool RegenerateLayers;

     [Header("Texture + Object that holds the map")]
    // Texture and object that holds the map
    public Renderer textureRenderer;
    public Texture2D temperatureColorImage;
    public ResourceGenerator assetPlacer;
    public GameObject waterPlane;
    public DisplayGrid displayGrid;
    
    void Start()
    {
        GenerateWorld();
    }
    void Update()
    {
        Vector3 point = displayGrid.getPointClicked();
        Vector2 loc = worldPosToGridPos(point);

        //WILL create separate method to display all the info
        int biomeId = getBiomeIdAt((int)loc.x, (int)loc.y);
        string biomeName = BiomeMap.getBiomeName(biomeId, PCGConfig.Biomes);
        float temperature = getTemp((int)loc.x, (int)loc.y);
        float precipitation = getPrec((int)loc.x, (int)loc.y);
        Debug.Log("Biome at " + point + "is " + biomeId + " " + biomeName );
        Debug.Log("Temperature: " + temperature + "\n Precipitation: " + precipitation + "||" + biomeName );
        
     
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
                    colorMap[j * width + i] = (map[i, j] > PCGConfig.seaLevel) ? Color.Lerp(Color.black, Color.white, value) : Color.black;
                }
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();
        //texture.filterMode = FilterMode.Point;

        textureRenderer.sharedMaterial.mainTexture = texture;
        //textureRenderer.transform.localScale = new Vector3(width, 0, height);  //this line changes the plane's size to the size of the grid  maps
    }

    public void DrawBiomeTexture(int[,] map) {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        biomeColorMap = new Color[width * height];

        for (int j = 0; j < height; j++) {
            for (int i = 0; i < width; i++) {
                foreach (var b in PCGConfig.Biomes)
                {
                    if(map[i,j] == 7)
                        biomeColorMap[j * width + i] = Color.blue;
                    else if (b.BiomeId == map[i,j])
                        biomeColorMap[j * width + i] = b.color;
                    else
                        continue;
                }
                
            }
        }

        texture.SetPixels(biomeColorMap);
        texture.Apply();
        //texture.filterMode = FilterMode.Point;

        textureRenderer.material.mainTexture = texture;
       // textureRenderer.transform.localScale = new Vector3(width, 0, height);
    }

    public void GenerateWorld()
    {
        
        float startTime = Time.realtimeSinceStartup;
        //PCGConfig.seed = Random.Range(int.MinValue, int.MaxValue);
        // if(PCGConfig.randomOffset)
        // {
        //     PCGConfig.offset.x = Random.Range(0f, 9999f);
        //     PCGConfig.offset.y = Random.Range(0f, 9999f);
        //  }
   
        //cache map resolutions
        int alphaMapResolution = TargetTerrain.terrainData.alphamapResolution;
        int mapResolution = TargetTerrain.terrainData.heightmapResolution;

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

        Debug.Log("Climate Computations:" + ((Time.realtimeSinceStartup - startTime)*1000f) + "ms");

        //apply terrain heights
        TargetTerrain.terrainData = GenerateTerrain(TargetTerrain.terrainData);

    #if UNITY_EDITOR
        if(RegenerateLayers)
            RegenerateTextures();
    #endif     
        //adjust sea level depending on terrain depth
        SetWaterPlaneHeight();

        //Retrieve Textures for each Biome
        Perform_GenerateTextureMapping();
        
        //Paint the Terrain
        Perform_TerrainPainting();
       
        //Spawn Objects
        assetPlacer.regenenerateObjects();

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

       

    }//END GenerateWorld()

    #if UNITY_EDITOR
    public void RegenerateTextures()
    {
        texturePainter = gameObject.GetComponent<TexturePainting>();
        texturePainter.Perform_LayerSetup(TargetTerrain, BiomeTextureToTerrainLayerIndex);
    }
    #endif 

    TerrainData GenerateTerrain (TerrainData terrainData)
    {
        float startTime = Time.realtimeSinceStartup;
        terrainData.heightmapResolution = PCGConfig.width + 1;
        terrainData.size = new Vector3(PCGConfig.width, PCGConfig.depth, PCGConfig.height);
        
        terrainData.SetHeights(0, 0, heightMap);

        Debug.Log(("Generate Terrain : " +( Time.realtimeSinceStartup - startTime)*1000f) + "ms");
        return terrainData;
        
    }

    public ProcGenConfig getConfig()
    {
        return PCGConfig;
    }
    public Color[] getBiomeColorMap()
    {
        return biomeColorMap;
    }
    public int[,] getBiomeMap()
    {
        return biomeMap;
    }
    public int getBiomeIdAt(int x, int y)
    {
        return biomeMap[x,y];
    }
    public float[,] getTempMap()
    {
        return temperatureMap;
    }
    public float getTemp(int x, int y)
    {
        return temperatureMap[x,y];
    }
    public float[,] getPrecMap()
    {
        return precipitationMap;
    }
    public float getPrec(int x, int y)
    {
        return precipitationMap[x,y];
    }
     public Vector2 worldPosToGridPos(Vector3 pointWorldPos)
    {
        //Get the Index in the array
        Vector3 terrainLocalPos = TargetTerrain.transform.InverseTransformPoint(pointWorldPos);
        Vector2 normalizedPos = new Vector2(terrainLocalPos.x / TargetTerrain.terrainData.size.x, terrainLocalPos.z / TargetTerrain.terrainData.size.z);

        //inverts the grid to match that of the terrain
        int x = Mathf.RoundToInt((1 - normalizedPos.y) * PCGConfig.width);
        int y = Mathf.RoundToInt((1 - normalizedPos.x) * PCGConfig.height);
        Debug.Log("Grid Pos: "+ x + ", " + y);
        //getIndexInBiomeMap(x,y); 
        Vector2 pos = new Vector2(x,y);
        return pos;
    }
    

    void SetWaterPlaneHeight()
    {
            //get values from PCG Config
            float planeY = PCGConfig.depth * PCGConfig.seaLevel;
            Vector3 currentPosition = waterPlane.transform.position;
            Vector3 waterLevel = new Vector3(PCGConfig.width/2 , planeY, PCGConfig.height/2);

            //adjust water plane scale and position
            waterPlane.transform.localScale = new Vector3(PCGConfig.width, PCGConfig.height, PCGConfig. height);
            waterPlane.transform.position = waterLevel;
    }
    
    public void Perform_GenerateTextureMapping()
    {
        float startTime = Time.realtimeSinceStartup;
        BiomeTextureToTerrainLayerIndex.Clear();
        
        Debug.Log("Building up a list of all textures");
        // build up list of all textures
        List<TextureConfig> allTextures = new List<TextureConfig>();

        foreach(var biomeMetadata in PCGConfig.Biomes)
        {
            //biome metadata is the BiomesConfig Scriptable Objects while Config.Biomes refer to the List
            //so we get all the biomes in the biomes list of the config then call the retreive textures method
            List<TextureConfig> biomeTextures = biomeMetadata.RetrieveTextures();

            Debug.Log("Adding " + biomeMetadata.BiomeName);

            if (biomeTextures == null || biomeTextures.Count == 0)
                continue;

            allTextures.AddRange(biomeTextures);

        }
        if (PCGConfig.PaintingPostProcessingModifier != null)
        {
            // extract all textures from every painter
            BaseTexturePainter[] allPainters = PCGConfig.PaintingPostProcessingModifier.GetComponents<BaseTexturePainter>();
            foreach(var painter in allPainters)
            {
                var painterTextures = painter.RetrieveTextures();

                if (painterTextures == null || painterTextures.Count == 0)
                    continue;

                allTextures.AddRange(painterTextures);
            }            
        }

        //filter out any duplicate entries
       allTextures = allTextures.Distinct().ToList();

        //iterate over the texture configs
        int layerIndex = 0;
        foreach(var textureConfig in allTextures)
        {
            BiomeTextureToTerrainLayerIndex[textureConfig] = layerIndex;
            ++layerIndex;
        }
        Debug.Log(("Texture Mapping : " +( Time.realtimeSinceStartup - startTime)*1000f) + "ms");
    }
    
   public int GetLayerForTexture(TextureConfig textureConfig)
    {
        return BiomeTextureToTerrainLayerIndex[textureConfig];
    }
    public void Perform_TerrainPainting()
    {
        float startTime = Time.realtimeSinceStartup;
        int alphaMapResolution = TargetTerrain.terrainData.alphamapResolution;
        int mapResolution = TargetTerrain.terrainData.heightmapResolution;

        float[,] heightMap = TargetTerrain.terrainData.GetHeights(0, 0, mapResolution, mapResolution);
        float[,,] alphaMaps = TargetTerrain.terrainData.GetAlphamaps(0, 0, alphaMapResolution, alphaMapResolution);
        texturePainter = gameObject.GetComponent<TexturePainting>();
        // zero out all layers
        for (int y = 0; y < alphaMapResolution; ++y)
        {
            for (int x = 0; x < alphaMapResolution; ++x)
            {
                for (int layerIndex = 0; layerIndex < TargetTerrain.terrainData.alphamapLayers; ++layerIndex)
                {
                    alphaMaps[x, y, layerIndex] = 0;
                }
            }
        }   
        // generate the slope map
        float[,] SlopeMap = new float[alphaMapResolution, alphaMapResolution];
        for (int y = 0; y < alphaMapResolution; ++y)
        {
            for (int x = 0; x < alphaMapResolution; ++x)
            {
                SlopeMap[x, y] = TargetTerrain.terrainData.GetInterpolatedNormal((float) x / alphaMapResolution, (float) y / alphaMapResolution).y;
            }
        } 

        // run terrain painting for each biome
        for (int biomeIndex = 0; biomeIndex < PCGConfig.Biomes.Count; ++biomeIndex)
        {
            var biome = PCGConfig.Biomes[biomeIndex];
            if (biome.TexturePainter == null)
                continue;

            BaseTexturePainter[] modifiers = biome.TexturePainter.GetComponents<BaseTexturePainter>();

            foreach(var modifier in modifiers)
            {
                modifier.Execute(this, mapResolution, heightMap, TargetTerrain.terrainData.heightmapScale, SlopeMap, 
                alphaMaps, alphaMapResolution, biomeMap, biomeIndex, biome);
            }
        }        

        // run texture post processing
        if (PCGConfig.PaintingPostProcessingModifier != null)
        {
            BaseTexturePainter[] modifiers = PCGConfig.PaintingPostProcessingModifier.GetComponents<BaseTexturePainter>();

            foreach(var modifier in modifiers)
            {
                modifier.Execute(this, mapResolution, heightMap, TargetTerrain.terrainData.heightmapScale, SlopeMap, 
                alphaMaps, alphaMapResolution);
            }    
        }

        TargetTerrain.terrainData.SetAlphamaps(0, 0, alphaMaps);
        Debug.Log(("Texture Painting : " + (Time.realtimeSinceStartup - startTime)*1000f) + "ms");
    }

    // void OnMouseDown()
    // {
    // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    // RaycastHit hit;
    // if (Physics.Raycast(ray, out hit))
    // {
    //     Vector3 pointWorldPos = hit.point;
    //     //selectedPoint = hit.point;
        
    //     //getPointClicked(pointWorldPos);
    //      Debug.Log("Coordinates: " + pointWorldPos);
    //     findCoord(pointWorldPos);
    // }
    // }

}

