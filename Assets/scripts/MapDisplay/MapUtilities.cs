using UnityEngine;

public static class MapUtilities {

	public static float Evaluate(float value, float a, float b) {
		return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow((b - b * value), a));
	}

	//public static float BiasFunction(float x, float bias) {
	//	float k = Mathf.Pow(1 - bias, 3);
	//	return (x * k) / (x * k - x + 1);
	//}

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

    public static float[,] GenerateFalloffMap(int width, int height, float a, float b) {
        float[,] falloffMap = new float[width, height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                float x = i / (float)width * 2 - 1;
                float y = j / (float)height * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                falloffMap[i, j] = Evaluate(value, a, b);
            }
        }

        return falloffMap;
    }

    public static float[,] GenerateTemperatureMap(float[,] heightMap, float temperatureBias, int topIndex, int bottomIndex, float tempHeight, float tempLoss, float baseTemp, bool useTrueEquator) {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float equator = useTrueEquator ? height / 2 : (topIndex + bottomIndex) / 2;

        float[,] tempMap = new float[width, height];
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {

                //Formula from medium.com/@henchman/adventures-in-procedural-terrain-generation-part-1-b64c29e2367a
                float distFromEquator = Mathf.Abs(j - equator);
                float temperature = ((distFromEquator / height) * -1 * temperatureBias) - (((heightMap[i, j]) / tempHeight) * tempLoss) + baseTemp;

                tempMap[i, j] = temperature;    
            }
        }

        return tempMap;
    }

    public static float[,] GeneratePrecipitationMap(float[,] og_heightMap, float[,] tempMap, float dewPoint, int topIndex, int bottomIndex, float intensity, bool useTrueEquator, float flatteningThreshold) {
        int width = tempMap.GetLength(0);
        int height = tempMap.GetLength(1);
        float[,] precMap = new float[width, height];
        float[,] humidityInversionMap = new float[width, height];

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

                //6.11f * 10 * ((7.5f
                float saturatedVaporPressure = ((458.25f * celcius) / (237.3f + celcius));
                float actualVaporPressure = ((458.25f * dewPoint) / (237.3f + dewPoint));

                //if the temperature is 0, saturated vapor pressure becomes 0, leading to a 
                //division by 0 error when calculating relHumidity.
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

        return precMap;
    }

    public static Color[,] GenerateBiomeMap(float[,] heightMap, float[,] tempMap, float[,] precMap, float seaLevel, ProcGen.Biome[] biomes, float spread, float spreadThreshold) {
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
                        foreach (ProcGen.Biome b in biomes) {
                            if (temperature > b.minTemperature && temperature <= b.maxTemperature && precipitation > b.minPrec && precipitation <= b.maxPrec) {
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
