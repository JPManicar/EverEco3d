using System.Collections;
using System.Collections.Generic;
using UnityEngine;
   
[System.Serializable]
[CreateAssetMenu(fileName = "Biome Config", menuName = "EverEco/Biome Configuration", order = -1)]
public class BiomesConfig : ScriptableObject
{
 
    public string BiomeName;
    //Temperature Settings
    public float minTemperature;
    public float maxTemperature;

    //Precipitation Settings
    public float minPrecipitation;
    public float maxPrecipitation;

    //Height Settings
    public float minHeight;
    public float maxHeight;

    //Modifiers
    public GameObject HeightModifier;
    public GameObject TexturePainter;
    public Color color;

    public List<TextureConfig> RetrieveTextures()
    {
        if (TexturePainter == null)
            return null;

        // extract all textures from every painter
        List<TextureConfig> allTextures = new List<TextureConfig>();
        BaseTexturePainter[] allPainters = TexturePainter.GetComponents<BaseTexturePainter>();
        foreach(var painter in allPainters)
        {
            var painterTextures = painter.RetrieveTextures();

            if (painterTextures == null || painterTextures.Count == 0)
                continue;

            allTextures.AddRange(painterTextures);
        }

        return allTextures;
    }
}
