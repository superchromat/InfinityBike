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

			TrackNode trackNode = GetTrackNode ();
			if (trackNode != null) 
			{
				Vector3 pos = CalculatePositionAboveTheTrack (trackNodeScript.GetComponent<Transform> ().position);
				GetTrackNode ().AddNode (pos);
				cycleNodeIndex = GetTrackNode ().GetNodeCount () - 1;
			}

		}

		if (GUILayout.Button ("Set selected node")) 
		{
			Vector3 pos = CalculatePositionAboveTheTrack(trackNodeScript.GetComponent<Transform> ().position);
			TrackNode trackNode = GetTrackNode ();

			if(trackNode!=null)
			trackNode.SetNode (pos,cycleNodeIndex);
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

		if (GUILayout.Button ("ResetAllNodes")) 
		{
			TrackNode trackNode = GetTrackNode ();

			if (trackNode != null) {

				cycleNodeIndex = 0;

				for (int node = 0; node < trackNode.GetNodeCount (); node++) {
					CycleNode (1);
					SetHandle ();

					Vector3 pos = CalculatePositionAboveTheTrack (trackNodeScript.GetComponent<Transform>().position);
					trackNode.SetNode (pos, cycleNodeIndex);

				}
			}

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
		TrackNode trackNode = GetTrackNode ();

		if(trackNode!=null)
		{
			cycleNodeIndex+=dir;

			if (cycleNodeIndex >=trackNode.GetNodeCount ()) 
			{cycleNodeIndex = 0;}


			if (cycleNodeIndex < 0) 
			{cycleNodeIndex =trackNode.GetNodeCount () - 1;}
		}	
	}


	void DebugDraw()
	{
		TrackNodeTool trackNodeScript = (TrackNodeTool)target;
		TrackNode trackNode = GetTrackNode ();

		if (trackNode != null) {
			Transform trackNodeToolTransform = trackNodeScript.GetComponent<Transform> ();;

			for (int index = 0; index < GetTrackNode ().GetNodeCount (); index++) {
				Handles.color = Color.red;
				Handles.DrawLine (GetTrackNode ().GetNode (index), GetTrackNode ().GetNode (index - 1));
			}

			Handles.color = Color.blue;
			Handles.DrawLine (GetTrackNode ().GetNode (GetTrackNode ().GetNodeCount () - 1),trackNodeToolTransform.position);
			Handles.color = Color.yellow;
			Handles.DrawLine (GetTrackNode ().GetNode (0), trackNodeToolTransform.position);
			Handles.color = Color.green;
			Handles.DrawLine (GetTrackNode ().GetNode (cycleNodeIndex), trackNodeToolTransform.position);
		}
	}	

	Vector3 CalculatePositionAboveTheTrack(Vector3 startPos)
	{	
		RaycastHit hit;
		Physics.Raycast (startPos, Vector3.down, out hit, 100f);

		if (hit.collider != null) 
		{		
			return hit.point + Vector3.up * 1.5f;
		}
		return startPos;		
	}	

	void SetHandle()
	{	
		TrackNodeTool trackNodeScript = (TrackNodeTool)target;

		Transform trackNodeToolTransform = trackNodeScript.GetComponent<Transform> ();
		TrackNode trackNode = GetTrackNode ();

		if (trackNode != null) 
		{trackNodeToolTransform.position = trackNode.GetNode (cycleNodeIndex);}	
	}




	TrackNode GetTrackNode()
	{
		TrackNodeTool trackNodeScript = (TrackNodeTool)target;
		TrackNode trackNode = trackNodeScript.trackNode;
		return trackNode;
	}



}


