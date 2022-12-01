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
    GenerationManager PCGManager;
#if UNITY_EDITOR
    
    public void Perform_LayerSetup(Terrain TargetTerrain, Dictionary<TextureConfig, int> BiomeTextureToTerrainLayerIndex )
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

        PCGManager.Perform_GenerateTextureMapping();

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

     


    








}
