﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Respawn : MonoBehaviour {

    public TrackNode trackNode = null;
    public float respawnDistance = 50f;
    private Rigidbody rb = null;
    public int respawnNode = 0;
    bool blockSpawnCheck = false;
    

    private Action onRespawn = null;

    public void AddToRespawnAction(Action toAdd)
    {onRespawn += toAdd;}

    public void CallRespawnAction()
    {onRespawn();}
    
    void ClearOnRespawnAction()
    {onRespawn = RespawnObject;}
    
    void Start()
    {   
        blockSpawnCheck = false;
        rb = GetComponent<Rigidbody>();
        onRespawn = RespawnObject;
        
        if ((GetComponent<Movement>()) != null)
        { AddToRespawnAction(GetComponent<Movement>().Stop); }
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

        int node = FindNearestNode(trackNode, transform);
        if (Vector3.Distance(trackNode.GetNode(node), transform.position) > respawnDistance)
        {   
            respawnNode = node;
            CallRespawnAction();
        }   
        
        yield return new WaitForSeconds(0.5f);
        blockSpawnCheck = false;
    }

    private void RespawnObject()
    {   
        if (trackNode.GetNodeCount() <= 1)
        { return; }
        
        if (respawnNode >= trackNode.GetNodeCount())
        { respawnNode = 0; }
        
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = trackNode.GetNode(respawnNode);
        transform.forward = trackNode.GetNode(respawnNode + 1) - trackNode.GetNode(respawnNode-1);
    }   
    
	static public int FindNearestNode(TrackNode respawnPoint, Transform objToRespawn)
	{
		float minDistance = float.MaxValue;
		int minDistanceNode = 0;

		for (int index = 0 ; index < respawnPoint.GetNodeCount() ; index++)
		{
            Vector3 distance = objToRespawn.transform.position - respawnPoint.GetNode(index);
            float sqrMagn = distance.sqrMagnitude;

            if (sqrMagn < minDistance) 
			{   
				minDistance = sqrMagn;
				minDistanceNode = index;
			}   
		}

        return minDistanceNode;
	}


}	

