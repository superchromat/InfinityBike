using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TrackNodeTool))]
public class EditorRespawnNodeSet : Editor {
	private bool doDrawTrajectory = false;
	private int cycleNodeIndex = 0;

	public override void OnInspectorGUI()
	{
		TrackNodeTool trackNodeScript = (TrackNodeTool)target;
		DrawDefaultInspector ();

		if (GUILayout.Button ("Draw track curve")) 
		{
			doDrawTrajectory = !doDrawTrajectory;
			DebugDraw ();
		}
		
		if (GUILayout.Button ("Add and Set Node")) 
		{
			Vector3 pos = CalculatePositionAboveTheTrack(trackNodeScript.GetComponent<Transform> ().position);
			trackNodeScript.trackNode.AddNode (pos);
			cycleNodeIndex = trackNodeScript.trackNode.GetNodeCount () - 1;

		}

		if (GUILayout.Button ("Set selected node")) 
		{
			Vector3 pos = CalculatePositionAboveTheTrack(trackNodeScript.GetComponent<Transform> ().position);
			trackNodeScript.trackNode.SetNode (pos,cycleNodeIndex);
		}

		if (GUILayout.Button ("Cycle up node")) 
		{
			CycleNode (1);
			SetHandle ();
		}
		
		if (GUILayout.Button ("Cycle down node")) 
		{
			CycleNode (-1);
			SetHandle ();
		}

		if (GUILayout.Button ("Set handle at selected node")) 
		{
			SetHandle ();
		}

		SceneView.RepaintAll ();

	}	
	
	public void OnSceneGUI()
	{
		if (doDrawTrajectory) {
			DebugDraw ();
		}
	}


	void CycleNode(int dir)
	{	
		TrackNodeTool trackNodeScript = (TrackNodeTool)target;
		cycleNodeIndex+=dir;

		if (cycleNodeIndex >= trackNodeScript.trackNode.GetNodeCount ()) 
		{cycleNodeIndex = 0;}


		if (cycleNodeIndex < 0) 
		{cycleNodeIndex = trackNodeScript.trackNode.GetNodeCount () - 1;}
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
		Handles.color = Color.green;
		Handles.DrawLine (trackNodeScript.trackNode.GetNode(cycleNodeIndex),trackNodeScript.GetComponent<Transform> ().position);
	}	

	Vector3 CalculatePositionAboveTheTrack(Vector3 startPos)
	{	
		RaycastHit hit;
		Physics.Raycast (startPos, Vector3.down, out hit, 100f);

		if (hit.collider != null) 
		{		
			return hit.point + Vector3.up;
		}
		return startPos;		
	}	

	void SetHandle()
	{
		TrackNodeTool trackNodeScript = (TrackNodeTool)target;
		trackNodeScript.GetComponent<Transform> ().position = trackNodeScript.trackNode.GetNode (cycleNodeIndex);
	}



}


