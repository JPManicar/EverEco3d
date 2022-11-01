using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcGenHandler : MonoBehaviour
{
    [SerializeField]ProcGenConfig PCGConfig;
    [SerializeField] Terrain TargetTerrain;
#if UNITY_EDITOR
   byte[,] BiomeMap;
   float[,] BiomeStrengths; //might remove this depends
#endif
   
#if UNITY_EDITOR
    public void GenerateWorld()
    {
        //Debug.Log("Hello");
        int mapResolution = TargetTerrain.terrainData.heightmapResolution;

        GenerateBiomes(mapResolution);
    }

    void GenerateBiomes(int mapResolution)
    {
        //Debug.Log("Allocating Biome Values");
        //allocate biome values
        //This can be changed later to suit our algo better
        BiomeMap = new byte[mapResolution, mapResolution];
        BiomeStrengths = new float[mapResolution, mapResolution];

        int numofBiomeSeed = Mathf.FloorToInt(mapResolution * mapResolution * PCGConfig.BiomeSeedDensity);
        List<byte> biomesToSpawn = new List<byte>(numofBiomeSeed);

        //populate biomes to spawn based on the weights(allocates space for biomes)
        float totalBiomeWeighting = PCGConfig.TotalWeighting;
        for(int biomeIndex = 0; biomeIndex < PCGConfig.numofBiomes; ++biomeIndex)
        {
            int numofEntries = Mathf.RoundToInt(numofBiomeSeed + PCGConfig.Biomes[biomeIndex].Weighting/totalBiomeWeighting);
            Debug.Log("Number of seedpoints: " + numofEntries + " for Biome typeof: " + PCGConfig.Biomes[biomeIndex].Biome.BiomeName);
            for (int entryIndex = 0; entryIndex < numofEntries; ++entryIndex)
            {
                biomesToSpawn.Add((byte)biomeIndex);
            }
        }
        Debug.Log(biomesToSpawn.Count);
        //actual spawning of biomes
        while (biomesToSpawn.Count > 0)
        {
            //Debug.Log(biomesToSpawn.Count);
            //pick seed point
            int seedPointIndex = Random.Range(0, biomesToSpawn.Count);

            //get biome index
            byte biomeIndex = biomesToSpawn[seedPointIndex];

            //removes the seed points when a biome is spawned in that point
            biomesToSpawn.RemoveAt(seedPointIndex);

            //Debug.Log("SPAWNING BIOMES INDIVIDUALLY");
            SpawnIndividualBiome(biomeIndex, mapResolution);
        }
    }


    Vector2Int[] NeighborOffset = new Vector2Int[]
    {
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(1,1),
        new Vector2Int(-1,-1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
    };


    //spawn a biome in the biome map
    void SpawnIndividualBiome(byte biomeIndex, int mapResolution)
    {

        //cache biome config
        BiomesConfig biomeConfig = PCGConfig.Biomes[biomeIndex].Biome;
        
        //Get Spawn Location
        //will change this to the climate based biome conditions
        Vector2Int spawnLocation = new Vector2Int(Random.Range(0, mapResolution),Random.Range(0, mapResolution));

        float startIntensity = Random.Range(biomeConfig.minIntensity, biomeConfig.maxDecayRate);


        Queue<Vector2Int> workingQueue = new Queue<Vector2Int>();
        workingQueue.Enqueue(spawnLocation);


        //get visited map
        bool[,] visited = new bool[mapResolution, mapResolution];
        float[,] targetIntensity = new float[mapResolution, mapResolution];

        //set initial intensity
        targetIntensity[spawnLocation.x, spawnLocation.y] = startIntensity;

        //Start Ooze
        while(workingQueue.Count > 0)
        {  
            Vector2Int workingLocation = workingQueue.Dequeue();
            //set biome
            BiomeMap[workingLocation.x, workingLocation.y] = biomeIndex;
            visited[workingLocation.x, workingLocation.y] = true;
            BiomeStrengths[workingLocation.x, workingLocation.y] = targetIntensity[workingLocation.x, workingLocation.y];


            //neighbor traversal (traverses the map to look for valid points to spawn biome)
            for(int neighborIndex = 0; neighborIndex < NeighborOffset.Length; ++neighborIndex)
            {
                Vector2Int neighborLocation = workingLocation * NeighborOffset[neighborIndex];

                //skip invalid points
                if(neighborLocation.x < 0 || neighborLocation.y < 0 || neighborLocation.x >= mapResolution || neighborLocation.y >= mapResolution)
                    continue;
                if (visited[neighborLocation.x, neighborLocation.y])
                    continue;
                //mark as visited
                visited[neighborLocation.x,neighborLocation.y] = true;
                //if valid then add to queue
                workingQueue.Enqueue(neighborLocation);
                //decay
                float neighborStrength = targetIntensity[workingLocation.x, workingLocation.y] - Random.Range(biomeConfig.minDecayRate, biomeConfig.maxDecayRate);


                if (neighborStrength < 0)
                {
                    continue;
                }
            }

            Texture2D biomeMap = new Texture2D(mapResolution, mapResolution, TextureFormat.RGB24, false);
            for(int y = 0; y < mapResolution; ++y)
            {
                for (int x = 0; x < mapResolution; ++x)
                {
                    float hue = ((float)BiomeMap[x, y]/ (float)PCGConfig.numofBiomes);

                    biomeMap.SetPixel(x, y, Color.HSVToRGB(hue, 0.75f, 0.75f));

                }        
            }
            biomeMap.Apply();
            System.IO.File.WriteAllBytes("BiomeMap.png", biomeMap.EncodeToPNG());
        }

    
    }


#endif
}
