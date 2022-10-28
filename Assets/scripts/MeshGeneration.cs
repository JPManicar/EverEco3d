using UnityEngine;
using System.Collections;

public class MeshGeneration: MonoBehaviour{

[SerializeField] MeshFilter meshFilter;
[SerializeField] int mapWidth;  //x coord
[SerializeField] int mapLength; //z coord

public float noiseScale;

public int octaves;  //number of Octaves (number of noise)
[Range(0,1)] public float persistance; //Persistance value must be between 0 to 1
public float lacunarity;
public float amplitude;

public int seed;
public Vector2 offset;

public bool autoUpdate;

public void Update(){
	GenerateMesh();
}

public void GenerateMesh() {
	Mesh mesh = new Mesh();
	mesh.vertices = CreateVertices();
	mesh.triangles = CreateTriangles();
	GetComponent<MeshFilter>().mesh = mesh;
	
	mesh.RecalculateNormals();
	meshFilter.sharedMesh = mesh;
}

private Vector3[] CreateVertices(){
	Vector3[] vertices = new Vector3[(mapWidth + 1) * (mapLength + 1)];
	//NoiseGeneration noiseGen = new NoiseGeneration();
	for(int z = 0, i = 0; z <= mapLength; z++){
		for (int x = 0; x <= mapWidth; x++){

			float [,] sampleNoise = NoiseGeneration.GenerateNoiseMap(mapWidth, mapLength, seed, noiseScale, octaves, persistance, lacunarity, offset);
			float sampleY = Mathf.PerlinNoise(x * noiseScale/lacunarity, z * noiseScale/lacunarity) * 2 - 1;
			vertices[i] = new Vector3(x, sampleY, z);
			i++;
		}
	}

	return vertices;
}

private int[] CreateTriangles(){
	int[] triangles = new int[mapWidth * mapLength * 6];

	for(int z = 0, vert = 0, tris = 0; z < mapLength; z++){
		for (int x = 0; x < mapWidth; x++){
			triangles[tris + 0] = vert + 0;
			triangles[tris + 1] = vert + mapWidth + 1;
			triangles[tris + 2] = vert + 1;
			triangles[tris + 3] = vert + 1;
			triangles[tris + 4] = vert + mapLength + 1;
			triangles[tris + 5] = vert + mapWidth + 2;

			vert++;
			tris += 6;
		}
		vert++;
	}

	return triangles;
}




}
