using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDriver : Movement
{   
    private int nearestNode;

    [SerializeField]
    public AiSettings aiSettings = new AiSettings();
	public TrackNode trackNode = null;
    private AiPid pid = null;
    private float TargetAngle
    {
        get { return targetAngle; }
        set
        {
            targetAngle = value;
            if (targetAngle > 45)
            { targetAngle = 45; }
            else if (targetAngle < -45)
            { targetAngle = -45; }
            frontWheel.steerAngle = TargetAngle;
        }

    }
    public float velocity;


    void Start () 
	{   
        nearestNode = 0;
        pid = GetComponent<AiPid>();
        rb = GetComponent<Rigidbody>();

        aiSettings.SetRandomValues();

        GetComponent<Respawn>().OnRespawn = aiSettings.SetRandomValues;
        GetComponent<Respawn>().OnRespawn = pid.ResetValues;
        GetComponent<Respawn>().OnRespawn = Stop;
        
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);
        

        pid.UpdateErrorValue += () => { pid.errorVariable = (aiSettings.targetSqrSpeed - rb.velocity.sqrMagnitude); };
    }   

    void FixedUpdate () 
	{
        velocity = rb.velocity.sqrMagnitude;
        pid.RunPID();

        Go(pid.controlVariable);

        SetRotationUp();
        SetSteeringAngle();
        ApplyVelocityDrag(velocityDrag);

        if (trackNode.isLoopOpen && (nearestNode + 1 == (trackNode.GetNodeCount())))
        { IdleMode = true; }
    }

    protected override void EnterIdleMode()
    {   
        aiSettings.targetSqrSpeed = 0;
        TargetAngle = 0;
    }
    
    protected override void ExitIdleMode()
    {   
        aiSettings.SetRandomValues();
    }   

    protected override void SetSteeringAngle()
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

        TargetAngle = Mathf.Lerp(frontWheel.steerAngle, angle, aiSettings.steeringLerpTime * Time.deltaTime);
    }   
         
    private void OnCollisionEnter(Collision collision)
    {
        try
        { GetComponent<Respawn>().OnRespawn(); }
        catch (System.NullReferenceException)
        { Debug.LogError(GetComponent<Respawn>().name); }
    }

}
