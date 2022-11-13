using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome Config", menuName = "EverEco/Biome Configuration", order = -1)]
public class BiomesConfig : ScriptableObject
{
    public string BiomeName;

//    //higher intensity = more biome spread
//    [Range(0f, 1f)] public float minIntensity = 0.5f;
//    [Range(0f, 1f)] public float maxIntensity = 1f;
//    //decay controls how far the biome can go
//    [Range(0f, 1f)] public float minDecayRate = 0.1f;
//    [Range(0f, 1f)] public float maxDecayRate = 0.3f;


    //Color or Texture
    public Texture texture;

    //Temperature Settings
    public float minTemperature;
    public float maxTemperature;

    //Precipitation Settings
    public float minPrecipitation;
    public float maxPrecipitation;

    //Height Settings
    public float minHeight;
    public float maxHeight;

    public GameObject HeightModifier;
    public GameObject TexturePainter;
    public Color color;
}
