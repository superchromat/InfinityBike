using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TrackNodeTool))]
public class EditorRespawnNodeSet : Editor {
	private static bool doDrawTrajectory = false;
    private bool useSpaceBarToSet = false;

	private int cycleNodeIndex = 0;
    private bool enableOnScreenPlacement = false;
    

    void OnSceneGUI()
    {
        //if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space && useSpaceBarToSet)
        //    Debug.Log("Hit");

        TrackNodeTool trackNodeScript = (TrackNodeTool)target;
        if (enableOnScreenPlacement && useSpaceBarToSet && EventType.KeyDown == Event.current.type && Event.current.keyCode == KeyCode.Space)
        {
            enableOnScreenPlacement = false;
            TrackNode trackNode = GetTrackNode();
            if (trackNode != null)
            {
                Vector3 pos = CalculatePositionAboveTheTrack(trackNodeScript.GetComponent<Transform>().position);

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
        TrackNodeTool trackNodeScript = (TrackNodeTool)target;
		DrawDefaultInspector ();

        if ( GUILayout.Toggle(doDrawTrajectory, "Draw track curve"))
        {
            if(doDrawTrajectory==false)
            SceneView.onSceneGUIDelegate += DebugDraw;
            doDrawTrajectory = true;
        }
        else
        {
            if (doDrawTrajectory == true)
            SceneView.onSceneGUIDelegate -= DebugDraw;
            doDrawTrajectory = false;
        }

        useSpaceBarToSet = GUILayout.Toggle(useSpaceBarToSet, "Use Space Bar To Add");


        //GUILayout.Toggle(useSpaceBarToSet, "Use Space Bar To Add");
        //enableOnScreenPlacement = useSpaceBarToSet;

        if (GUILayout.Button ("Add and Set Node") )
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

		if (GUILayout.Button ("Insert Node")) 
		{
            float distance = float.MaxValue;
            int nearestNode = 0;

            Transform trackNodeToolTransform = trackNodeScript.GetComponent<Transform>();
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




            if (Vector3.Dot(directionForward , trackNodeToolTransform.position- trackNode.GetNode(cycleNodeIndex))> Vector3.Dot(directionBackward, trackNodeToolTransform.position - trackNode.GetNode(cycleNodeIndex)))
            {
                trackNode.InsertNode(trackNodeToolTransform.position, cycleNodeIndex+1);
            }
            else
            {
                trackNode.InsertNode(trackNodeToolTransform.position, cycleNodeIndex);
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

        if (GUILayout.Button("Cycle & Set"))
        {
            TrackNode trackNode = GetTrackNode();
            for (int index = 0; index < GetTrackNode().GetNodeCount(); index++)
            {
                CycleNode(1);
                SetHandle();

                Vector3 pos = CalculatePositionAboveTheTrack(trackNodeScript.GetComponent<Transform>().position);

                if (trackNode != null)
                    trackNode.SetNode(pos, cycleNodeIndex);

                if (trackNode.GetNode(cycleNodeIndex) == trackNode.GetNode(cycleNodeIndex - 1))
                {trackNode.DeleteNode(cycleNodeIndex);}
            }   
        }

        SceneView.RepaintAll ();
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


	void DebugDraw(SceneView sceneView)
	{
        if (GetTrackNode().GetNodeCount() <= 0)
        {return;}
        TrackNodeTool trackNodeScript = (TrackNodeTool)target;
		TrackNode trackNode = GetTrackNode ();

		if (trackNode != null)
        {
			Transform trackNodeToolTransform = trackNodeScript.GetComponent<Transform> ();
            
            for (int index = 0; index < GetTrackNode().GetNodeCount(); index++)
            {
                if ((index & 1) == 1)
                {
                    Handles.color = Color.red;
                }
                else
                {
                    Handles.color = Color.cyan;
                }
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


