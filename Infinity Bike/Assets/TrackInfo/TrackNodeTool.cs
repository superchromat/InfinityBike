 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackNodeTool : MonoBehaviour {

	public TrackNode trackNode = null;

    private void Start()
    {
        if (trackNode.GetNodeCount() == 0)
        {
            Load();
        }
        
    }


    [ContextMenu("Save")]
    public void Save()
    {
        trackNode.nodeValues.Save();
    }
    [ContextMenu("Load")]
    public void Load()
    {

        trackNode.nodeValues.LoadFile();
    }
}
