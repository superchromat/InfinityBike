using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAroundPlayer : MonoBehaviour {


    private GameObject player;
    private NPCspawner nPCspawner;
    public int spawnNodeDistance = -10;
    public float maxDistanceFromPlayer = 50f;

    bool isCheckStarted = false;

    // Use this for initialization
    void Start ()
    {   
        nPCspawner = GetComponent<NPCspawner>();
        player = nPCspawner.player;
    }   
	
	// Update is called once per frame
	void Update ()
    {
        if (!isCheckStarted)
        {   
            isCheckStarted = true;
            StartCoroutine(CheckIfNPCisTooFarFromPlayer());
        }   
    }   
    
    IEnumerator CheckIfNPCisTooFarFromPlayer()
    {   
        List<GameObject> npcListHolder = new List<GameObject>(nPCspawner.npcList);
        List<Respawn> npcToRespawn = new List<Respawn>();

        foreach (GameObject item in npcListHolder)
        {   
            if (item.activeSelf)
            {
                Vector3 direction = item.transform.position - player.transform.position;

                if (direction.sqrMagnitude > maxDistanceFromPlayer)
                {   
                    npcToRespawn.Add(item.GetComponent<Respawn>());
                }   
            }   
            
            yield return new WaitForSeconds(0.1f);
        }   

        int node = Respawn.FindNearestNode(nPCspawner.trackNodes, player.transform) + spawnNodeDistance;
        foreach (Respawn item in npcToRespawn)
        {
            item.respawnNode = node;
            item.CallRespawnAction();
            
            yield return new WaitForSeconds(0.2f);
        }   
        
        isCheckStarted = false;
    }   
    

}   
