using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrecipitationMap
{

    public static float[,] GeneratePrecipitationMap(float[,] og_heightMap, float[,] tempMap, float dewPoint, int topIndex, int bottomIndex, float intensity, bool useTrueEquator, float flatteningThreshold) {
        int width = tempMap.GetLength(0);
        int height = tempMap.GetLength(1);
        float[,] precMap = new float[width, height];
        float[,] humidityInversionMap = new float[width, height];
        float startTime = Time.realtimeSinceStartup;
        // If the humidity is inverted (line 93), deserts and other high temperature/low precipitation biomes wont spawn.
        // If it is inverted, then medium temperature/mid to high precipitation biomes wont spawn
        // This generates a map based on the original height map to determine when to invert the humidity or not, allowing for deserts and such to spawn. 
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (og_heightMap[i, j] >= flatteningThreshold)
                    humidityInversionMap[i, j] = 1f;
                else
                    humidityInversionMap[i, j] = 0f;
            }
        }

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                // Humidity formula from www.wikihow.com/Calculate-Humidity
                float celcius = tempMap[i, j] * 100;

                float saturatedVaporPressure = ((458.25f * celcius) / (237.3f + celcius));
                float actualVaporPressure = ((458.25f * dewPoint) / (237.3f + dewPoint));

                //if the temperature is 0, saturated vapor pressure becomes 0, leading to a 
                //division by 0 error when calculating relHumidity so we adjust this to avoid errors.
                float relHumidity = (saturatedVaporPressure == 0) ? (actualVaporPressure * 10) : (actualVaporPressure / saturatedVaporPressure) * 10; 

                if (relHumidity > 50f)
                    relHumidity = 50f;
                
                // Regular humidity (no inversion) would be ~20-30, leading to lots of deserts. Inverting it brings it to around 60 (close to irl humidity levels). 
                // Based on the previously generated humidity inversion map, we determine when we should invert the map. 
                relHumidity = humidityInversionMap[i, j] == 1f ? 100 - (relHumidity * 2) : (relHumidity * 2); 
                float precipitation = CalculatePrecipitation(relHumidity, tempMap[i, j], topIndex, bottomIndex, j, intensity, height, useTrueEquator);

                
                precMap[i, j] = precipitation;
            }
        }

        Debug.Log("Precipitation Runtime:" + ((Time.realtimeSinceStartup - startTime)*1000f) + "ms");
        return precMap;
    }

    //compute the base precipitation to be derived in main function
    public static float EstimateBasePrecipitation(int topIndex, int bottomIndex, int currentIndex, int height, bool useTrueEquator) {
        float equator = useTrueEquator ? height / 2 : (topIndex + bottomIndex) / 2;
        float vertical = (Mathf.Abs(currentIndex - equator) / equator) * 0.5f + 0.5f;
        float value = (-1 * Mathf.Cos(vertical * 3f * (Mathf.PI * 2))) * 0.5f + 0.5f;
        return value;
    }

    public static float CalculatePrecipitation(float humidity, float temp, int topIndex, int bottomIndex, int currentIndex, float intensity, int height, bool useTrueEquator) {
        float estimated = EstimateBasePrecipitation(topIndex, bottomIndex, currentIndex, height, useTrueEquator);
        float simulated = 2.0f * temp * humidity;
        return intensity * (estimated + simulated);

    }


}
