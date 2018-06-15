using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class PlayerMovement : MonoBehaviour {
    public bool isScriptActivated = true;
	public float speedMultiplier = 1f;
	public float angleChangeRange = 180f;

	public float velocityDrag = 1f;

	public WheelCollider backWheel;
	public WheelCollider frontWheel;

	public ArduinoThread serialValues;
	public Transform handleBar;

	//public WheelFrictionCurveSetter wheelFrictionSetter = new WheelFrictionCurveSetter();

	private float processedAngle = 0;
	private float processerdSpeed = 0;

	public float breakForce = 1000;
	private Rigidbody playerRigidBody;

    
	// Use this for initialization
	void Start () 
	{   
		backWheel.ConfigureVehicleSubsteps(1, 12, 15);
		frontWheel.ConfigureVehicleSubsteps(1, 12, 15);
		playerRigidBody = GetComponent<Rigidbody> ();


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyHandleBarRotation();
        if (isScriptActivated)
        { 
          ApplyWheelForces();
        }

		if (handleBar != null) {
			handleBar.localRotation = Quaternion.Euler (0, processedAngle + 90, 90);
		}

        SetPlayerRotationUp();
    }   

    void SetPlayerRotationUp()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.forward, GetPlayerNormal()), 0.5f);
    }

    void ApplyWheelForces()
	{   
		processerdSpeed = serialValues.values.speed * speedMultiplier;
	    
		if (Input.GetKey (KeyCode.Space)) 
		{	
			backWheel.motorTorque = -processerdSpeed;
		}	 

		else if (processerdSpeed != 0) 
		{	
			backWheel.brakeTorque = 0;
			frontWheel.brakeTorque = 0;

			backWheel.motorTorque = processerdSpeed;
            ApplyVelocityDrag(velocityDrag);
        }	
		else
		{
            ApplyVelocityDrag(velocityDrag);

            backWheel.brakeTorque = breakForce;
			frontWheel.brakeTorque = breakForce;
		}	

	}

    void ApplyHandleBarRotation()
    {
        processedAngle = (serialValues.values.rotation / ((serialValues.arduinoAgent.rotationAnalogRange.range)) - 0.5f) * angleChangeRange;
        frontWheel.steerAngle = processedAngle;
    }
    
    void ApplyVelocityDrag(float drag)
	{	
		playerRigidBody.AddForce (-drag * playerRigidBody.velocity.normalized*Mathf.Abs( Vector3.SqrMagnitude(playerRigidBody.velocity)));
	}	


	private Vector3 GetPlayerNormal()
	{
		Vector3 normal = Vector3.zero;

		WheelHit hit;
		backWheel.GetGroundHit (out hit);
		normal += hit.normal;
		frontWheel.GetGroundHit (out hit);
		normal += hit.normal;


		return normal.normalized;
	}





    private void OnCollisionEnter(Collision collision)
    {   
        GetComponent<Respawn>().RespawnObject();
    }

    
}
