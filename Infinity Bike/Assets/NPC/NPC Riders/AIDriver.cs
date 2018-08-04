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


    struct TrajectoryOffset
    {
        public float frequency;
        public float timeOffSet;
        public float spatialOffset;
        public float amplitude;
    }
    TrajectoryOffset trajectoryOffset = new TrajectoryOffset();


    private float timeAlive = 0;


    void Start () 
	{   
        nearestNode = Respawn.FindNearestNode(trackNode, transform) + 1; ;
        pid = GetComponent<AiPid>();
        rb = GetComponent<Rigidbody>();

        aiSettings.SetRandomValues();


        trajectoryOffset.frequency = 1f/Random.Range(1, 100);
        trajectoryOffset.timeOffSet = Random.Range(0, 2f*Mathf.PI);
        trajectoryOffset.spatialOffset = Random.Range(-3, 3);
        trajectoryOffset.amplitude = Random.Range(0.1f, 3- Mathf.Abs(trajectoryOffset.spatialOffset));
        

        Respawn resp = GetComponent<Respawn>();

        resp.OnRespawn = aiSettings.SetRandomValues;
        resp.OnRespawn = pid.ResetValues;
        resp.OnRespawn = Stop;
        resp.OnRespawn = ()=> { nearestNode = Respawn.FindNearestNode(trackNode, transform) + 1; timeAlive = 0;IdleMode = false; };
        resp.OnRespawn();

        timeAlive = 0;
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);
        

        pid.UpdateErrorValue += () => { pid.errorVariable = (aiSettings.targetSqrSpeed - rb.velocity.sqrMagnitude); };
    }

    private void Update()
    {
        timeAlive += Time.deltaTime; ;



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

        if(Vector3.Dot(trackNode.GetNode(nearestNode)- transform.position,transform.forward) < 0  )
        { nearestNode = nearestNode + 1; }

        Vector3 targetDirection = transform.InverseTransformPoint( trackNode.GetNode(nearestNode));
        targetDirection.x += trajectoryOffset.amplitude * Mathf.Sin(trajectoryOffset.frequency * timeAlive+ trajectoryOffset.timeOffSet)+ trajectoryOffset.spatialOffset;



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
