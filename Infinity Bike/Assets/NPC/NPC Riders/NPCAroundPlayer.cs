using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAroundPlayer : MonoBehaviour {
    
    private GameObject player;
    private NPCspawner nPCspawner;
    public float distanceFromPlayer = 50;
    public int spawnNodeDistance = -1;
    
    private List<AIDriver> aiDriverList;
    private PlayerMovement playerMovement; 

    void Start ()
    {   
        nPCspawner = GetComponent<NPCspawner>();
        player = nPCspawner.player;
        playerMovement = player.GetComponent<PlayerMovement>();

        aiDriverList = new List<AIDriver>();
        foreach (GameObject item in nPCspawner.npcList)
        {   
            AIDriver currentDriver = item.GetComponent<AIDriver>();
            aiDriverList.Add(currentDriver);
            item.GetComponent<Respawn>().AddFirstToRespawnAction(()=> { currentDriver.WaypointNodeID = playerMovement.ClosestNode + spawnNodeDistance; });
        }
        
    }   

    bool isCheckStarted = false;
	void Update ()
    {
        if (!isCheckStarted)
        {   
            isCheckStarted = true;
            CheckIfNPCisTooFarFromPlayer();
        }   
    }   
    
    void CheckIfNPCisTooFarFromPlayer()
    {
        float sqrDistance = distanceFromPlayer * distanceFromPlayer;
        foreach (AIDriver driver in aiDriverList)
        {
           
            if (driver.gameObject.activeSelf && !driver.IdleMode)
            {
                if(sqrDistance < (player.transform.position-driver.transform.position).sqrMagnitude)
                {   
                    driver.WaypointNodeID = playerMovement.ClosestNode + spawnNodeDistance;
                    driver.gameObject.GetComponent<Respawn>().CallRespawnAction();
                }   
                
            }   
        }

        isCheckStarted = false;


    }


}   
