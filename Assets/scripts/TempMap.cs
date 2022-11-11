using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMap: MonoBehaviour
{
    public Gradient gradient = new Gradient();
    public HeightMap heightMap;
    public MapDisplay display;

    void Start(){
        int mapHeight = heightMap.height;
        int mapWidth  = heightMap.width;
        float[,] tempMap = GenerateTempMap(heightMap.getHeightMap());
        display.DrawNoiseMap (tempMap);
        //display.DrawGradient(gradient,mapWidth, mapHeight);
    }
    public float[,] GenerateTempMap(float[,] elevation){
        
        int mapWidth  = heightMap.width;
        int mapHeight = heightMap.height;
        float[,] tempGrid =  new float[mapWidth, mapHeight];
        float[,] distanceToEquator = new float[mapWidth, mapHeight];
        // Temperature computation
        //First, get the distance to equator
        for( int x = 0; x < mapWidth; x++ )
        {
            for(int z = 0; z < mapHeight; z++)
            {
                //get the distance value based from a gradient r component
				distanceToEquator[z,x] = gradient.Evaluate(x/(float)mapWidth).r;
            
            }
        }
        //Then compute the value with the elevationMap 
        // G(x + M(x, y), y + N(x, y))
        for( int x = 0; x < mapWidth; x++ )
        {
            for(int z = 0; z < mapHeight; z++)
            {
                //multiply the 2 textures to form the temperature grid
                tempGrid[x,z] =  elevation[x,z] * distanceToEquator[x,z];  //can add some type of offset or scaling maybe

                //just some validation
                if (tempGrid[x,z] > 1 )
                    tempGrid[x,z] = 1;

            }
        }
        //return temperature map of type float array
        return tempGrid;
    }

    /*
    //A more scientific approach but decided to simplify
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
