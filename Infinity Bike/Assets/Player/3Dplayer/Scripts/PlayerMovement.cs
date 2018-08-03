using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{   

	public float speedMultiplier = 1f;

	public float angleChangeRange = 180f;

	public float velocityDrag = 1f;
    
	public ArduinoThread serialValues;

	public Transform handleBar;

	private float processedAngle = 0;

	private float processedSpeed = 0;

    public Vector3 centerOfMass = Vector3.down;


    void Start () 
	{   
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);

        rb = GetComponent<Rigidbody> ();
        rb.centerOfMass = centerOfMass;
    }

    void FixedUpdate()
    {   
        Debug.DrawLine(transform.TransformPoint( rb.centerOfMass), transform.position);

        SetSteeringAngle();
        ApplyWheelForces();

		if (handleBar != null)
        {   
			handleBar.localRotation = Quaternion.Euler (0, processedAngle + 90, 90);
		}   

        SetRotationUp();
    }

    void ApplyWheelForces()
	{   
		processedSpeed = serialValues.arduinoInfo.arduinoValueStorage.rawSpeed * speedMultiplier;
        
        if (processedSpeed != 0) 
		{   
            Go();
        }	
		else
		{   
            Stop();
        }   
        
        ApplyVelocityDrag(velocityDrag);
    }   

    void ApplyVelocityDrag(float drag)
	{	
		rb.AddForce (-drag * rb.velocity.normalized*Mathf.Abs( Vector3.SqrMagnitude(rb.velocity)));
	}




    private Rigidbody rb;
    public float breakForce = 10000;
    public WheelCollider backWheel;
    public WheelCollider frontWheel;
    void SetSteeringAngle()
    {
        processedAngle = (serialValues.arduinoInfo.arduinoValueStorage.rawRotation / ((serialValues.arduinoInfo.rotationAnalogRange.range)) - 0.5f) * angleChangeRange;
        frontWheel.steerAngle = processedAngle;
    }

    public void Go()
    {
        backWheel.brakeTorque = 0;
        frontWheel.brakeTorque = 0;
        backWheel.motorTorque = processedSpeed;
    }

    public void Stop()
    {
        backWheel.brakeTorque = breakForce;
        frontWheel.brakeTorque = breakForce;
        backWheel.motorTorque = 0;
    }

    void SetRotationUp()
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
        GetComponent<Respawn>().OnRespawn();
    }

}
