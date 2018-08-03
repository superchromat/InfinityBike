using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Respawn : MonoBehaviour {

	public TrackNode trackNode = null;
	public float verticalRespawnPoint = 50f;
	private Rigidbody rb = null;
    private int minDistanceNode = 0;

    private Action onRespawn = null;
    public Action OnRespawn
    {   
        get
        { return onRespawn; }
        set
        { onRespawn += value; }
    }   
    void ClearOnRespawnAction()
    {onRespawn = RespawnObject;}

    void Start () 
	{	
		rb = GetComponent<Rigidbody> ();
        onRespawn += RespawnObject;
        onRespawn();
    }   
    
    void LateUpdate () 
	{   
        minDistanceNode = FindNearestNode(trackNode, transform);
        if (  Vector3.Distance( trackNode.GetNode(minDistanceNode),transform.position) > verticalRespawnPoint)
		{onRespawn();}

        if (GetComponent<PlayerMovement>() != null)
        {OnRespawn = GetComponent<PlayerMovement>().Stop;}
        else if (GetComponent<AIDriver>()!= null)
        {OnRespawn = GetComponent<AIDriver>().Stop;}
    }   

    private void RespawnObject()
    {   
        if (trackNode.GetNodeCount() <= 1)
        { return; }
        
        if (minDistanceNode >= trackNode.GetNodeCount())
        { minDistanceNode = 0; }
        
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }


        WheelCollider[] wheel = GetComponentsInChildren<WheelCollider>();
        foreach (WheelCollider item in wheel)
        {
            item.brakeTorque = 1000f;
            item.motorTorque = 0;
            item.steerAngle = 0;
        }

        transform.position = trackNode.GetNode(minDistanceNode);
        transform.forward = trackNode.GetNode(minDistanceNode + 1) - transform.position;
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

