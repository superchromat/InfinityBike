using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCspawner : MonoBehaviour 
{
    
	public Transform player;
	public GameObject toSpawnPrefab = null;
	public TrackNode trackNodes;

    public List<GameObject> npcList = new List<GameObject>();
    public int maxNpcOnTrack = 20;

	[Range(0,1f)]
	public float spawnProb = 0f;
	public float spawnCooldown= 1f;
    private bool isReadyForNextSpawn = true;    

	// Use this for initialization
	void Start () 
	{   
        GenerateList();
    }   

    [ContextMenu("GenerateList")]
    void GenerateList()
    {

        while (npcList.Count > maxNpcOnTrack)
        {
            if(Application.isEditor)
            DestroyImmediate(npcList[npcList.Count - 1]);
            else
            Destroy(npcList[npcList.Count - 1]);

            npcList.RemoveAt(npcList.Count - 1);
        }

        List<GameObject> holder = new List<GameObject>();
        for (int i = 0; i < npcList.Count; i++)
        {
            if (npcList[i] == null)
            {
                holder.Add(npcList[i]);
            }
        }
        foreach (GameObject item in holder)
        {npcList.Remove(item);}

        {
            int i = npcList.Count;
            for (; i < maxNpcOnTrack; i++)
            { npcList.Add(GenerateNewNPC("NPC_" + npcList.Count)); }
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
        { SpawnNPC(); }


        yield return new WaitForSeconds(spawnCooldown);
        isReadyForNextSpawn = true;
    }   

	[ContextMenu("Spawn NPC")]
	void SpawnNPC()
	{
        int node = 0;
        node = Respawn.FindNearestNode(trackNodes, player)-10;

        for (int i = 0 ; i < npcList.Count; i++) 
		{	
			if (npcList[i].activeSelf == false) 
			{
                npcList[i].SetActive (true);                
                npcList[i].transform.position = trackNodes.GetNode(node);
                npcList[i].transform.forward = trackNodes.GetNode(node+1) - trackNodes.GetNode(node);
                return; 
			}	        
		}	
	}   

    private void OnValidate()
    {
        if (maxNpcOnTrack < 0)
        {
            maxNpcOnTrack = 0;
        }

    }

}

