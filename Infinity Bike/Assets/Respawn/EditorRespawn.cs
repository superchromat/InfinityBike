using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TrackNodeTool))]
public class EditorRespawn : Editor {
	private bool doDrawTrajectory = false;

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();

		TrackNodeTool trackNodeScript = (TrackNodeTool)target;
		if (GUILayout.Button ("Add and Set Node")) 
		{
			trackNodeScript.trackNode.AddNode (trackNodeScript.GetComponent<Transform> ().position);
		}

		if (GUILayout.Button ("Set Node")) 
		{
			trackNodeScript.trackNode.SetNode (trackNodeScript.GetComponent<Transform> ().position);
		}

		if (GUILayout.Button ("Draw track curve")) 
		{
			doDrawTrajectory = !doDrawTrajectory;
			SceneView.RepaintAll ();
		}

	}

	public void OnSceneGUI()
	{
		if(doDrawTrajectory)
			DebugDraw ();
	}

	void DebugDraw()
	{
		TrackNodeTool trackNodeScript = (TrackNodeTool)target;
		for (int index =0 ; index < trackNodeScript.trackNode.GetNodeCount() ;index++)
		{
			Handles.color = Color.red;
			Handles.DrawLine (trackNodeScript.trackNode.GetNode(index), trackNodeScript.trackNode.GetNode(index-1));
		}
		Handles.color = Color.blue;
		Handles.DrawLine (trackNodeScript.trackNode.GetNode(trackNodeScript.trackNode.GetNodeCount()-1),trackNodeScript.GetComponent<Transform> ().position);
		Handles.color = Color.yellow;
		Handles.DrawLine (trackNodeScript.trackNode.GetNode(0),trackNodeScript.GetComponent<Transform> ().position);



	}	
}


