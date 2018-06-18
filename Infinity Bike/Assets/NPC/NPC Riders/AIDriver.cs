using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDriver : MonoBehaviour {


	public TrackNode trackNode = null;
	public float motorTorque = 10f;
	public float steeringLerpTime = 0.1f;


    private float initiatingTime = 0f;

	public float lifeTimer = 0f;
	public float lifeTimeLength = 20f;

    public WheelCollider backWheel;
    public WheelCollider frontWheel;

    private Rigidbody rigidbody = null;

    public float velocityDrag = 1f;

    int nearestNode = 0;
//	private MeshRenderer rend = null;

	// Use this for initialization
	void Start () 
	{

        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);

        initiatingTime = Time.time;
        frontWheel.brakeTorque = 0;
        rigidbody = GetComponent<Rigidbody>();


    }	
	
	// Update is called once per frame
	void FixedUpdate () 
	{	
		//transform.position = CalculateNextTarget ();
	    nearestNode = Respawn.FindNearestNode (trackNode, transform);
        Vector3 targetDirection = Vector3.zero;

        for (int j = 0; j < 4; j++)
        {
            Vector3 nextNode = trackNode.GetNode(nearestNode + j + 1) ;
            targetDirection +=(nextNode - frontWheel.transform.position).normalized / (j + 1);



        }





        targetDirection = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;

        float angle = Vector3.Angle(targetDirection, frontWheel.transform.forward);
        if (Vector3.Dot(targetDirection, frontWheel.transform.right) < 0)
        {angle = -angle;}
        

        frontWheel.steerAngle = Mathf.Lerp(frontWheel.steerAngle,angle,steeringLerpTime*Time.deltaTime);
        backWheel.motorTorque = motorTorque;
        frontWheel.brakeTorque = 0;
        backWheel.brakeTorque = 0;

        ApplyVelocityDrag(velocityDrag);
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

    void ApplyVelocityDrag(float drag)
    {
        rigidbody.AddForce(-drag * rigidbody.velocity.normalized * Mathf.Abs(Vector3.SqrMagnitude(rigidbody.velocity)));
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Respawn>().RespawnObject();
    }

    void SetRotationUp()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.forward, GetNormal()), 0.5f);
    }


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
