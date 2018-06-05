using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCspawner : MonoBehaviour 
{
	public Transform player;
	public GameObject toSpawnPrefab = null;
	private List<GameObject> NPCList = new List<GameObject> ();
	public TrackNode trackNodes;
	public int maxNpcOnTrack = 20;
	public float spawnTimeBehind = 2f;// how far the npc should spawn in seconds.
	public float spawnMinDistance = 10f;

	[Range(0,1f)]
	public float spawnProb = 0f;
	public float spawnCooldown= 1f;
	private float spawnTimer = 1f;

	public float spawnDelay = 5f;

	// Use this for initialization
	void Start () 
	{
		if (toSpawnPrefab == null) 
		{
			enabled = false;
		}

		for (int i = 0; i < maxNpcOnTrack; i++)
		{	
			NPCList.Add(Instantiate (toSpawnPrefab,transform));
			NPCList [i].SetActive (false);
			NPCList [i].name = "NPC_Rider_" + i;
		}	
		spawnTimer = -spawnDelay;
		
	}
	
	// Update is called once per frame
	void Update () 
	{

		if (spawnTimer > spawnCooldown ) 
		{
			float randomNumber = Random.Range (0, 1f);

			if (randomNumber < spawnProb) 
			{
				SpawnNPC ();
			}

			spawnTimer = 0;
		}


		spawnTimer += Time.deltaTime;
	}

	[ContextMenu("Spawn NPC")]
	void SpawnNPC()
	{
		bool hasDiabledGameObjectBeenFound = false;
		for (int i = 0; i < NPCList.Count && !hasDiabledGameObjectBeenFound; i++) 
		{	
			if (NPCList [i].activeSelf == false) 
			{	
				NPCList [i].SetActive (true);

				AIDriver aiHolder = NPCList [i].GetComponent<AIDriver> ();
				float targetVelocity = player.GetComponent<Rigidbody> ().velocity.magnitude;
				if (aiHolder != null)
                {
					if(targetVelocity < aiHolder.velocity && targetVelocity > 0.1f) 
					{	
						aiHolder.velocity = targetVelocity * (1 + Random.Range (0.1f, 0.5f));
					}	
				}
				float spawnDistance = spawnMinDistance;

                if (aiHolder.velocity > targetVelocity)
                {
                    if (spawnMinDistance > (spawnTimeBehind * aiHolder.velocity))
                    {   
                        spawnDistance = spawnTimeBehind * aiHolder.velocity;
                    }   
                }


				int node = Respawn.FindNearestNode (trackNodes,player.transform);

                NPCList [i].transform.position = player.transform.position - (trackNodes.GetNode(node)- trackNodes.GetNode(node-1)).normalized*spawnDistance;
                NPCList [i].transform.forward = player.transform.forward;

				hasDiabledGameObjectBeenFound = true;


			}	

		}	
	}


}
