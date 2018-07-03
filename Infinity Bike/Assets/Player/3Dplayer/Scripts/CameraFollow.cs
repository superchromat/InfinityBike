using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour {

    private Camera mainCamera;
	public Rigidbody objToFollowBack;
	public Vector3 cameraOffset = Vector3.zero;

	
	private Vector3 velocity = Vector3.zero;
	public float smoothTime = 0.05F;

    public float startFovSlider = 60;
    public float cameraFovSensitivity = 1f;
    public float cameraFovAmplitude = 0.5f;

    void Start () 
	{
        mainCamera = GetComponent<Camera>();
        startFovSlider = mainCamera.fieldOfView;
    }   

    // Update is called once per frame
    void Update () {

		OnUpdate ();
    }

    void OnUpdate()
	{


        Vector3 targetPosition = objToFollowBack.position + objToFollowBack.rotation * cameraOffset;

		transform.position = Vector3.SmoothDamp (transform.position, targetPosition, ref velocity, smoothTime);
		transform.LookAt (objToFollowBack.transform, objToFollowBack.transform.up);


        mainCamera.fieldOfView = startFovSlider * (1 + cameraFovAmplitude*(float)Math.Tanh(objToFollowBack.velocity.sqrMagnitude / cameraFovSensitivity));
        

    }


}
