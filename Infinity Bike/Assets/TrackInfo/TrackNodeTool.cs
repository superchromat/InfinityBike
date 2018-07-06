 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrackNodeTool : MonoBehaviour {

	public TrackNode trackNode = null;
    public string fileName = "track_1";
    private string extension = ".track";
    public List<string> availableTrackFiles = new List<string>();

    private void Start()
    {   
        if (trackNode.GetNodeCount() == 0)
        {
            trackNode.LoadFile(fileName + extension);
        }
    }

    public string GetFileName()
    {
        return fileName + extension;
    }

    public void FindTrackFiles()
    {   
        availableTrackFiles.Clear();
        string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath, "*.track");
        foreach (string item in files)
        {
            string x = item;
            if (x.Contains(Application.persistentDataPath))
            {
                x = x.Replace(Application.persistentDataPath, "").Remove(0, 1);
                x = x.Replace(".track", "");
            }
            availableTrackFiles.Add(x);
        }
    }   


}
