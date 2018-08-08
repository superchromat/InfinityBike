 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrackNodeTool : MonoBehaviour {

	public TrackNode trackNode = null;
    public BezierSpline sourceBesierSpline = null;
    
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
        {
            trackNode.AddNode(sourceBesierSpline.GetPoint((float)node /(float)numberOfNodes));
        }   
    }
    
    private void OnValidate()
    {   
        DistanceBetweenPoints = distanceBetweenPoints;
        FindTrackFiles();
    }
    
}   
