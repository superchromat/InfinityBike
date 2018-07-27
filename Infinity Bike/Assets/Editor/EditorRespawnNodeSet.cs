using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(TrackNodeTool))]
public class EditorRespawnNodeSet : Editor
{   

	private static bool doDrawTrajectory = false;
    private static int subscribeCount = 0; 
    private bool useSpaceBarToSet = false;
	private int cycleNodeIndex = 0;
    private bool enableOnScreenPlacement = false;


    void OnSceneGUI()
    {   
        TrackNodeTool trackNodeToolScript = (TrackNodeTool)target;
        if (enableOnScreenPlacement && useSpaceBarToSet && EventType.KeyDown == Event.current.type && Event.current.keyCode == KeyCode.Space)
        {
            enableOnScreenPlacement = false;
            TrackNode trackNode = GetTrackNode();
            if (trackNode != null)
            {
                Vector3 pos = CalculatePositionAboveTheTrack(trackNodeToolScript.GetComponent<Transform>().position);

                GetTrackNode().AddNode(pos);
                cycleNodeIndex = GetTrackNode().GetNodeCount() - 1;
            }
        }
        else if (EventType.KeyUp == Event.current.type && Event.current.keyCode == KeyCode.Space)
        {   
            enableOnScreenPlacement = true;
        }   
    }   
    
    public override void OnInspectorGUI()
	{

        TrackNodeTool trackNodeToolScript = (TrackNodeTool)target;
		DrawDefaultInspector ();

        trackNodeToolScript.FindTrackFiles();

        if (GUILayout.Toggle(doDrawTrajectory, "Draw track curve"))
        {
            if (subscribeCount == 0)
            {   
                SceneView.onSceneGUIDelegate += DebugDraw;
                subscribeCount++;
            }   

            doDrawTrajectory = true;
        }
        else
        {
            while (subscribeCount > 0)
            {
                SceneView.onSceneGUIDelegate -= DebugDraw;
                subscribeCount--;
            }

            doDrawTrajectory = false;
        }

        useSpaceBarToSet = GUILayout.Toggle(useSpaceBarToSet, "Use Space Bar To Add");

        trackNodeToolScript.trackNode.isLoopOpen = GUILayout.Toggle(trackNodeToolScript.trackNode.isLoopOpen, "Is Loop Open");



        {
            GUILayout.BeginHorizontal();
                if (GUILayout.Button("Cycle up selected node"))
                {
                    CycleNode(1);
                }

                if (GUILayout.Button("Cycle down selected node"))
                {
                    CycleNode(-1);
                }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Select nearest node"))
            {
                int nearestIndex = FindNearestNode();

                cycleNodeIndex = nearestIndex;
            }
            
            if (GUILayout.Button("Set handle at selected node"))
            {
                SetHandleAtIndex(cycleNodeIndex);
            }
        }

        GUILayout.BeginVertical();
        if (GUILayout.Button ("Add Node") )
		{

			TrackNode trackNode = GetTrackNode ();
			if (trackNode != null) 
			{

				Vector3 pos = CalculatePositionAboveTheTrack (trackNodeToolScript.GetComponent<Transform> ().position);
				GetTrackNode ().AddNode (pos);
				cycleNodeIndex = GetTrackNode ().GetNodeCount () - 1;
			}

		}

		if (GUILayout.Button ("Set selected node")) 
		{
			Vector3 pos = CalculatePositionAboveTheTrack(trackNodeToolScript.GetComponent<Transform> ().position);
			TrackNode trackNode = GetTrackNode ();

			if(trackNode!=null)
			trackNode.SetNode (pos,cycleNodeIndex);
		}
    
        if (GUILayout.Button ("Insert Node")) 
		{
            float distance = float.MaxValue;
            int nearestNode = 0;

            Transform trackNodeToolTransform = trackNodeToolScript.GetComponent<Transform>();
            TrackNode trackNode = GetTrackNode();

            for (int index = 0; index < trackNode.GetNodeCount(); index++)
            {
                if ( distance > Vector3.SqrMagnitude(trackNodeToolTransform.position - trackNode.GetNode(index))      )
                {
                    distance = Vector3.SqrMagnitude(trackNodeToolTransform.position - trackNode.GetNode(index));
                    nearestNode = index;
                }
            }

            cycleNodeIndex = nearestNode;

            Vector3 directionForward = trackNode.GetNode(cycleNodeIndex + 1) - trackNode.GetNode(cycleNodeIndex);

            Vector3 directionBackward = trackNode.GetNode(cycleNodeIndex - 1) - trackNode.GetNode(cycleNodeIndex);



            Vector3 pos = CalculatePositionAboveTheTrack(trackNodeToolTransform.position);

            if (Vector3.Dot(directionForward , trackNodeToolTransform.position- trackNode.GetNode(cycleNodeIndex))> Vector3.Dot(directionBackward, trackNodeToolTransform.position - trackNode.GetNode(cycleNodeIndex)))
            {
                trackNode.InsertNode(pos, cycleNodeIndex+1);
            }
            else
            {
                trackNode.InsertNode(pos, cycleNodeIndex);
            }



        }

        if (GUILayout.Button("Remove Node"))
        {
            TrackNode trackNode = GetTrackNode();
            trackNode.DeleteNode(cycleNodeIndex);

            if (cycleNodeIndex >= trackNode.GetNodeCount())
            {
                cycleNodeIndex = trackNode.GetNodeCount() - 1;
            }
            else if (cycleNodeIndex < 0)
            {
                cycleNodeIndex = 0;
            }
        }
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save track node to file"))
            {trackNodeToolScript.trackNode.Save(trackNodeToolScript.GetFileName());}
            if (GUILayout.Button("Load track node from file"))
            {trackNodeToolScript.trackNode.LoadFile(trackNodeToolScript.GetFileName());}

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Import Track from bezier"))
        {
            if(trackNodeToolScript.sourceBesierSpline != null)
            trackNodeToolScript.PopulateTrackNodeWithBesier(trackNodeToolScript.pointsFromBezier);
        }





            if (doDrawTrajectory)
        {GUILayout.TextField("Debug Draw color legend\n\tMain Track : RED and CYAN\n\tSelected node : GREEN\n\tFirst Node : YELLOW\n\tLast Node : BLUE");}

        if (useSpaceBarToSet)
        {GUILayout.TextField("Use Space Bar To Add\n\tDrag the tool around the scene and press spacebar to add a new node.\n\tTip : Place the scene in isometric view with a top view.");}
        





        SceneView.RepaintAll ();
	}	



