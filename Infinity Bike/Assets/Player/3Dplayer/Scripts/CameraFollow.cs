using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform objToFollow;
	private Vector3 velocity = Vector3.zero;
	public float smoothTime = 0.3F;
	public float cameraOffSet = 1f;


	void Start () 
	{
		
		
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 targetPosition = objToFollow.position - objToFollow.forward*cameraOffSet;
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

		transform.forward = (objToFollow.position - transform.position).normalized;
		//Debug.DrawLine (transform.position, objToFollow.position);
	}


}
