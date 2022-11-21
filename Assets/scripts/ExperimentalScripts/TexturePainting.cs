using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif // UNITY_EDITOR


public class TexturePainting : MonoBehaviour
{

    Dictionary<TextureConfig, int> BiomeTextureToTerrainLayerIndex = new Dictionary<TextureConfig, int>();

    void Perform_GenerateTextureMapping(ProcGenConfig Config)
    {
        BiomeTextureToTerrainLayerIndex.Clear();

        // build up list of all textures
        List<TextureConfig> allTextures = new List<TextureConfig>();
        foreach(var biomeMetadata in Config.Biomes)
        {
            List<TextureConfig> biomeTextures = biomeMetadata.RetrieveTextures();

            if (biomeTextures == null || biomeTextures.Count == 0)
                continue;

            allTextures.AddRange(biomeTextures);
        }

        // if (Config.PaintingPostProcessingModifier != null)
        // {
        //     // extract all textures from every painter
        //     BaseTexturePainter[] allPainters = Config.PaintingPostProcessingModifier.GetComponents<BaseTexturePainter>();
        //     foreach(var painter in allPainters)
        //     {
        //         var painterTextures = painter.RetrieveTextures();

        //         if (painterTextures == null || painterTextures.Count == 0)
        //             continue;

        //         allTextures.AddRange(painterTextures);
        //     }            
        // }

        //filter out any duplicate entries
       // allTextures = allTextures.Distinct().ToList();

        // iterate over the texture configs
        int layerIndex = 0;
        foreach(var textureConfig in allTextures)
        {
            BiomeTextureToTerrainLayerIndex[textureConfig] = layerIndex;
            ++layerIndex;
        }
    }



#if UNITY_EDITOR
    
    public void RegenerateTextures()
    {
        //TexturePainting.Perform_LayerSetup(terrain);
    }
    public void Perform_LayerSetup(Terrain TargetTerrain)
    {
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
                if (string.IsNullOrEmpty(layerFile))
                    continue;

                AssetDatabase.DeleteAsset(layerFile);
            }

            Undo.FlushUndoRecordObjects();
        }

        string scenePath = System.IO.Path.GetDirectoryName(SceneManager.GetActiveScene().path);

        //Perform_GenerateTextureMapping();

        // generate all of the layers
        int numLayers = BiomeTextureToTerrainLayerIndex.Count;
        List<TerrainLayer> newLayers = new List<TerrainLayer>(numLayers);
        
        // preallocate the layers
        for (int layerIndex = 0; layerIndex < numLayers; ++layerIndex)
        {
            newLayers.Add(new TerrainLayer());
        }

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
}
