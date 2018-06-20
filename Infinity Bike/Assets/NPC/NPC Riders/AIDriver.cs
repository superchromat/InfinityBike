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
    
    void Start () 
	{
        aiSettings.SetRandomValues();
        GetComponent<Respawn>().onRespawn += aiSettings.SetRandomValues;
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);
        rigidbody = GetComponent<Rigidbody>();
    }   

    void FixedUpdate () 
	{
        SetRotationUp();
        frontWheel.steerAngle = SetSteeringAngle();
        SetWheelMotorTorque();
        rigidbody.AddForce(-aiSettings.velocityDrag * rigidbody.velocity.normalized * Mathf.Abs(Vector3.SqrMagnitude(rigidbody.velocity)));

    }

    private float SetSteeringAngle()
    {   
        int nearestNode = Respawn.FindNearestNode(trackNode, transform);
        float farNodeWeightHolder = 1;
        Vector3 targetDirection = Vector3.zero;
        for (int j = 0; j < aiSettings.numberNodeInPrediction; j++)
        {
            Vector3 nextNode = trackNode.GetNode(nearestNode + j);
            if (j == 0)
            {   
                if (Vector3.Dot((nextNode - frontWheel.transform.position), frontWheel.transform.forward) > 0)
                {
                    targetDirection += (nextNode - frontWheel.transform.position);
                //    Debug.DrawLine(transform.position, trackNode.GetNode(nearestNode));
                }
            }   
            else
            {   
             //   Debug.DrawLine(transform.position, nextNode);
                targetDirection += (nextNode - frontWheel.transform.position) * farNodeWeightHolder;
            }
            farNodeWeightHolder *= aiSettings.farNodeWeight;
        }



        targetDirection = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;
     //   Debug.DrawRay(transform.position, targetDirection*5f, Color.red);


        float angle = Vector3.Angle(targetDirection, frontWheel.transform.forward);
        if (Vector3.Dot(targetDirection, frontWheel.transform.right) < 0)
        { angle = -angle; }


        return Mathf.Lerp(frontWheel.steerAngle, angle, aiSettings.steeringLerpTime * Time.deltaTime);


    }

    private void SetWheelMotorTorque()
    {
        SetWheelBrakeTorque();
        backWheel.motorTorque = Mathf.Lerp(backWheel.motorTorque, aiSettings.maxMotorTorque, aiSettings.torqueAcceleration * Time.deltaTime);
    }   

    private void SetWheelBrakeTorque()
    {
        frontWheel.brakeTorque = 0;
        backWheel.brakeTorque = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        backWheel.motorTorque = 0;
        GetComponent<Respawn>().onRespawn();
        SetWheelBrakeTorque();
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

}
