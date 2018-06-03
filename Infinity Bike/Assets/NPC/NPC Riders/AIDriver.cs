using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDriver : MonoBehaviour {


	public TrackNode trackNode = null;
	public float velocity = 10f;
	public float velocityChangeFrequency =1f;
	public float velocityChangeMultiplier=1f;
	private float initiatingTime = 0f;

	public float lifeTimer = 0f;
	public float lifeTimeLength = 20f;

	int nearestNode = 0;
	private MeshRenderer rend = null;

	// Use this for initialization
	void Start () 
	{	
		nearestNode = Respawn.FindNearestNode (trackNode, transform);
		transform.position = trackNode.GetNode(nearestNode);

		initiatingTime = Time.time;

		rend = GetComponent<MeshRenderer> ();
	}	
	
	// Update is called once per frame
	void Update () 
	{	
		
		transform.position = CalculateNextTarget ();
		nearestNode = Respawn.FindNearestNode (trackNode, transform);


		if (!rend.isVisible)
			lifeTimer += Time.deltaTime;
		else
			lifeTimer = 0;

		if (lifeTimer > lifeTimeLength) 
		{	
			lifeTimer = 0;
			gameObject.SetActive (false);
		}	


	}


	Vector3 CalculateNextTarget()
	{	
		return Vector3.MoveTowards (transform.position, trackNode.GetNode (nearestNode + 1), UpdateVelocity() * Time.deltaTime);
	}	

	float UpdateVelocity()
	{	
		return velocity*(1f + velocityChangeMultiplier *(Mathf.Sin (velocityChangeFrequency * (Time.time - initiatingTime)) ));
	}	


}
