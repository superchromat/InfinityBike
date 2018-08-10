using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NPCspawner))]
public class Editor_NPCSpawner : Editor
{



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        NPCspawner npcSpawnerScript = (NPCspawner)target;

        GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate List"))
            {   
                npcSpawnerScript.GenerateList();
            }



        GUILayout.EndHorizontal();






    }

}
