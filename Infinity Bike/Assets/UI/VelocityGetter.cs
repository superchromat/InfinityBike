using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VelocityGetter : MonoBehaviour {

    public CommonCanvasVariables commonCanvasVariables;
    private Rigidbody playerRB;
    private Text vel;
    private float lastVelocity;
    public float Velocity{get{return lastVelocity; }}
    public TimerController timerController;
    float timeUnpaused = 0;

    // Use this for initialization
    void Start () {

        playerRB = commonCanvasVariables.playerRB.GetComponent<Rigidbody>();
        vel = GetComponent<Text> ();

        if (playerRB == null)
        {throw new System.Exception("playerRB is set to null in script attached to " + this.gameObject.name);}

        timeUnpaused = timerController.StartTime;
    }
	
	// Update is called once per frame
	void LateUpdate () 
	{   
        lastVelocity = Mathf.Floor(playerRB.velocity.magnitude * 10000) / 10000;
        vel.text = (lastVelocity).ToString();
    }   
    

}
