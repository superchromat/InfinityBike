using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDriver : Movement
{   
    private int closestNode;
    private int ClosestNode
    {
        get { return closestNode + 10; }
        set { closestNode = value;   }

    }

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
            if (float.IsNaN(value))
            { targetAngle = 0; }
            else
            { targetAngle = value; }

            if (targetAngle > maximumSteeringAngle)
            { targetAngle = maximumSteeringAngle; }
            else if (targetAngle < -maximumSteeringAngle)
            { targetAngle = -maximumSteeringAngle; }
            frontWheel.steerAngle = targetAngle;
        }

    }

    public float velocity;
    private Vector3 nextWaypoint;
    
    private float timeAlive = 0;
    Respawn respawn = null;

    void Start () 
	{

        respawn = GetComponent<Respawn>(); ;

        ClosestNode = Respawn.FindNearestNode(trackNode, transform);
        pid = GetComponent<AiPid>();
        rb = GetComponent<Rigidbody>();

        aiSettings.SetRandomValues();

        Respawn resp = GetComponent<Respawn>();

        resp.OnRespawn = aiSettings.SetRandomValues;
        resp.OnRespawn = pid.ResetValues;
        resp.OnRespawn = Stop;
        resp.OnRespawn = ()=> { ClosestNode = Respawn.FindNearestNode(trackNode, transform); timeAlive = 0; StartCoroutine (SetIdleOnATimer(1f)); };
        resp.OnRespawn();

        timeAlive = 0;
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);
        

        pid.UpdateErrorValue += () => { pid.errorVariable = (aiSettings.targetSqrSpeed - rb.velocity.sqrMagnitude); };
    }



    private void Update()
    {
        timeAlive += Time.deltaTime;

        velocity = rb.velocity.sqrMagnitude;
        SetRotationUp();

        nextWaypoint = GetNextWayPoint();
        Vector3 trackDirection = (trackNode.GetNode(ClosestNode + 1) - trackNode.GetNode(ClosestNode)).normalized;
        if (Vector3.Dot(trackDirection, transform.forward) < 0)
        {respawn.OnRespawn();}
        
        Debug.DrawLine(transform.position, transform.TransformPoint(nextWaypoint), Color.blue);

    }   

    void FixedUpdate () 
	{
        if (!IdleMode && isGrounded)
        {
            pid.RunPID();
            Go(pid.controlVariable);
        }

        SetSteeringAngle();
        ApplyVelocityDrag(velocityDrag);


        if (trackNode.isLoopOpen && (ClosestNode + 1 >= (trackNode.GetNodeCount())))
        { IdleMode = true; }
    }

    protected override void EnterIdleMode()
    {   
        Stop();
    }   
    
    protected override void ExitIdleMode()
    {   
    }   

    protected override void SetSteeringAngle()
    {
        float dot = nextWaypoint.x / nextWaypoint.magnitude;
        TargetAngle = Mathf.Lerp(TargetAngle, dot * maximumSteeringAngle, aiSettings.turnSpeed*Time.fixedDeltaTime);
    }

    private Vector3 GetNextWayPoint()
    {
        ClosestNode = Respawn.FindNearestNode(trackNode, transform);

        Vector3 targetDirection = transform.InverseTransformPoint(trackNode.GetNode(ClosestNode));
        targetDirection.x += aiSettings.trajectoryOffset.amplitude * Mathf.Sin(aiSettings.trajectoryOffset.frequency * timeAlive + aiSettings.trajectoryOffset.timeOffSet) + aiSettings.trajectoryOffset.transverseOffset;
        return targetDirection;
    }

    private void OnCollisionEnter(Collision collision)
    {   
        try
        { respawn.OnRespawn(); }
        catch (System.NullReferenceException)
        { Debug.LogError(GetComponent<Respawn>().name); }
    }

    IEnumerator SetIdleOnATimer(float timeIdle)
    {
        IdleMode = true;

        yield return new WaitForSeconds(timeIdle);

        IdleMode = false;
    }



}
