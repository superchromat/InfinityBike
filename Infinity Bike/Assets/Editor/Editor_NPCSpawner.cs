using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NpcSpawner))]
public class Editor_NPCSpawner : Editor
{



    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        // The npcList should be saved between start/stop of the scene before the button is available.

        //
        //NpcSpawner npcSpawnerScript = (NpcSpawner)target;

        //GUILayout.BeginHorizontal();
        //    if (GUILayout.Button("Generate List"))
        //    {   
        //        npcSpawnerScript.GenerateList();
        //    }



        //GUILayout.EndHorizontal();






    }

}
