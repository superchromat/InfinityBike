using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDriver : MonoBehaviour
{   
    [SerializeField]
    public AiSettings aiSettings = new AiSettings();

	public TrackNode trackNode = null;


    private AiPid pid;
    private int nearestNode;



    private Rigidbody rb;
    public float breakForce = 10000;
    public WheelCollider backWheel;
    public WheelCollider frontWheel;

    void Start () 
	{   
        nearestNode = 0;
        pid = GetComponent<AiPid>();
        
        aiSettings.SetRandomValues();

        GetComponent<Respawn>().OnRespawn = aiSettings.SetRandomValues;
        GetComponent<Respawn>().OnRespawn = pid.ResetValues;
        GetComponent<Respawn>().OnRespawn = ()=>{StartCoroutine(RestartHelper()); };
        
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);
        
        rb = GetComponent<Rigidbody>();

        SetUpPIDReferences();
    }   

    void FixedUpdate () 
	{   
        SetRotationUp();
        frontWheel.steerAngle = SetSteeringAngle();
        rb.AddForce(-aiSettings.velocityDrag * rb.velocity.normalized * Mathf.Abs(Vector3.SqrMagnitude(rb.velocity)));
        Go();

        if (trackNode.isLoopOpen && nearestNode + 1 == (trackNode.GetNodeCount() ))
        {SetAIToIdleMode();}

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
        pid.errorVariable = (aiSettings.targetSqrSpeed - rb.velocity.sqrMagnitude);
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
    
    IEnumerator RestartHelper()
    {
        Stop();
        yield return null;
        Go();
    }



    private float SetSteeringAngle()
    {
        nearestNode = Respawn.FindNearestNode(trackNode, transform);
        float farNodeWeightHolder = 1;
        Vector3 targetDirection = Vector3.zero;

        Debug.DrawRay(frontWheel.transform.position, frontWheel.transform.forward * 2, Color.blue);

        int numberOfNodes = aiSettings.numberNodeInPrediction;
        for (int j = 0; j < aiSettings.numberNodeInPrediction; j++)
        {
            int setNode = aiSettings.numberOfNodeAhead + j;
            Vector3 nextNode = trackNode.GetNode(nearestNode + setNode);
            Vector3 nextDirection = (nextNode - frontWheel.transform.position).normalized;
            nextDirection -= Vector3.Dot(nextDirection, frontWheel.transform.up) * frontWheel.transform.up;

            if (Vector3.Dot(nextDirection, frontWheel.transform.forward) > 0.25)
            {
                targetDirection += nextDirection * farNodeWeightHolder;
                if (j == 0)
                    Debug.DrawRay(frontWheel.transform.position, nextDirection, Color.red);
                else
                    Debug.DrawRay(frontWheel.transform.position, nextDirection, Color.white);
            }
            else
            {
                numberOfNodes++;
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
        { resultAngle = -45; }

        return resultAngle;

    }

    public void Go()
    {
        backWheel.brakeTorque = 0;
        frontWheel.brakeTorque = 0;
        backWheel.motorTorque = aiSettings.maxMotorTorque;
    }

    public void Stop()
    {
        backWheel.brakeTorque = breakForce;
        frontWheel.brakeTorque = breakForce;
        backWheel.motorTorque = 0;
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

    private void OnCollisionEnter(Collision collision)
    {
        try
        { GetComponent<Respawn>().OnRespawn(); }
        catch (System.NullReferenceException)
        { Debug.LogError(GetComponent<Respawn>().name); }
    }

}
