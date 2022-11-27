using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif // UNITY_EDITOR


public class TexturePainting : MonoBehaviour
{
    Dictionary<TextureConfig, int> BiomeTextureToTerrainLayerIndex = new Dictionary<TextureConfig, int>();
    GenerationManager PCGManager;
#if UNITY_EDITOR
    
    public void Perform_LayerSetup(Terrain TargetTerrain)
    {
        Debug.Log("Deleting Existing Layers");
        // delete any existing layers
        if (TargetTerrain.terrainData.terrainLayers != null || TargetTerrain.terrainData.terrainLayers.Length > 0)
        {
            Undo.RecordObject(TargetTerrain, "Clearing previous layers");

            // build up list of asset paths for each layer
            List<string> layersToDelete = new List<string>();
            foreach(var layer in TargetTerrain.terrainData.terrainLayers)
            {
                if (layer == null)
                    continue;

                layersToDelete.Add(AssetDatabase.GetAssetPath(layer.GetInstanceID()));
            }

            // remove all links to layers
            TargetTerrain.terrainData.terrainLayers = null;

            // delete each layer
            foreach(var layerFile in layersToDelete)
            {
                //validation step
                if (string.IsNullOrEmpty(layerFile))
                    continue;

                AssetDatabase.DeleteAsset(layerFile);
            }
            Debug.Log("Terrain Layers Deleted");
            Undo.FlushUndoRecordObjects();
        }

        string scenePath = System.IO.Path.GetDirectoryName(SceneManager.GetActiveScene().path);
        PCGManager = gameObject.GetComponent<GenerationManager>();

        Debug.Log("Scene Path = " + scenePath);

        Perform_GenerateTextureMapping(PCGManager.getConfig(), scenePath);

        // generate all of the layers
        int numLayers = BiomeTextureToTerrainLayerIndex.Count;
        List<TerrainLayer> newLayers = new List<TerrainLayer>(numLayers);

        // preallocate the layers
        for (int layerIndex = 0; layerIndex < numLayers; ++layerIndex)
        {
            newLayers.Add(new TerrainLayer());
        }
        //debugging  ignore
        if(numLayers == 0)
            Debug.Log("Empty Layers");
        //end debug
        // iterate over the texture map
        foreach(var textureMappingEntry in BiomeTextureToTerrainLayerIndex)
        {
            var textureConfig = textureMappingEntry.Key;
            var textureLayerIndex = textureMappingEntry.Value;
            var textureLayer = newLayers[textureLayerIndex];

            // configure the terrain layer textures
            textureLayer.diffuseTexture = textureConfig.Diffuse;
            textureLayer.normalMapTexture = textureConfig.NormalMap;

            // save as asset
            string layerPath = System.IO.Path.Combine(scenePath, "Layer_" + textureLayerIndex);
            AssetDatabase.CreateAsset(textureLayer, $"{layerPath}.asset");
        }

        Undo.RecordObject(TargetTerrain.terrainData, "Updating terrain layers");
        TargetTerrain.terrainData.terrainLayers = newLayers.ToArray();
    }
#endif

    void Perform_GenerateTextureMapping(ProcGenConfig Config, string scenePath)
    {
        BiomeTextureToTerrainLayerIndex.Clear();
        
        Debug.Log("Building up a list of all textures");
        // build up list of all textures
        List<TextureConfig> allTextures = new List<TextureConfig>();

        foreach(var biomeMetadata in Config.Biomes)
        {
            //biome metadata is the BiomesConfig Scriptable Objects while Config.Biomes refer to the List
            //so we get all the biomes in the biomes list of the config then call the retreive textures method
            List<TextureConfig> biomeTextures = biomeMetadata.RetrieveTextures();

            Debug.Log("Adding " + biomeMetadata.BiomeName);

            if (biomeTextures == null || biomeTextures.Count == 0)
                continue;

            allTextures.AddRange(biomeTextures);

        }
        if (Config.PaintingPostProcessingModifier != null)
        {
            // extract all textures from every painter
            BaseTexturePainter[] allPainters = Config.PaintingPostProcessingModifier.GetComponents<BaseTexturePainter>();
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
    }

    public int GetLayerForTexture(TextureConfig textureConfig)
    {
        return BiomeTextureToTerrainLayerIndex[textureConfig];
    }








}
