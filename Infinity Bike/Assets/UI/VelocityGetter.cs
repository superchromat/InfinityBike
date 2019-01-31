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
    private GraphSetter graphSetter;
    private float time;

    // Use this for initialization
    void Start () {

        playerRB = commonCanvasVariables.playerRB.GetComponent<Rigidbody>();
        vel = GetComponent<Text> ();
        graphSetter = GetComponent<GraphSetter>();

        if (playerRB == null)
        {throw new System.Exception("playerRB is set to null in script attached to " + this.gameObject.name);}
        time = 0; ;
    }
	
	// Update is called once per frame
	void LateUpdate () 
	{   
        lastVelocity = Mathf.Floor(playerRB.velocity.magnitude * 10000) / 10000;
        vel.text = (lastVelocity).ToString();
        graphSetter.AddToCurve(time, lastVelocity);
        time += Time.deltaTime;
    }   
    

}
