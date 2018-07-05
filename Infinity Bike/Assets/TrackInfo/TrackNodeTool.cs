 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackNodeTool : MonoBehaviour {

	public TrackNode trackNode = null;
    public string fileName = "track_1";


    private void Start()
    {   
        if (trackNode.GetNodeCount() == 0)
        {Load();}
    }   

    [ContextMenu("Save")]
    public void Save()
    {
        
        trackNode.Save(fileName);

    }
    [ContextMenu("Load")]
    public void Load()
    {
        trackNode.LoadFile(fileName);
    }   
}
