using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsManager : MonoBehaviour {
    //public OctopusController octopusController; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.C)&& Input.GetKey(KeyCode.T)){
        //    octopusController.ActivateOctopus(); 
        }
	}
}
