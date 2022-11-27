using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paintTerrain : MonoBehaviour
{
    [System.Serializable]
    public class SplatHeights
    {
        public int textureIndex;
        public int startHeight;
        public int overlap;
    }

    public SplatHeights[] splatHeights;

    void normalize(float[] v)
    {
        float total = 0;
        for (int i = 0; i < v.Length; i++)
        {
            total += v[i];
        }

        for (int i = 0; i < v.Length; i++)
        {
            v[i] /= total;
        }
    }

    public float map(float value, float sMin, float sMax, float mMin, float mMax)
    {
        return (value - sMin) * (mMax - mMin) / (sMax - sMin) + mMin;
    }

    public void Start()
    {
        //Get the terrain data of the active terrain
        TerrainData terrainData = Terrain.activeTerrain.terrainData;

        //create the splat map data
        float[,,] splatmapData = new float[terrainData.alphamapWidth,
                                            terrainData.alphamapHeight,
                                            terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float terrainHeight = terrainData.GetHeight(y, x);
                float[] splat = new float[splatHeights.Length];

                for (int i = 0; i < splatHeights.Length; i++)
                {
                    float thisNoise = map(Mathf.PerlinNoise(x * 0.05f, y * 0.05f), 0f, 1f, 0.5f, 1f);

                    float thisHeightStart = splatHeights[i].startHeight * thisNoise
                        - splatHeights[i].overlap * thisNoise;
                    float nextHeightStart = 0;

                    if (i != splatHeights.Length - 1)
                    {
                        nextHeightStart = splatHeights[i + 1].startHeight * thisNoise
                            + splatHeights[i + 1].overlap * thisNoise;
                    }

                    if (i == splatHeights.Length - 1 && terrainHeight >= thisHeightStart)
                    {
                        splat[i] = 1;
                    }
                    else if (terrainHeight >= thisHeightStart && terrainHeight <= nextHeightStart)
                    {
                        splat[i] = 1;
                    }
                }

                normalize(splat);

                for (int j = 0; j < splatHeights.Length; j++)
                {
                    splatmapData[x, y, j] = splat[j];
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
    }
}