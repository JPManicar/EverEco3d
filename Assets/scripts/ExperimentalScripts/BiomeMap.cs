using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomeMap
{
    public static int[,] GenerateBiomeMap(float[,] heightMap, float[,] tempMap, float[,] precMap, float seaLevel, List<BiomesConfig> biomes, float spread, float spreadThreshold) 
    {
        int width = tempMap.GetLength(0);
        int height = tempMap.GetLength(1);
       // Color[,] biomeMap = new Color[width, height];
        int[,] biomeMap = new int[width, height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {

                float elevation = heightMap[i, j];
                float temperature = tempMap[i, j];
                float precipitation = precMap[i, j] / 100f;

                //Color c = Color.black;
                int c = -1;
                if (elevation <= seaLevel) {
                   c = 7; // c = Color.blue;
                } else {
                    if (precipitation + (temperature * spread) < spreadThreshold)  // Allows for finer control over how widespread the tundra and polar ice caps are
                        {
                            //c = Color.white;
                            c = 6;
                        } 
                        //  else if(temperature < 0.25f){
                        //     //boreal and polar climate zones     
                        //     if (precipitation < 0.25f) 
                        //         c = 6; //return tundra
                        //     if(precipitation < 0.5f) 
                        //         c = 4; //return boreal forest
                        // }else if(temperature < 0.6f) 
                        // {
                        //     //temperate climate zones
                        //     if(precipitation < 0.25f)
                        //         c = 5;  //return shrubland
                        //     if(precipitation < 0.5f)
                        //         c = 3;  // return temperate forest
                        // }else if(temperature > 0.6f)
                        // {
                        //     //tropical areas
                        //     if(precipitation < 0.25f)
                        //         c = 1; //returns desert biome
                        //     if(precipitation < 0.6f)
                        //         c = 0; //return tropical savannah
                        //     if(precipitation > 0.6f)
                        //         c = 2;  //returns tropical rainforest
                        // }
                        
                        // // foreach(BiomesConfig b in biomes)
                        // {
                        //     if(b.BiomeId == c)
                        //     Debug.Log("["+ i + ","+ j + "]" + "Biome: " + b.BiomeId + "-" + b.BiomeName + "\nPrec: " + precipitation); 
                        //     Debug.Log("["+ i + ","+ j + "]" + "\nTemp: " + temperature); 
                        // }
                              
    
                         
                                 
                    else {
                        foreach (BiomesConfig b in biomes) {

                            if (temperature > b.minTemperature && temperature <= b.maxTemperature 
                                && precipitation > b.minPrecipitation && precipitation <= b.maxPrecipitation
                                ) {
                                    
                                //c = b.color;  
                                c = b.BiomeId;
                                // Debug.Log("Biome Id : " + b.BiomeId + " Biome name: " + b.BiomeName + 
                                //         "\nPrecipitation: " + precipitation);     
                                // Debug.Log("\nTemperature: " + temperature);                         
                            }
                        }
                    }

                }

                biomeMap[i, j] = c;
            }
        }

        return biomeMap;
    }

    // public static string[,] determine_biome( float elevation , float temperature , float precipitation ) {
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
}


    