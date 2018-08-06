using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Respawn : MonoBehaviour {

    public TrackNode trackNode = null;
    public float verticalRespawnPoint = 50f;
    private Rigidbody rb = null;
    private int closestNode = 0;
    bool blockSpawnCheck = false;


    private Action onRespawn = null;
    public Action OnRespawn
    {
        get
        {
            if (onRespawn == null)
            { onRespawn = RespawnObject; }
            return onRespawn;
        }
        set
        { onRespawn += value; }
    }

    public void AddToRespawnAction(Action toAdd)
    {
        onRespawn += toAdd;
    }

    public void CallRespawnAction()
    {
        onRespawn();
    }


    void ClearOnRespawnAction()
    { onRespawn = RespawnObject; }

    void Start()
    {   
        blockSpawnCheck = false;
        rb = GetComponent<Rigidbody>();
        OnRespawn = RespawnObject;
        
        if ((GetComponent<Movement>()) != null)
        { OnRespawn = GetComponent<Movement>().Stop; }


        OnRespawn();
    }   

    void LateUpdate()
    {
        if (!blockSpawnCheck)
        {
            blockSpawnCheck = true;
            StartCoroutine(CheckIfRespawnIsNeeded());
        }
    }

    private IEnumerator CheckIfRespawnIsNeeded()
    {
        closestNode = FindNearestNode(trackNode, transform);
        if (Vector3.Distance(trackNode.GetNode(closestNode), transform.position) > verticalRespawnPoint)
        {
            OnRespawn();
        }
        
        yield return new WaitForSeconds(0.5f);
        blockSpawnCheck = false;
    }

    private void RespawnObject()
    {

        if (trackNode.GetNodeCount() <= 1)
        { return; }
        
        if (closestNode >= trackNode.GetNodeCount())
        { closestNode = 0; }
        
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = trackNode.GetNode(closestNode);
        transform.forward = trackNode.GetNode(closestNode + 1) - trackNode.GetNode(closestNode-1);
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

