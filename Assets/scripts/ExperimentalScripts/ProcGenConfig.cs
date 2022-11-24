using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

[System.Serializable]
public class BiomeMenu
{
    public BiomesConfig Biome;
    [Range(0f, 1f)] public float Weighting = 1f;
}


[CreateAssetMenu(fileName = "ProcGen Config", menuName = "EverEco/ProcGen Configuration", order = -1)]
public class ProcGenConfig : ScriptableObject
{
    public List<BiomesConfig> Biomes;
    public int numofBiomes => Biomes.Count;

    [Header("Base Height Map Generation")]
    public int width = 512;
	public int height = 512;
	public float scale;

	public int octaves;
	[Range(0,1)]
	public float persistance = 0.5f;
    public float lacunarity = 2f;
    public int depth = 45;
	public int seed;
     public float seaLevel;
	public Vector2 offset;
   

    [Header("Falloff map variables")]
    public float a = 1.86f;
    public float b = 2.81f;

    [Header("Booleans")]
    public bool useTrueEquator;
    public bool useFalloffMap;
    public bool randomOffset = true;

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
    
    private void OnValidate() {
        if (a < 0)
            a = 0;
        if (b < 0)
            b = 0;
        if (tempLoss < 0)
            tempLoss = 0;
        if (precipitationIntensity < 0.001f)
            precipitationIntensity = 0.001f;
        if (width < 1) {
			width = 1;
		}
		if (height < 1) {
			height = 1;
		}
		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}
    }
}
