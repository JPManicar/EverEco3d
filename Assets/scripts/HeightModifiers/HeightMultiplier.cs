using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMultiplier : MonoBehaviour
{
    public float mountainScale;
    [Range(0,1)] public float hilliness; //in range [0, 1]; how much hilliness you see in the flat plains regions
    [Range(0,2)]public float sharpness; //greater than 0; how rapidly plains transition to mountains
    [Range(0,1)] public float bias; //any value but try in range [0, 1]; lower for more plains or higher for more mountains
    public float Strength;

    float GetHeightMult(float x, float y)
    {
    float unscaledHeightMult = Mathf.PerlinNoise(x/mountainScale, y/mountainScale);
    float heightMult = hilliness + ((float)Mathf.Tan(sharpness*(unscaledHeightMult - bias)) + 1f)*(1f - hilliness)/2f;

    return heightMult;
    }
}



