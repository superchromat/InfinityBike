using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform objToFollowBack;
	public Transform objToFollowFront;
	public Vector3 cameraOffset = Vector3.zero;

	
	private Vector3 velocity = Vector3.zero;
	public float smoothTime = 0.3F;


	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () {

		OnUpdate ();

	}

	void OnUpdate()
	{
		Vector3 targetPosition = objToFollowBack.position;
		transform.position = Vector3.SmoothDamp (transform.position, targetPosition, ref velocity, smoothTime) - objToFollowBack.TransformDirection(cameraOffset);
		transform.LookAt (objToFollowFront);
		Debug.DrawLine (transform.position, objToFollowBack.position);
	}


}
