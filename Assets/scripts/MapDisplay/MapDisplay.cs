using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour {

	public Renderer textureRender;

	public void DrawNoiseMap(float[,] noiseMap) {
		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength(1);

		Texture2D texture = new Texture2D (width, height);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = Color.Lerp (Color.black, Color.white, noiseMap [x, y]);
			}
		}
		texture.SetPixels (colourMap);
		texture.Apply ();

		textureRender.material.mainTexture = texture;
		//textureRender.transform.localScale = new Vector3 (width, 1, height);
	}

	public void DrawGradient(Gradient gradient, int mapWidth, int mapHeight){
		
		Texture2D texture = new Texture2D (mapWidth, mapHeight);
		
		Color[] gradientColors = new Color[mapWidth * mapHeight];

		for( int x = 0; x < mapWidth; x++ )
        {
           
            for(int z = 0; z < mapHeight; z++)
            {
				gradientColors[x * mapWidth + z] = gradient.Evaluate(x/(float)mapWidth);
            }
        }
                
		texture.SetPixels(gradientColors);
        texture.Apply();
		textureRender.material.mainTexture = texture;

		//textureRender.transform.Rotate(0, 90, 0);
		//textureRender.transform.localScale = new Vector3 (mapWidth, 1, mapHeight);
	}
	
}