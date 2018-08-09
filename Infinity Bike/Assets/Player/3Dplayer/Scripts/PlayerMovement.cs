using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : Movement
{
    public float speedMultiplier = 1f;
	public float angleChangeRange = 180f;
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
        }
    }

    public Transform handleBar;
    public Vector3 centerOfMass = Vector3.down;
    public ArduinoThread serialValues;


    void Start ()
	{
        MovementStart();
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);

        rb.centerOfMass = centerOfMass;
        IdleMode = true;

    }

    void FixedUpdate()
    {   
        float targetTorque = serialValues.arduinoInfo.arduinoValueStorage.rawSpeed * speedMultiplier;

        if (!IdleMode && targetTorque > 0)
        {   
            Go(targetTorque);
        }   
        else
        {   
            Stop();
            IdleMode = true;
        }

        SetSteeringAngle();

		if (handleBar != null)
        {handleBar.localRotation = Quaternion.Euler (0, TargetAngle + 90, 90);}

        SetRotationUp();
        ApplyVelocityDrag(velocityDrag);
        environementObserver.TallyUpCommingObstacles();
        GameObject folowing;
        if (CheckIfFollowingDriver(out folowing))
        {
            if (folowing != null)
            {   
                float distance = 1+(folowing.transform.position - transform.position).sqrMagnitude;
                if (distance > 1.01f)
                { ApplyVelocityDrag(-velocityDrag / (1 + distance)*2); }
                else
                { ApplyVelocityDrag(-velocityDrag*2); }
            }   
        }
    }   





    protected override void SetSteeringAngle()
    {
        TargetAngle = (serialValues.arduinoInfo.arduinoValueStorage.rawRotation / ((serialValues.arduinoInfo.rotationAnalogRange.range)) - 0.5f) * angleChangeRange;
    }

    protected override void EnterIdleMode()
    { }

    protected override void ExitIdleMode()
    { }


    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Respawn>().CallRespawnAction();
    }
    private void OnDrawGizmos()
    {
      //  Gizmos.DrawSphere(transform.position + transform.forward*2, 0.4f);


    }
}
