using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "TrackNode", menuName = "TrackNode", order = 1)]
public class TrackNode : ScriptableObject 
{	
	public TrackNodeValues nodeValues = new TrackNodeValues();

    public void DeleteNode(int index)
    {
        Debug.Log(index);
        nodeValues.nodeList.RemoveAt(index);
    }

    public void InsertNode(Vector3 toadd, int indexAhead)
    {
        if (indexAhead < 0)
        {
            indexAhead += nodeValues.nodeList.Count;
        }
        else if (indexAhead > nodeValues.nodeList.Count)
        {
            indexAhead -= nodeValues.nodeList.Count;
        }

        nodeValues.nodeList.Insert(indexAhead, toadd);
    }

    public void AddNode (Vector3 toadd)
	{
        RaycastHit hit;
        if (Physics.Raycast(toadd, Vector3.down, out hit, 100f))
        { nodeValues.nodeList.Add(hit.point + Vector3.up * 1.5f); }
        else if(Physics.Raycast(toadd, Vector3.up, out hit, 100f))
        { nodeValues.nodeList.Add(hit.point + Vector3.up * 1.5f); }


    }	
	
	public void SetNode (Vector3 toadd, int index)
	{	
		if(nodeValues.nodeList.Count != 0)
		{
            nodeValues.nodeList[index] = toadd;
		}	
	}


	public Vector3 GetNode (int index)
	{

		if (GetNodeCount () == 0) 
		{
            return Vector3.zero;
        }	

		while (index >= nodeValues.nodeList.Count) 
		{index -= nodeValues.nodeList.Count;}	

		while (index < 0) 
		{index += nodeValues.nodeList.Count;}

        return nodeValues.nodeList[index];
	}

	public int GetNodeCount()
	{
		return nodeValues.nodeList.Count;
	}
    
}

[Serializable]
public class TrackNodeValues
{   
    public List<Vector3> nodeList = new List<Vector3>();
}   