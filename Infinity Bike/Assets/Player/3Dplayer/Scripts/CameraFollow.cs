using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour {

	public Transform objToFollowBack;
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
		Vector3 targetPosition = objToFollowBack.position + objToFollowBack.rotation * cameraOffset;

		transform.position = Vector3.SmoothDamp (transform.position, targetPosition, ref velocity, smoothTime);
		transform.LookAt (objToFollowBack, objToFollowBack.up);

		Debug.DrawLine (transform.position, objToFollowBack.position);
	}


}
