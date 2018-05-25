using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class PlayerMovementV2 : MonoBehaviour {

	public float speed = 1f;
	public float rawRotation = 0;
	public float rawSpeed= 0;
	public float angleChangeRange = 180f;
	public float speedMultiplier = 180f;
	public float velocityTorqueDrag = 100f;
	public float velocityTorqueConstant = 100f;

    public WheelCollider backWheel;
    public WheelCollider frontWheel;

	public ArduinoThread storageValue;
	public Transform handleBar;

	//public WheelFrictionCurveSetter wheelFrictionSetter = new WheelFrictionCurveSetter();
	public AnalogRange rotationAnalogRange = new AnalogRange();
	public AnalogRange	speedAnalogRange = new AnalogRange();

	// Use this for initialization
	void Start () 
	{
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);
	}

	// Update is called once per frame
	void FixedUpdate () 
	{	
		
		rawRotation = storageValue.values.rotation;
		rawSpeed = storageValue.values.speed;

		float angle = (rawRotation / ((rotationAnalogRange.maxRawRotation - rotationAnalogRange.minRawRotation)) - 0.5f)*angleChangeRange;
		float actSpeed = rawSpeed / ((speedAnalogRange.maxRawRotation - speedAnalogRange.minRawRotation)) * speed;

		handleBar.localRotation =  Quaternion.Euler(0 , angle + 90 , 90);
        if(Input.GetKey(KeyCode.Space)){
            backWheel.motorTorque = -actSpeed;
        }
        else {
            backWheel.motorTorque = actSpeed;  
        }
		
		frontWheel.steerAngle = angle;

		transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
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







}
