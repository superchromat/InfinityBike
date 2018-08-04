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
    public float maximumSteeringAngle = 45;
    private float TargetAngle
    {
        get { return targetAngle; }
        set
        {
            targetAngle = value;
            if (targetAngle > maximumSteeringAngle)
            { targetAngle = maximumSteeringAngle; }
            else if (targetAngle < -maximumSteeringAngle)
            { targetAngle = -maximumSteeringAngle; }
            frontWheel.steerAngle = TargetAngle;
        }

    }
    public float velocity;


    void Start () 
	{   
        nearestNode = Respawn.FindNearestNode(trackNode, transform) + 1; ;
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

    private void Update()
    {

        if(Vector3.Distance(trackNode.GetNode(nearestNode),transform.position) > Vector3.Distance(trackNode.GetNode(nearestNode+1),transform.position) )
        {   
            nearestNode = nearestNode + 1;
        }   

        velocity = rb.velocity.sqrMagnitude;
        SetRotationUp();
        SetSteeringAngle();

    }

    void FixedUpdate () 
	{
        pid.RunPID();

        Go(pid.controlVariable);


        ApplyVelocityDrag(velocityDrag);

        if (trackNode.isLoopOpen && (nearestNode + 1 >= (trackNode.GetNodeCount())))
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

     //   nearestNode = Respawn.FindNearestNode(trackNode, transform) + 1;

        Vector3 targetDirection = transform.InverseTransformPoint( trackNode.GetNode(nearestNode));
        float dot = targetDirection.x / targetDirection.magnitude;

        TargetAngle = dot * maximumSteeringAngle;
    }   
         
    private void OnCollisionEnter(Collision collision)
    {
        try
        { GetComponent<Respawn>().OnRespawn(); }
        catch (System.NullReferenceException)
        { Debug.LogError(GetComponent<Respawn>().name); }
    }

}
/*
    float farNodeWeightHolder = 1;
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
        }
        else
        {   
            numberOfNodes++;
        }   

        farNodeWeightHolder *= aiSettings.farNodeWeight;
    }
    */
