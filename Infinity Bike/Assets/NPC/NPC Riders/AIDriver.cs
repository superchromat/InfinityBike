using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIDriver : MonoBehaviour {


    public AiSettings aiSettings = new AiSettings();

	public TrackNode trackNode = null;
    public WheelCollider backWheel;
    public WheelCollider frontWheel;

    private new Rigidbody rigidbody;

    void Start () 
	{
       
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);

        frontWheel.brakeTorque = 0;
        rigidbody = GetComponent<Rigidbody>();

        SetWheelBrakeTorque();

    }

    void FixedUpdate () 
	{
        frontWheel.steerAngle = SetSteeringAngle();


        SetWheelMotorTorque();
        

        rigidbody.AddForce(-aiSettings.velocityDrag * rigidbody.velocity.normalized * Mathf.Abs(Vector3.SqrMagnitude(rigidbody.velocity)));
    }

    private float SetSteeringAngle()
    {
        int nearestNode = Respawn.FindNearestNode(trackNode, transform);
        Vector3 targetDirection = Vector3.zero;

        for (int j = 0; j < 4; j++)
        {
            Vector3 nextNode = trackNode.GetNode(nearestNode + j + 1);
            targetDirection += (nextNode - frontWheel.transform.position).normalized / (j + 1);
        }

        targetDirection = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;

        float angle = Vector3.Angle(targetDirection, frontWheel.transform.forward);
        if (Vector3.Dot(targetDirection, frontWheel.transform.right) < 0)
        { angle = -angle; }


        return Mathf.Lerp(frontWheel.steerAngle, angle, aiSettings.steeringLerpTime * Time.deltaTime);


    }

    private void SetWheelMotorTorque()
    {   
        backWheel.motorTorque = Mathf.Lerp(backWheel.motorTorque, aiSettings.maxMotorTorque, aiSettings.torqueAcceleration * Time.deltaTime);
    }

    private void SetWheelBrakeTorque()
    {
        frontWheel.brakeTorque = 0;
        backWheel.brakeTorque = 0;
    }

    public float GetVelocity()
    {
        try
        {
            return GetComponent<Rigidbody>().velocity.sqrMagnitude;
        }
        catch
        {
            return 0;
        }

    }
    
    private void OnCollisionEnter(Collision collision)
    {
        backWheel.motorTorque = 0;
        GetComponent<Respawn>().RespawnObject();
        SetWheelBrakeTorque();
    }

    void SetRotationUp()
    {transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.forward, GetNormal()), 0.5f);}

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



    [ContextMenu("Save")]
    public void Save()
    {
        SaveLoad.Save(aiSettings);
    }

}
