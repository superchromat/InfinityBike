using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIDriver : MonoBehaviour
{   
    
    public AiSettings aiSettings = new AiSettings();

	public TrackNode trackNode = null;

    public WheelCollider backWheel;
    public WheelCollider frontWheel;

    private new Rigidbody rigidbody;

    private AiPid pid;
    private int nearestNode;

    void Start () 
	{   
        nearestNode = 0;
        pid = GetComponent<AiPid>();
        
        aiSettings.SetRandomValues();

        GetComponent<Respawn>().onRespawn += aiSettings.SetRandomValues;
        GetComponent<Respawn>().onRespawn += pid.ResetValues;
        GetComponent<Respawn>().onRespawn += ()=>{aiSettings.maxMotorTorque = 0;frontWheel.brakeTorque = 0;backWheel.brakeTorque = 0;};
        
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);
        
        rigidbody = GetComponent<Rigidbody>();

        frontWheel.brakeTorque = 0;
        backWheel.brakeTorque = 0;
        SetUpPIDReferences();
    }   

    void FixedUpdate () 
	{
        frontWheel.brakeTorque = 0;
        backWheel.brakeTorque = 0;

        SetRotationUp();
        frontWheel.steerAngle = SetSteeringAngle();
        rigidbody.AddForce(-aiSettings.velocityDrag * rigidbody.velocity.normalized * Mathf.Abs(Vector3.SqrMagnitude(rigidbody.velocity)));
        SetWheelMotorTorque();

        if (trackNode.isLoopOpen && nearestNode+1 == (trackNode.GetNodeCount() ))
        {SetAIToIdleMode();}

    }   

    private float SetSteeringAngle()
    {   
        nearestNode = Respawn.FindNearestNode(trackNode, transform);
        float farNodeWeightHolder = 1;
        Vector3 targetDirection = Vector3.zero;

        Debug.DrawRay(frontWheel.transform.position, frontWheel.transform.forward*2, Color.blue);

        int numberOfNodes = aiSettings.numberNodeInPrediction;
        for (int j = 0; j < aiSettings.numberNodeInPrediction; j++)
        {
            Vector3 nextNode = trackNode.GetNode(nearestNode + j);
            Vector3 nextDirection = (nextNode - frontWheel.transform.position).normalized;
            nextDirection -= Vector3.Dot(nextDirection, frontWheel.transform.up) * frontWheel.transform.up;

            if (j == 0)
            {
                if (Vector3.Dot(nextDirection, frontWheel.transform.forward) > 0.5)
                {
                    targetDirection += nextDirection;
                    Debug.DrawRay(frontWheel.transform.position, nextDirection, Color.red);
                }
                else
                {
                    numberOfNodes++;
                }
            }   
            else
            {
                targetDirection += nextDirection * farNodeWeightHolder;
                Debug.DrawRay(frontWheel.transform.position, nextDirection);

            }
            farNodeWeightHolder *= aiSettings.farNodeWeight;
        }

        targetDirection = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;


        float angle = Vector3.Angle(targetDirection, frontWheel.transform.forward);
        if (Vector3.Dot(targetDirection, frontWheel.transform.right) < 0)
        { angle = -angle; }
        
        float resultAngle = Mathf.Lerp(frontWheel.steerAngle, angle, aiSettings.steeringLerpTime * Time.deltaTime);
        if (resultAngle > 45)
        { resultAngle = 45; }
        else if (resultAngle < -45)
        {resultAngle = -45;}

        return resultAngle;

    }   

    private void SetWheelMotorTorque()
    {   
        backWheel.motorTorque = Mathf.Lerp(backWheel.motorTorque, aiSettings.maxMotorTorque, aiSettings.torqueAcceleration*Time.deltaTime);
    }   

    private void OnCollisionEnter(Collision collision)
    {   
        try
        {GetComponent<Respawn>().onRespawn();}
        catch(NullReferenceException)
        {Debug.LogError(GetComponent<Respawn>().name);}

        frontWheel.brakeTorque = 0;
        backWheel.brakeTorque = 0;
    }   

    void SetRotationUp()
    { transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.forward, GetNormal()), 100f * Time.deltaTime); }

    private Vector3 GetNormal()
    {
        Vector3 normal = Vector3.zero;

        WheelHit hit;
        backWheel.GetGroundHit(out hit);
        normal += hit.normal;
        frontWheel.GetGroundHit(out hit);
        normal += hit.normal;


        return normal.normalized;
    }


    void SetAIToIdleMode()
    {
        aiSettings.targetSqrSpeed = 0;
    }   


    void SetUpPIDReferences()
    {
        pid.UpdateErrorValue += PIDerrorCalc;
        pid.ReactToControlVariable += PIDActiveControl;
    }   


    void PIDerrorCalc()
    {
        pid.errorVariable = (aiSettings.targetSqrSpeed - rigidbody.velocity.sqrMagnitude);
        //Debug.Log(rigidbody.velocity.sqrMagnitude);
        Debug.Log(backWheel.motorTorque);
    }

    void PIDActiveControl()
    {
        if (aiSettings.targetSqrSpeed != 0)
        {   
            aiSettings.maxMotorTorque = pid.controlVariable;
        }   
        else
        {   
            aiSettings.maxMotorTorque = 0;
            backWheel.brakeTorque = pid.controlVariable;
        }   

    }

}   
