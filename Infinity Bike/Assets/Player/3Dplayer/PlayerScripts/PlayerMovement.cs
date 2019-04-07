using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : Movement
{
    public float speedMultiplier = 1f;
	public float angleChangeRange = 180f;

    //To handle animation 
    public PlayerAnimatorScript playerAnimatorScript; 

    private float TargetAngle
    {
        get
        {
            return targetAngle;
        }
        set
        {
            targetAngle = value;
            frontWheel.steerAngle = value;
            playerAnimatorScript.UpdateSteeringAngle(value);
        }
    }

    public Transform handleBar;
    public Vector3 centerOfMass = Vector3.down;
    public ArduinoThread serialValues;
    public Respawn respawn;

    void Start ()
	{
        respawn = GetComponent<Respawn>();
        MovementStart();
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);

        rb.centerOfMass = centerOfMass;
        IdleMode = true;

    }
    private void Update()
    {   
        closestNode = Respawn.FindNearestNode(respawn.trackNode, transform);
    }   
    void FixedUpdate()
    {   
        float targetTorque = serialValues.arduinoInfo.arduinoValueStorage.rawSpeed * speedMultiplier;

        if (!IdleMode && targetTorque > 0)
        {   

            Go(targetTorque);
            playerAnimatorScript.UpdateCyclingSpeed(targetTorque);
        }   
        else
        {   
            Stop(breakForce);
            playerAnimatorScript.UpdateCyclingSpeed(0);
            IdleMode = true;
        }

        SetSteeringAngle();

		if (handleBar != null)
        {handleBar.localRotation = Quaternion.Euler (0, TargetAngle + 90, 90);}

        SetRotationUp();
        ApplyVelocityDrag(velocityDrag);


    }   





    protected override void SetSteeringAngle()
    {
        TargetAngle = (serialValues.arduinoInfo.arduinoValueStorage.rawRotation / ((serialValues.arduinoInfo.rotationAnalogRange.range)) - 0.5f) * angleChangeRange; // TODO TargetAngle (remove Cap from T)

    }

    protected override void EnterIdleMode()
    { }

    protected override void ExitIdleMode()
    { }


    private void OnCollisionEnter(Collision collision)
    {
        respawn.CallRespawnAction();
    }
    private void OnDrawGizmos()
    {
      //  Gizmos.DrawSphere(transform.position + transform.forward*2, 0.4f);


    }
}
