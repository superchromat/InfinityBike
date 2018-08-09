using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EnvironementObserver))]
public abstract class Movement : MonoBehaviour
{
    protected EnvironementObserver environementObserver;
    protected Rigidbody rb;
    public float breakForce = 10000;
    public WheelCollider backWheel;
    public WheelCollider frontWheel;

    public float velocityDrag = 1f;
    public bool isGrounded = true;
    protected float targetAngle = 0;

    protected int closestNode = 0;
    public int ClosestNode
    {
        get { return closestNode; }
    }


    [SerializeField]
    public EnvironementObserver.LayerToReact layerToReact;
    
    [SerializeField]
    protected bool idleMode = true;
    public bool IdleMode
    {
        get
        {   
            return idleMode;
        }   
        set
        {
            idleMode = value;
            if (idleMode)
            {
                EnterIdleMode();
            }   
            else
            {
                ExitIdleMode();
            }   
        }   

    }



    protected void MovementStart()
    {   
        environementObserver = GetComponent<EnvironementObserver>();
        rb = GetComponent<Rigidbody>();
    }   

    protected abstract void EnterIdleMode();
    protected abstract void ExitIdleMode();



    protected abstract void SetSteeringAngle();

    protected void ApplyVelocityDrag(float drag)
    {
        rb.AddForce(-drag * rb.velocity.normalized * Mathf.Abs(Vector3.SqrMagnitude(rb.velocity)));
    }

    protected void SetRotationUp()
    {
        Vector3 normal = Vector3.zero;
        GetNormal(out normal);

        if (isGrounded)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.forward, normal), 50f * Time.deltaTime);
        else
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 50f * Time.deltaTime);

    }

    protected void GetNormal(out Vector3 normal)
    {
        WheelHit hit;
        Vector3 vect = Vector3.zero;
        bool isGrounded = false;
        if (backWheel.GetGroundHit(out hit))
        {
            vect += hit.normal;
            isGrounded = true;
        }

        if (frontWheel.GetGroundHit(out hit))
        {
            vect += hit.normal;
            isGrounded = true;
        }

        normal = vect;
        this.isGrounded = isGrounded;
    }

    protected bool CheckIfFollowingDriver(out GameObject obj)
    {   
        float closestDistance = float.MaxValue;
        bool hitFound = false;
        obj = null;
        foreach (RaycastHit item in environementObserver.hit)
        {
            if ((  (1<<item.transform.gameObject.layer) & layerToReact.npcLayer.value) != 0)
            {
                hitFound = true;
                float distance = Vector3.Distance(transform.position, item.transform.position);
                if (distance < closestDistance)
                {distance = closestDistance;}
                obj = item.collider.gameObject;
            }   
        }

        return hitFound;







    }   
    
    public void Go(float motorTorque)
    {
        backWheel.brakeTorque = 0;
        frontWheel.brakeTorque = 0;
        backWheel.motorTorque = motorTorque;
    }
    public void Stop()
    {
        backWheel.brakeTorque = breakForce;
        frontWheel.brakeTorque = breakForce;
        backWheel.motorTorque = 0;
    }
    
}   
