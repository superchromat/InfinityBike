using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "TrackNode", menuName = "TrackNode", order = 1)]
public class TrackNode : ScriptableObject 
{	
	public TrackNodeValues nodeValues = new TrackNodeValues();

	public void AddNode (Vector3 toadd)
	{	
		RaycastHit hit;
		Physics.Raycast (toadd, Vector3.down, out hit, 100f);

		nodeValues.objectPosition.Add (hit.point + Vector3.up);
	}	
	
	public void SetNode (Vector3 toadd, int index)
	{	
		if(nodeValues.objectPosition.Count != 0)
		{
			nodeValues.objectPosition [index] = toadd;
		}	
	}


	public Vector3 GetNode (int index)
	{	
		if (GetNodeCount () == 0) 
		{return Vector3.zero;}	

		while (index >= nodeValues.objectPosition.Count) 
		{index -= nodeValues.objectPosition.Count;}	

		while (index < 0) 
		{index += nodeValues.objectPosition.Count;}	
		
		return nodeValues.objectPosition [index];
	}

	public int GetNodeCount()
	{
		return nodeValues.objectPosition.Count;
	}

	[Serializable]
	public class TrackNodeValues
	{	
		public List<Vector3> objectPosition = new List<Vector3> ();
	}	



}
