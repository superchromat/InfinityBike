 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class TrackNodeTool : MonoBehaviour {

	public TrackNode trackNode = null;
    public BezierSpline sourceBesierSpline = null;

    public TrackNodeToolSettings trackNodeToolSettings =null;

    private void Update()
    {
        if (trackNodeToolSettings.doDrawTrajectory)
        {
            DebugDraw();
        }
    }

    void DebugDraw()
    {

        if (trackNode.GetNodeCount() <= 0)
        { return; }

        if (trackNode != null)
        {
            bool indexSwitch = false;
            for (int index = 0; index < trackNode.GetNodeCount(); index++)
            {

                if (index == 0)
                { continue; }

                Color col;
                if (indexSwitch)
                {col = Color.red;}
                else
                {col = Color.cyan;}
                
                indexSwitch = !indexSwitch;

                Debug.DrawLine(trackNode.GetNode(index), trackNode.GetNode(index - 1),col);
            }
            if (!trackNode.isLoopOpen)
            {
                Debug.DrawLine(trackNode.GetNode(0), trackNode.GetNode(- 1), Color.white);

            }


            Debug.DrawLine(trackNode.GetNode(trackNode.GetNodeCount() - 1), transform.position, Color.blue);
            Debug.DrawLine(trackNode.GetNode(0), transform.position, Color.yellow);
            Debug.DrawLine(trackNode.GetNode(trackNodeToolSettings.cycleNodeIndex), transform.position, Color.green);

        }   

    }

    [SerializeField]
    private float distanceBetweenPoints = 0.1f;
    public float DistanceBetweenPoints
    {
        get { return distanceBetweenPoints; }
        set
        {
            if (value <= 0)
            {
                distanceBetweenPoints = 0.05f;
            }
            else
            {
                distanceBetweenPoints = value;
            }
            if (sourceBesierSpline != null)
                pointsFromBezier = Mathf.FloorToInt(sourceBesierSpline.GetSplineLenght() / distanceBetweenPoints);
        }
    }
    [SerializeField]
    private int pointsFromBezier = 5;
    public int PointsFromBezier
    {
        get { return pointsFromBezier; }
    }

    public List<string> availableTrackFiles = new List<string>();
    public SaveLoad saveLoad = new SaveLoad();
    
    private void Start()
    {
        if (trackNode.GetNodeCount() == 0)
        {
            trackNode.Load(saveLoad.fileName +"."+ saveLoad.extension,saveLoad.dataPath);
        }
    }   

    public string GetFileName()
    {
        return saveLoad.fileName + "." + saveLoad.extension;
    }

    public void FindTrackFiles()
    {
        saveLoad.dataPath = Application.dataPath;
        availableTrackFiles.Clear();
        string patern = "*." + saveLoad.extension;

        string[] files = System.IO.Directory.GetFiles(saveLoad.dataPath, patern);

        foreach (string item in files)
        {
            string x = item;
            if (x.Contains(saveLoad.dataPath))
            {
                x = x.Replace(saveLoad.dataPath, "").Remove(0, 1);
                x = x.Replace("."+saveLoad.extension, "");
            }
            availableTrackFiles.Add(x);
        }
    }

    public void PopulateTrackNodeWithBesier(int numberOfNodes)
    {
        trackNode.nodeList.Clear();
        for (int node = 0; node < numberOfNodes; node++)
        {trackNode.AddNode(sourceBesierSpline.GetPoint((float)node / (float)numberOfNodes));}
    }
    
    private void OnValidate()
    {   
        DistanceBetweenPoints = distanceBetweenPoints;
        FindTrackFiles();
    }
    
}   
