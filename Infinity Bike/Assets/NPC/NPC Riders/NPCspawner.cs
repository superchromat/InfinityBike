using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCspawner : MonoBehaviour 
{
	public Transform player;
	public GameObject toSpawnPrefab = null;
	private List<GameObject> NPCList = new List<GameObject> ();
	public int maxNpcOnTrack = 20;

	[Range(0,1f)]
	public float spawnOnFrameProbability = 0f;
	public float spawnCoolDoneTimer = 1f;
	private float spawnTimer = 1f;

	public float firstSpawnOffset = 5f;

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
		spawnTimer = -firstSpawnOffset;
		
	}
	
	// Update is called once per frame
	void Update () 
	{

		if (spawnTimer > spawnCoolDoneTimer ) 
		{
			float randomNumber = Random.Range (0, 1f);

			if (randomNumber < spawnOnFrameProbability) 
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
				NPCList [i].transform.position = player.transform.position - player.transform.rotation*Vector3.forward*20f;
				NPCList [i].transform.forward = player.transform.forward;

				NPCList [i].SetActive (true);
				hasDiabledGameObjectBeenFound = true;

				AIDriver aiHolder = NPCList [i].GetComponent<AIDriver> ();
				if (aiHolder != null) 
				{
					float targetVelocity = player.GetComponent<Rigidbody> ().velocity.magnitude;

					if(targetVelocity < aiHolder.velocity && targetVelocity > 0.1f) 
					{	
						aiHolder.velocity = targetVelocity * (1 + Random.Range (-0.5f, 0.5f));
					}	
				}
			}	

		}	
	}


}
