 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrackNodeTool : MonoBehaviour {

	public TrackNode trackNode = null;
    public BezierSpline sourceBesierSpline = null;
    public int pointsFromBezier = 5;

    public List<string> availableTrackFiles = new List<string>();

    public SaveLoad saveLoad = new SaveLoad();
    private void OnValidate()
    { saveLoad.dataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);}

    private void Start()
    {
        if (trackNode.GetNodeCount() == 0)
        {
            trackNode.LoadFile(saveLoad.fileName + saveLoad.extension);
        }
    }   

    public string GetFileName()
    {
        return saveLoad.fileName + saveLoad.extension;
    }

    public void FindTrackFiles()
    {   
        availableTrackFiles.Clear();
        string[] files = System.IO.Directory.GetFiles(saveLoad.dataPath, "*.track");

        foreach (string item in files)
        {
            string x = item;
            if (x.Contains(saveLoad.dataPath))
            {
                x = x.Replace(saveLoad.dataPath, "").Remove(0, 1);
                x = x.Replace(".track", "");
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





}