	void CycleNode(int dir)
	{	
		TrackNodeTool trackNodeToolScript = (TrackNodeTool)target;
		TrackNode trackNode = GetTrackNode ();

		if(trackNode!=null)
		{   
			cycleNodeIndex += dir;
            trackNode.ClampIndex(ref cycleNodeIndex);

            /*
			if (cycleNodeIndex >=trackNode.GetNodeCount ()) 
			{cycleNodeIndex = 0;}
			if (cycleNodeIndex < 0) 
			{cycleNodeIndex =trackNode.GetNodeCount () - 1;}
            */
		}	
	}
    
	void DebugDraw(SceneView sceneView)
	{

        TrackNodeTool trackNodeToolScript = (TrackNodeTool)target;
        TrackNode trackNode = trackNodeToolScript.trackNode;

        if (trackNode.GetNodeCount() <= 0)
        { return; }

        if (trackNode != null)
        {
            Transform trackNodeToolTransform = null ;
            try
            {trackNodeToolTransform = trackNodeToolScript.GetComponent<Transform>();}
            catch
            {
                doDrawTrajectory = false;

                while (subscribeCount > 0)
                {
                    SceneView.onSceneGUIDelegate -= DebugDraw;
                    subscribeCount--;
                }
                return;
            }




            for (int index = 0; index < trackNode.GetNodeCount(); index++)
            {
                if (trackNode.isLoopOpen && index == 0)
                { continue; }
                
                if ((index & 1) == 1)
                {
                    Handles.color = Color.red;
                }
                else
                {
                    Handles.color = Color.cyan;
                }
                Handles.DrawLine (trackNode.GetNode (index), trackNode.GetNode (index - 1));
			}

			Handles.color = Color.blue;
			Handles.DrawLine (trackNode.GetNode (trackNode.GetNodeCount () - 1),trackNodeToolTransform.position);

			Handles.color = Color.yellow;
			Handles.DrawLine (trackNode.GetNode (0), trackNodeToolTransform.position);
			Handles.color = Color.green;
			Handles.DrawLine (trackNode.GetNode (cycleNodeIndex), trackNodeToolTransform.position);
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

	void SetHandleAtIndex(int index)
	{	
		TrackNodeTool trackNodeScript = (TrackNodeTool)target;

		TrackNode trackNode = trackNodeScript.trackNode;

		if (trackNode != null) 
		{
            trackNodeScript.GetComponent<Transform>().position = trackNode.GetNode (index);
        }	
	}


    TrackNode GetTrackNode()
	{
		TrackNodeTool trackNodeToolScript = (TrackNodeTool)target;
		TrackNode trackNode = trackNodeToolScript.trackNode;
		return trackNode;
	}

    int FindNearestNode()
    {
        TrackNodeTool trackNodeToolScript = (TrackNodeTool)target;
        TrackNode trackNode = trackNodeToolScript.trackNode;
        Vector3 result = Vector3.zero;
        int ind = -1;
        if (trackNode != null)
        {
            Transform trackNodeToolTransform = trackNodeToolScript.GetComponent<Transform>();

            float distance = float.MaxValue;
            for (int index = 0; index < trackNode.GetNodeCount(); index++)
            {
                float possibleDistance = Vector3.Distance(trackNodeToolTransform.position, trackNode.GetNode(index));
                if (possibleDistance < distance)
                {
                    distance = possibleDistance;
                    result = trackNodeToolTransform.position;
                    ind = index;
                }
            }
        }

        return ind;
    }



}


