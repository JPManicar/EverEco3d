using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeMenu
{
    public BiomesConfig Biome;
    [Range(0f, 1f)] public float Weighting = 1f;
}


[CreateAssetMenu(fileName = "ProcGen Config", menuName = "EverEco/ProcGen Configuration", order = -1)]
public class ProcGenConfig : ScriptableObject
{
    public List<BiomeMenu> Biomes;

    public int numofBiomes => Biomes.Count;

    [Range(0f,1f)]public float BiomeSeedDensity = 0.1f; //where the drunken walk starts
    
    //Normalize the Weighting
    public float TotalWeighting
    {
        get
        {
            float sum = 0f;

            foreach(var config in Biomes)
            {
                sum += config.Weighting;
            }   
            return sum; 
        }
    }
    
    
}
