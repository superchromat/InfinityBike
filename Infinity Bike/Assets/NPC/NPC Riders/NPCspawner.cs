using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCspawner : MonoBehaviour 
{
	public Transform player;

	public GameObject toSpawnPrefab = null;

    public List<GameObject> NPCList = new List<GameObject> ();
	public TrackNode trackNodes;

	public int maxNpcOnTrack = 20;

	[Range(0,1f)]
	public float spawnProb = 0f;
	public float spawnCooldown= 1f;
    private bool isReadyForNextSpawn = true;    

	// Use this for initialization
	void Start () 
	{
        try
        {
            for (int i = 0; i < maxNpcOnTrack; i++)
            {
                NPCList.Add(GenerateNewNPC("NPC_Rider_" + i));
            }
		}
        catch (UnassignedReferenceException e)
        {
            Debug.LogWarning(e.Message) ;
            enabled = false;
            return;
        }

	}

    public GameObject GenerateNewNPC(string name)
    {
        GameObject obj = Instantiate(toSpawnPrefab, transform);
        obj.SetActive(false);
        obj.name = name;
        return obj;
    }


	// Update is called once per frame
	void FixedUpdate () 
	{
        if(isReadyForNextSpawn)
        StartCoroutine(SpanwNextNPC());
	}

    IEnumerator SpanwNextNPC()
    {   
        isReadyForNextSpawn = false;
        float randomNumber = Random.Range(0, 1f);
        if (randomNumber < spawnProb)
        {SpawnNPC();}

        yield return new WaitForSeconds(spawnCooldown);
        isReadyForNextSpawn = true;
    }   

	[ContextMenu("Spawn NPC")]
	void SpawnNPC()
	{



        for (int i = 0 ; i < NPCList.Count ; i++) 
		{	
			if (NPCList [i].activeSelf == false) 
			{	
				NPCList [i].SetActive (true);

                int node = Random.Range(0, trackNodes.GetNodeCount() - 1);


                NPCList[i].transform.position = trackNodes.GetNode(node);
                NPCList [i].transform.forward = trackNodes.GetNode(node+1) - trackNodes.GetNode(node);
                
                return;
			}	
		}	
	}   


}   

