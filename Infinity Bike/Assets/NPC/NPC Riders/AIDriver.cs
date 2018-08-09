using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDriver : Movement
{   

    [SerializeField]
    public AiSettings aiSettings = new AiSettings();
	public TrackNode trackNode = null;
    private AiPid pid = null;
    public float maximumSteeringAngle = 45;

    private int waypointNodeID;
    public float velocity;
    private Vector3 nextWaypoint;

    private float timeAlive = 0;
    Respawn respawn = null;

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

        NPCspawner npcSpawner = GetComponentInParent<NPCspawner>();

        //waypointNodeID = respawn.respawnNode = Respawn.FindNearestNode(npcSpawner.trackNodes, npcSpawner.player.transform) - 10;
        waypointNodeID = respawn.respawnNode = Random.Range(0, trackNode.GetNodeCount());

       respawn.AddToRespawnAction(() => { waypointNodeID = respawn.respawnNode = Random.Range(0, trackNode.GetNodeCount()); /*Respawn.FindNearestNode(npcSpawner.trackNodes, npcSpawner.player.transform) - 10 ;*/ });
        respawn.AddToRespawnAction(aiSettings.SetRandomValues);
        respawn.AddToRespawnAction(pid.ResetValues);
        respawn.AddToRespawnAction(Stop);
        respawn.AddToRespawnAction(()=> { timeAlive = 0; StartCoroutine (SetIdleOnATimer(0.2f)); });
        respawn.CallRespawnAction();
        
        timeAlive = 0;
                
        pid.UpdateErrorValue += () => { pid.errorVariable = (aiSettings.targetSqrSpeed - rb.velocity.sqrMagnitude); };


        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);
    }   
    
    private void Update()
    {
        timeAlive += Time.deltaTime;

        velocity = rb.velocity.sqrMagnitude;
        SetRotationUp();

        UpdateWaypointID();

        nextWaypoint = GetNextWayPoint();
        Vector3 trackDirection = (trackNode.GetNode(waypointNodeID + 1) - trackNode.GetNode(waypointNodeID)).normalized;
        if (Vector3.Dot(trackDirection, transform.forward) < 0)
        {
            respawn.CallRespawnAction();

        }

        //Debug.DrawLine(transform.position,trackNode.GetNode(waypointNodeID), Color.blue);
        //Debug.Log(waypointNodeID);

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
        Vector3 targetDirection = transform.InverseTransformPoint(trackNode.GetNode(waypointNodeID+2));// added 2 to make sure the driver always looks forward.
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
        yield return new WaitForSeconds(timeIdle);
        IdleMode = false;
    }   
    
    void UpdateWaypointID()
    {
        float nextDistance = Vector3.Dot(trackNode.GetNode(waypointNodeID) - transform.position, transform.forward);

        if (nextDistance < 0)
        {
            waypointNodeID = Respawn.FindNearestNode(trackNode, transform);
        }
        else
        {
            float lastDistance = Vector3.Dot(trackNode.GetNode(waypointNodeID - 1) - transform.position, transform.forward);

            if (nextDistance >= 0 && lastDistance < 0)
            {
                waypointNodeID++;
            }
        }   
                
    }
  
}
