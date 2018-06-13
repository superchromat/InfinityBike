using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VelocityGetter : MonoBehaviour {

	public Rigidbody player;
	private Text vel;
	// Use this for initialization
	void Start () {
        if (player== null)
        {
            this.enabled = false;
        }

        vel = GetComponent<Text> ();

       
		
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		
		vel.text =( Mathf.Floor( player.velocity.magnitude * 10000)/10000) .ToString();
	}
}
