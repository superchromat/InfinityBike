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

	public ArduinoThread storageValue;
	public Transform handleBar;

	//public WheelFrictionCurveSetter wheelFrictionSetter = new WheelFrictionCurveSetter();
	public AnalogRange rotationAnalogRange = new AnalogRange();
	public AnalogRange	speedAnalogRange = new AnalogRange();

	private float processerdAngle = 0;
	private float processerdSpeed = 0;

	public float breakForce = 1000;
	private Rigidbody playerRigidBody;
  //  private bool onTriggerDone = false;

    
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
			handleBar.localRotation = Quaternion.Euler (0, processerdAngle + 90, 90);
		}
		transform.rotation = Quaternion.LookRotation (transform.forward, GetPlayerNormal ());
	}	
    
	void ApplyWheelForces()
	{   
		processerdSpeed = storageValue.values.speed / ((speedAnalogRange.maxRawRotation - speedAnalogRange.minRawRotation)) * speedMultiplier;
	    
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
            ApplyVelocityDrag(1);

            backWheel.brakeTorque = breakForce;
			frontWheel.brakeTorque = breakForce;
		}	

	}

    void ApplyHandleBarRotation()
    {
        processerdAngle = (storageValue.values.rotation / ((rotationAnalogRange.maxRawRotation - rotationAnalogRange.minRawRotation)) - 0.5f) * angleChangeRange;
        frontWheel.steerAngle = processerdAngle;
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


	[Serializable]
	public class AnalogRange
	{
		public float minRawRotation;
		public float maxRawRotation;

		public AnalogRange(float minRawRotation, float maxRawRotation)
		{
			this.minRawRotation = minRawRotation;
			this.maxRawRotation = maxRawRotation;
		}

		public AnalogRange()
		{
			this.minRawRotation = 0;
			this.maxRawRotation = 1024;
		}
	}


    private void OnCollisionEnter(Collision collision)
    {   
        GetComponent<Respawn>().RespawnObject();

        //if (onTriggerDone == false)
        //{

        //    onTriggerDone = true;
        //    Vector3 normal = collision.contacts[0].normal;


        //    transform.rotation = Quaternion.LookRotation(transform.forward, normal);
        //}
    }



}
