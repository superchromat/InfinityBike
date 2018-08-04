using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Respawn))]
public class Editor_RespawnPlayer : Editor {
    

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();

		Respawn respawnScript = (Respawn)target;
		if (GUILayout.Button ("Respawn player")) 
		{
            respawnScript.OnRespawn();
		}
	}



}
