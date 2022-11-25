using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerationManager))]
public class GenerateWorld : Editor
{
    public override void OnInspectorGUI() {
		GenerationManager s = (GenerationManager)target;

		if (DrawDefaultInspector()) { 
			if (s.autoUpdate) {
				s.GenerateWorld();
			}
		}

        if (GUILayout.Button("Generate World")) {
            s.GenerateWorld();
        }

		if (GUILayout.Button("Generate Textures")) {
            s.RegenerateTextures();
        }

		
    }
}
