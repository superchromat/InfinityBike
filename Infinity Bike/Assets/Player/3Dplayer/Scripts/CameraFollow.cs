using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour {

    private Camera mainCamera;
	public Rigidbody objToFollowBack;
	public Vector3 cameraOffset = Vector3.zero;
    public float cameraDistance = 0f;

	
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
        Vector3 tempCamOff = cameraOffset.normalized* cameraDistance;
        Vector3 targetPosition = objToFollowBack.position + objToFollowBack.rotation * tempCamOff;

        float dist = Vector3.Dot(objToFollowBack.transform.forward, targetPosition - transform.position);
        Vector3 tempVect = (targetPosition - transform.position) - dist * objToFollowBack.transform.forward;
        transform.position += dist * objToFollowBack.transform.forward;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothTime * Time.deltaTime);

        //   transform.position += (targetPosition - transform.position) * (Vector3.Distance(targetPosition, transform.position)- cameraDistance);

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothTime * Time.deltaTime);
        Debug.DrawLine(transform.position, targetPosition);

        Quaternion lookDirection = Quaternion.LookRotation(objToFollowBack.transform.forward, objToFollowBack.transform.up); 

        transform.rotation = Quaternion.Lerp(transform.rotation, lookDirection, smoothTime * Time.deltaTime);

        mainCamera.fieldOfView = startFovSlider * (1 + cameraFovAmplitude*(float)Math.Tanh(objToFollowBack.velocity.sqrMagnitude / cameraFovSensitivity));
    }

}
