using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(TrackNodeTool))]
public class Editor_TrackNodeTool : Editor
{
    private static bool doDrawTrajectory = false;

    private bool useSpaceBarToSet = false;
    private int cycleNodeIndex = 0;
    private int CycleNodeIndex
    {
        get { return cycleNodeIndex; }
        set
        {   
            cycleNodeIndex = value;
            GetTrackNode().ClampIndex(ref cycleNodeIndex);
        }   
    }

    private bool enableOnScreenPlacement = false;

    private void OnEnable()
    {   
        TrackNodeTool trackNodeToolScript = (TrackNodeTool)target;
        EditorUtility.SetDirty(trackNodeToolScript.trackNodeToolSettings);
    }   


    void OnSceneGUI()
    {
        TrackNodeTool trackNodeToolScript = (TrackNodeTool)target;
        TrackNode trackNode = GetTrackNode();

        EditorUtility.SetDirty(trackNodeToolScript);
        EditorUtility.SetDirty(trackNodeToolScript.trackNode);

        if (enableOnScreenPlacement && useSpaceBarToSet && EventType.KeyDown == Event.current.type && Event.current.keyCode == KeyCode.Space)
        {
            enableOnScreenPlacement = false;
            if (trackNode != null)
            {
                Vector3 pos = trackNodeToolScript.GetComponent<Transform>().position;

                GetTrackNode().AddNode(pos);
                CycleNodeIndex = GetTrackNode().GetNodeCount() - 1;
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
        EditorUtility.SetDirty(trackNodeToolScript.trackNode);
        EditorUtility.SetDirty(trackNodeToolScript.trackNodeToolSettings);

        DrawDefaultInspector();

        trackNodeToolScript.FindTrackFiles();
        doDrawTrajectory = trackNodeToolScript.trackNodeToolSettings.doDrawTrajectory;
        doDrawTrajectory = GUILayout.Toggle(doDrawTrajectory, "Draw track curve");
        useSpaceBarToSet = GUILayout.Toggle(useSpaceBarToSet, "Use Space Bar To Add");


        trackNodeToolScript.trackNode.isLoopOpen = GUILayout.Toggle(trackNodeToolScript.trackNode.isLoopOpen, "Is Loop Open");


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

            CycleNodeIndex = nearestIndex;
        }

        if (GUILayout.Button("Set handle at selected node"))
        {
            SetHandleAtIndex(CycleNodeIndex);
        }


        GUILayout.BeginVertical();
        if (GUILayout.Button("Add Node"))
        {

            TrackNode trackNode = GetTrackNode();
            if (trackNode != null)
            {

                Vector3 pos = trackNodeToolScript.GetComponent<Transform>().position;
                GetTrackNode().AddNode(pos);
                CycleNodeIndex = GetTrackNode().GetNodeCount() - 1;
            }

        }

        if (GUILayout.Button("Set selected node"))
        {
            Vector3 pos = trackNodeToolScript.GetComponent<Transform>().position;
            TrackNode trackNode = GetTrackNode();

            if (trackNode != null)
                trackNode.SetNode(pos, CycleNodeIndex);
        }

        if (GUILayout.Button("Insert Node"))
        {
            float distance = float.MaxValue;
            int nearestNode = 0;

            Transform trackNodeToolTransform = trackNodeToolScript.GetComponent<Transform>();
            TrackNode trackNode = GetTrackNode();

            for (int index = 0; index < trackNode.GetNodeCount(); index++)
            {
                if (distance > Vector3.SqrMagnitude(trackNodeToolTransform.position - trackNode.GetNode(index)))
                {
                    distance = Vector3.SqrMagnitude(trackNodeToolTransform.position - trackNode.GetNode(index));
                    nearestNode = index;
                }
            }

            CycleNodeIndex = nearestNode;

            Vector3 directionForward = trackNode.GetNode(CycleNodeIndex + 1) - trackNode.GetNode(CycleNodeIndex);

            Vector3 directionBackward = trackNode.GetNode(CycleNodeIndex - 1) - trackNode.GetNode(CycleNodeIndex);



            Vector3 pos = trackNodeToolTransform.position;

            if (Vector3.Dot(directionForward, trackNodeToolTransform.position - trackNode.GetNode(CycleNodeIndex)) > Vector3.Dot(directionBackward, trackNodeToolTransform.position - trackNode.GetNode(CycleNodeIndex)))
            {
                CycleNodeIndex = CycleNodeIndex + 1;
            }
            trackNode.InsertNode(pos, CycleNodeIndex);



        }

        if (GUILayout.Button("Remove Node"))
        {
            TrackNode trackNode = GetTrackNode();
            trackNode.DeleteNode(CycleNodeIndex);

            if (CycleNodeIndex >= trackNode.GetNodeCount())
            {
                CycleNodeIndex = trackNode.GetNodeCount() - 1;
            }
            else if (CycleNodeIndex < 0)
            {
                CycleNodeIndex = 0;
            }
        }
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save track node to file"))
        { trackNodeToolScript.trackNode.Save(trackNodeToolScript.GetFileName(), trackNodeToolScript.saveLoad.dataPath); }
        if (GUILayout.Button("Load track node from file"))
        {
            trackNodeToolScript.trackNode.Load(trackNodeToolScript.GetFileName(), trackNodeToolScript.saveLoad.dataPath);
        }
        GUILayout.EndHorizontal();

        if (trackNodeToolScript.sourceBesierSpline == null)
        {
            GUILayout.TextField("Drag a reference to a bezier spline to import track from bezier.");
        }
        else
        {
            if (GUILayout.Button("Import Track from bezier"))
            {
                TrackNode trackNode = GetTrackNode();
                trackNodeToolScript.PopulateTrackNodeWithBesier(trackNodeToolScript.PointsFromBezier);
                for (int i = 0; i < trackNodeToolScript.PointsFromBezier; i++)
                { trackNode.SetNode(trackNode.GetNode(i), i); }

            }
        }

        GUILayout.BeginHorizontal();
        GUILayout.TextField("Selected Node number");
        CycleNodeIndex = EditorGUILayout.IntField(CycleNodeIndex);
        GUILayout.EndHorizontal();

        if (doDrawTrajectory)
        { GUILayout.TextField("Debug Draw color legend\n\tMain Track : RED and CYAN\n\tSelected node : GREEN\n\tFirst Node : YELLOW\n\tLast Node : BLUE"); }

        if (useSpaceBarToSet)
        { GUILayout.TextField("Use Space Bar To Add\n\tDrag the tool around the scene and press spacebar to add a new node.\n\tTip : Place the scene in isometric view with a top view."); }

        trackNodeToolScript.trackNodeToolSettings.doDrawTrajectory = doDrawTrajectory;
        trackNodeToolScript.trackNodeToolSettings.cycleNodeIndex = cycleNodeIndex;

        SceneView.RepaintAll();

    }

    void CycleNode(int dir)
    {
        TrackNodeTool trackNodeToolScript = (TrackNodeTool)target;
        TrackNode trackNode = GetTrackNode();

        if (trackNode != null)
        {
            CycleNodeIndex += dir;
        }
    }

    

    void SetHandleAtIndex(int index)
    {
        TrackNodeTool trackNodeScript = (TrackNodeTool)target;

        TrackNode trackNode = trackNodeScript.trackNode;

        if (trackNode != null)
        {
            trackNodeScript.GetComponent<Transform>().position = trackNode.GetNode(index);
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


