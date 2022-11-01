using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMap: MonoBehaviour
{
    public Gradient gradient = new Gradient();
    public HeightMap heightMap;
    public float[,] GenerateTempMap(float[,] elevation){
        
        int mapHeight = heightMap.height;
        int mapWidth  = heightMap.width;
        // Temperature computation
        //First, get the distance to equator
        // for( int x = 0; x < mapWidth; x++ )
        // {
        //     for(int z = 0; z < mapHeight; z++)
        //     {
        //       //for each point in the grid, get value from gradient map
        //         Debug.Log("iterating through something");
        //       //store values in a grid 
        //     }
        // }

        //Then compute the value with the elevationMap 
            float[,] tempGrid = null;
            //***some function to compute temperature****//
        for( int x = 0; x < mapWidth; x++ )
        {
            for(int z = 0; z < mapHeight; z++)
            {
                float perlinValue = Mathf.PerlinNoise (x, z) * 2 - 1;
                tempGrid[x,z] = perlinValue;
            }
        }
        //store computed temperature for each node/vertex to a grid again
            //**some for loop to store values in array***//
        

        //return temperature map
        return tempGrid;
    }

    public float[,] getTempMap(){
        float[,] elevationMap = heightMap.getHeightMap();
        float[,] tempMap = GenerateTempMap(elevationMap);
        return tempMap;
    }
    /*
    float getDistanceToEquator() {
        //float latitude = to_signed_range ( in_TextureCoordinates . y ) ; //not sure what
        //float delta = latitude - u_Equator ;
        //float range = 1.0 - sign ( delta ) * u_Equator ;
        //return abs ( delta ) / range ;
        float distance = 0.5f;

        return distance;
    }
    */
    
}
