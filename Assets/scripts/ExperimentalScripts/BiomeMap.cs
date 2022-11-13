using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomeMap
{
    public static Color[,] GenerateBiomeMap(float[,] heightMap, float[,] tempMap, float[,] precMap, float seaLevel, List<BiomesConfig> biomes, float spread, float spreadThreshold) {
        int width = tempMap.GetLength(0);
        int height = tempMap.GetLength(1);
        Color[,] biomeMap = new Color[width, height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {

                float elevation = heightMap[i, j];
                float temperature = tempMap[i, j];
                float precipitation = precMap[i, j] / 100f;

                Color c = Color.black;

                if (elevation <= seaLevel) {
                    c = Color.blue;
                } else {
                    if (precipitation + (temperature * spread) < spreadThreshold)  // Allows for finer control over how widespread the tundra and polar ice caps are
                        c = Color.white;
                    else {
                        foreach (BiomesConfig b in biomes) {

                            

                            if (temperature > b.minTemperature && temperature <= b.maxTemperature 
                                && precipitation > b.minPrecipitation && precipitation <= b.maxPrecipitation
                                ) {
                                    
                                c = b.color;                                
                            }
                        }
                    }
                }

                biomeMap[i, j] = c;
            }
        }

        return biomeMap;
    }
}


    // Biome determine_biome ( float elevation , float temperature , float precipitation ) {
    // if ( elevation < sealevel ) {
    // // below sea level from deep to shallow
    // if ( elevation < sealevel - 0.5f ) return Biome :: Abyssal ;
    // if ( elevation < sealevel - 0.25f ) return Biome :: Ocean ;
    // return Biome :: Coast ;

    // } else if ( temperature < 0.0f ) {
    // // below freezing point
    // return Biome :: Ice ;

    // } else if ( temperature < lower_threshold ) {
    // // polar regions from dry to wet
    // if ( precipitation < lower_threshold ) return Biome :: Mountain ;
    // if ( precipitation < upper_threshold ) return Biome :: Tundra ;
    // return Biome :: Taiga ;

    // } else if ( temperature < upper_threshold ) {
    // // temperate regions from dry to wet
    // if ( precipitation < lower_threshold ) return Biome :: Steppe ;
    // if ( precipitation < upper_threshold ) return Biome :: Prairie ;
    // return Biome :: Forest ;

    // } else {
    // // tropical regions from dry to wet
    // if ( precipitation < lower_threshold ) return Biome :: Desert ;
    // if ( precipitation < upper_threshold ) return Biome :: Savanna ;
    // return Biome :: Rainforest ;
    // }
    // }