using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class ResourceGenerator : MonoBehaviour
{
    [Header("Spawn settings")]
    public GameObject Tree;
    public GameObject Stone;
    public float spawnChance;


    [Header("Raycast setup")]
    public float distanceBetweenCheck;
    public float heightOfCheck = 10f, rangeOfCheck = 30f;
    public LayerMask layerMask;
    public Vector2 positivePosition, negativePosition;
    
    // private void Start()
    // {
    //     SpawnResources();
    // }

    public void regenenerateObjects()
    {
            float startTime = Time.realtimeSinceStartup;
            DeleteResources();
            SpawnResources();
            Debug.Log("Asset Placement:" + ((Time.realtimeSinceStartup - startTime)*1000f) + "ms");
    }


    void SpawnResources()
    {
        for (float x = negativePosition.x; x < positivePosition.x; x += distanceBetweenCheck)
        {
            for (float z = negativePosition.y; z < positivePosition.y; z += distanceBetweenCheck)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(x, heightOfCheck, z), Vector3.down, out hit, rangeOfCheck, layerMask))
                {
                    if (spawnChance > Random.Range(0f, 101f))
                    {
                        Instantiate(Tree, hit.point, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform);

                    }
                }
                RaycastHit hit1;
                if (Physics.Raycast(new Vector4(x, heightOfCheck, z), Vector3.down, out hit1, rangeOfCheck, layerMask))
                {
                    if (spawnChance > Random.Range(0f, 101f))
                    {
                        Instantiate(Stone, hit1.point, Quaternion.Euler(new Vector4(0, Random.Range(0, 360), 0)), transform);

                    }
                }
            }
        }
    }

    void DeleteResources()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

}
