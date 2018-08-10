using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDriver : Movement
{   

    [SerializeField]
    public AiSettings aiSettings = new AiSettings();
	public TrackNode trackNode = null;
    public AiPid pid = null;
    public float maximumSteeringAngle = 45;

    private int waypointNodeID;
    public float velocitySqr;
    private Vector3 nextWaypoint;

    private float timeAlive = 0;
    Respawn respawn = null;
    public int lookAhead = 2;

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
    

    void Start()
    {   
        MovementStart();

        respawn = GetComponent<Respawn>(); ;
        pid = GetComponent<AiPid>();

        aiSettings.SetRandomValues();

        respawn.respawnNode = 0;
        waypointNodeID = respawn.respawnNode + 1;
        respawn.AddToRespawnAction(()=> { waypointNodeID = 0; });
        respawn.AddToRespawnAction(pid.ResetValues);
        respawn.AddToRespawnAction(()=> { Stop(breakForce); });
        respawn.AddToRespawnAction(() => { respawn.respawnNode = waypointNodeID - 1; });
        respawn.AddToRespawnAction(()=> { timeAlive = 0; StartCoroutine (SetIdleOnATimer(1f)); });
        respawn.AddToRespawnAction(respawn.RespawnObject);
        respawn.CallRespawnAction();

        timeAlive = 0;
                
        pid.UpdateErrorValue += () => { pid.errorVariable = (aiSettings.targetSqrSpeed - Vector3.Dot(rb.velocity,transform.forward)* Vector3.Dot(rb.velocity, transform.forward)); };
        
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);
    }   

 

    private void Update()
    {
        timeAlive += Time.deltaTime;

        velocitySqr =Vector3.Dot(rb.velocity, transform.forward)* Vector3.Dot(rb.velocity, transform.forward);
        SetRotationUp();

        UpdateWaypointID();
        nextWaypoint = GetNextWayPoint();

        Vector3 trackDirection = (trackNode.GetNode(waypointNodeID + 1) - trackNode.GetNode(waypointNodeID)).normalized;
        if (Vector3.Dot(trackDirection, transform.forward) < 0)
        {respawn.CallRespawnAction();}

        Debug.DrawLine(transform.position,trackNode.GetNode(waypointNodeID+ lookAhead), Color.blue);

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
        if (trackNode.isLoopOpen && ((waypointNodeID + 1) >= (trackNode.GetNodeCount())))
        { IdleMode = true; }
    }   

    protected override void EnterIdleMode()
    {   
        Stop(breakForce);
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
        Vector3 targetDirection = transform.InverseTransformPoint(trackNode.GetNode(waypointNodeID+ lookAhead));
        targetDirection.x += aiSettings.trajectoryOffset.amplitude * Mathf.Sin(aiSettings.trajectoryOffset.frequency * timeAlive + aiSettings.trajectoryOffset.timeOffSet) + aiSettings.trajectoryOffset.transverseOffset;

        return targetDirection;
    }



    private void OnCollisionEnter(Collision collision)
    {   
        try
        {
            respawn.CallRespawnAction();
        }
        catch (System.NullReferenceException)
        { Debug.LogError(GetComponent<Respawn>().name); }
    }   

    IEnumerator SetIdleOnATimer(float timeIdle)
    {   
        IdleMode = true;
        do
        {
            yield return new WaitForSeconds(timeIdle);
        } while (!isGrounded);
        IdleMode = false;

    }

    void UpdateWaypointID()
    {   
        waypointNodeID = Respawn.FindNearestNode(trackNode, transform);
    }   
  
}
