using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMap
{
    public static float[,] getHeightMap(float[,] heightMap, int width, int height, float seaLevel, float[,] fallOff, bool useFalloffMap)
    {
        if(useFalloffMap)
        {
            //subtracts the falloff from the original heightmap
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    float value = Mathf.Clamp01(heightMap[i, j] - fallOff[i, j]);
                    heightMap[i, j] = (value > seaLevel) ? value : seaLevel;
                }
            }   
        }
        
        //adjust height multipliers for better hilliness and mountain transitions
        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                heightMap[x,z] *= HeightMap.GetHeightMult(x,z);
            }
        }

        
        return heightMap;
    }
    public static float GetHeightMult(float x, float z)
    {   
    float mountainScale = 200f;
    float unscaledHeightMult = Mathf.PerlinNoise(x/mountainScale, z/mountainScale);
    
    float hilliness = 0.55f; //in range [0, 1]; how much hilliness you see in the flat plains regions
    float sharpness = 1.5f;
    float bias = 0.6f; //any value but try in range [0, 1]; lower for more plains or higher for more mountains

    
    float heightMult = hilliness + ((float)Mathf.Tan(sharpness*(unscaledHeightMult - bias)) + 1f)*(1f - hilliness)/2f;
    
    return heightMult;
    }

    public static int getEarliestIndex(float[,] heightMap, int width, int height, float seaLevel)
    {
        // Calculate the topmost and bottommost latittude for the false-equator
        int earliestIndex = 0;

        for (int j = 0; j < height; j++) {
            for (int i = 0; i < width; i++) {
                if (heightMap[i, j] == seaLevel)
                    continue;
                else {
                    earliestIndex = j;
                    break;
                }
            }
        } //end for
        
        return earliestIndex;
    }

    public static int getLatestIndex(float[,] heightMap, int width, int height, float seaLevel)
    {
        int latestIndex = height - 1;
        for (int j = height - 1; j > 0; j--) {
            for (int i = 0; i < width; i++) {
                if (heightMap[i, j] == seaLevel)
                    continue;
                else {
                    latestIndex = j;
                    break;
                }
            }
        } //end for

        return latestIndex;
    }
    
    
    
    
    
}
