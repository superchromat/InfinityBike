using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "TrackNode", menuName = "TrackNode", order = 1)]
public class TrackNode : ScriptableObject 
{	
	//public TrackNodeValues nodeValues = new TrackNodeValues();
    public List<Vector3> objectPosition = new List<Vector3>();

    public void DeleteNode(int index)
    {
        Debug.Log(index);
        objectPosition.RemoveAt(index);
    }

    public void InsertNode(Vector3 toadd, int indexAhead)
    {
        if (indexAhead < 0)
        {
            indexAhead += objectPosition.Count;
        }
        else if (indexAhead > objectPosition.Count)
        {
            indexAhead -= objectPosition.Count;
        }

        objectPosition.Insert(indexAhead, toadd);
    }

    public void AddNode (Vector3 toadd)
	{
        RaycastHit hit;
        if (Physics.Raycast(toadd, Vector3.down, out hit, 100f))
        { objectPosition.Add(hit.point + Vector3.up * 1.5f); }
        else if(Physics.Raycast(toadd, Vector3.up, out hit, 100f))
        { objectPosition.Add(hit.point + Vector3.up * 1.5f); }


    }	
	
	public void SetNode (Vector3 toadd, int index)
	{	
		if(objectPosition.Count != 0)
		{
			objectPosition [index] = toadd;
		}	
	}


	public Vector3 GetNode (int index)
	{	
		if (GetNodeCount () == 0) 
		{return Vector3.zero;}	

		while (index >= objectPosition.Count) 
		{index -= objectPosition.Count;}	

		while (index < 0) 
		{index += objectPosition.Count;}	
		
		return objectPosition [index];
	}

	public int GetNodeCount()
	{
		return objectPosition.Count;
	}




}
