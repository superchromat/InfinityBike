using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Respawn : MonoBehaviour
{   
    public TrackNode trackNode = null;
    public float respawnDistance = 50f;
    private Rigidbody rb = null;
    public int respawnNode = 0;
    bool blockSpawnCheck = false;

    private Action onRespawn = null;

    public void AddLastToRespawnAction(Action toAdd)
    { onRespawn += toAdd; }
    public void AddFirstToRespawnAction(Action toAdd)
    {
        Action total = onRespawn;
        onRespawn = toAdd;
        onRespawn += total;
    }
    
    public void CallRespawnAction()
    {   
        if (onRespawn == null)
            RespawnObject();
        else
            onRespawn();
    }   

    void Start()
    {
        blockSpawnCheck = false;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {   
        if (!blockSpawnCheck)
        {
            blockSpawnCheck = true;
            StartCoroutine(CheckIfRespawnIsNeeded());
        }
    }   

    private IEnumerator CheckIfRespawnIsNeeded()
    {
        int node = TrackNode.FindNearestNode(trackNode, transform);
        if (Vector3.Distance(trackNode.GetNode(node), transform.position) > respawnDistance)
        {
            respawnNode = node;
            CallRespawnAction();
        }

        yield return new WaitForSeconds(0.5f);
        blockSpawnCheck = false;
    }
    
    public void RespawnObject()
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
        transform.forward = trackNode.GetNode(respawnNode + 1) - trackNode.GetNode(respawnNode - 1);
    }

    public void RespawnObject(Vector3 spawnPosition, Vector3 spawnDirection)
    {
        if (trackNode.GetNodeCount() <= 1)
        { return; }

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = spawnPosition;
        transform.forward = spawnDirection;
    }

}

