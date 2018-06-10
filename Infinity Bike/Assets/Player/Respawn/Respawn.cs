using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Respawn : MonoBehaviour {

	public TrackNode respawnPoint;
	public float verticalRespawnPoint = -50f;
	private Rigidbody rb;

	void Start () 
	{	
		rb = GetComponent<Rigidbody> ();
		RespawnObject ();

	}	
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (transform.position.y < verticalRespawnPoint) 
		{RespawnObject ();}
	}

	public void RespawnObject()
	{

		if (respawnPoint.GetNodeCount () <= 1) 
		{return;}

		int minDistanceNode = FindNearestNode(respawnPoint,transform);


		if (minDistanceNode >= respawnPoint.GetNodeCount()) 
		{minDistanceNode = 0;}


		if (rb != null) 
		{	
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}	

		WheelCollider[] wheel = GetComponentsInChildren<WheelCollider> ();
		foreach (WheelCollider item in wheel) 
		{item.brakeTorque = 1000f;}

		transform.position = respawnPoint.GetNode(minDistanceNode);
		transform.forward = respawnPoint.GetNode(minDistanceNode + 1) - transform.position;

	}	

	static public int FindNearestNode(TrackNode respawnPoint, Transform objToRespawn)
	{
		float minDistance = float.MaxValue;
		int minDistanceNode = 0;
		int count = 0;

		for (int index = 0 ; index < respawnPoint.GetNodeCount() ; index++)
		{
			if (Vector3.Distance (objToRespawn.transform.position, respawnPoint.GetNode(index)) < minDistance) 
			{
				minDistance = Vector3.Distance (objToRespawn.transform.position, respawnPoint.GetNode(index));
				minDistanceNode = count;
			}
			count++;
		}

		return minDistanceNode;
	}


}	

