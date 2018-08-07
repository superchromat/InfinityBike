using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VelocityGetter : MonoBehaviour {

    public CommonCanvasVariables commonCanvasVariables;
	private Rigidbody playerRB;
	private Text vel;
	// Use this for initialization
	void Start () {

        playerRB = commonCanvasVariables.playerRB.GetComponent<Rigidbody>();
        vel = GetComponent<Text> ();

        if (playerRB == null)
        {throw new System.Exception("playerRB is set to null in script attached to " + this.gameObject.name);}

    }
	
	// Update is called once per frame
	void LateUpdate () 
	{
        vel.text = (Mathf.Floor(playerRB.velocity.magnitude * 10000) / 10000).ToString();
    }
}   
