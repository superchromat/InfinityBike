using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{   

	public float speedMultiplier = 1f;
	public float angleChangeRange = 180f;

	public float velocityDrag = 1f;
    public float breakForce = 10000;
    
    public WheelCollider backWheel;
	public WheelCollider frontWheel;

	public ArduinoThread serialValues;
	public Transform handleBar;

	private float processedAngle = 0;
	private float processedSpeed = 0;
	private Rigidbody playerRigidBody;
    public Vector3 centerOfMass = Vector3.down;
    
	// Use this for initialization
	void Start () 
	{
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);


        playerRigidBody = GetComponent<Rigidbody> ();
        playerRigidBody.centerOfMass = centerOfMass;

    }

    // Update is called once per frame


    void FixedUpdate()
    {

        Debug.DrawLine(transform.TransformPoint( playerRigidBody.centerOfMass), transform.position);

        ApplyHandleBarRotation();
        ApplyWheelForces();

		if (handleBar != null)
        {   
			handleBar.localRotation = Quaternion.Euler (0, processedAngle + 90, 90);
		}   

        SetPlayerRotationUp();
    }   


    void ApplyWheelForces()
	{   
		processedSpeed = serialValues.arduinoInfo.arduinoValueStorage.rawSpeed * speedMultiplier;

        if (processedSpeed != 0) 
		{
            backWheel.motorTorque = 0;
            backWheel.brakeTorque = 0;
            frontWheel.brakeTorque = 0;

            backWheel.motorTorque = processedSpeed;
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
        processedAngle = (serialValues.arduinoInfo.arduinoValueStorage.rawRotation / ((serialValues.arduinoInfo.rotationAnalogRange.range)) - 0.5f) * angleChangeRange;
        frontWheel.steerAngle = processedAngle;
    }
    void ApplyVelocityDrag(float drag)
	{	
		playerRigidBody.AddForce (-drag * playerRigidBody.velocity.normalized*Mathf.Abs( Vector3.SqrMagnitude(playerRigidBody.velocity)));
	}
    void SetPlayerRotationUp()
    {
        Vector3 normal = Vector3.zero;

        if(GetNormal(out normal))
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.forward,normal), 50f * Time.deltaTime);

    }


    private bool GetNormal(out Vector3 normal)
	{
		WheelHit hit;
        Vector3 vect = Vector3.zero;
        bool isGrounded = false;
        if (backWheel.GetGroundHit(out hit))
        {
            vect += hit.normal;
            isGrounded = true;
        }

        if (frontWheel.GetGroundHit(out hit))
        {
            vect += hit.normal;
            isGrounded = true;
        }

        normal = vect;
        return isGrounded;
	}

    private void OnCollisionEnter(Collision collision)
    {   
        GetComponent<Respawn>().onRespawn();
    }

}
