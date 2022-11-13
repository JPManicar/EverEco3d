using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ProcGen))]
public class GenerateTerrain : Editor
{

	public override void OnInspectorGUI() {
		ProcGen s = (ProcGen)target;

		if (DrawDefaultInspector()) { 
			if (s.autoUpdate) {
				s.GenerateTerrain();
			}
		}

        if (GUILayout.Button("Generate")) {
            s.seed = Random.Range(int.MinValue, int.MaxValue);
            s.GenerateTerrain();
        }

		
    }
}