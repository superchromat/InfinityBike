using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerPoints : MonoBehaviour {

	public Text pointText;
	public int points = 0;

	// Use this for initialization
	void Start () 
	{	
		if (pointText == null)
			this.enabled = false;
		points = 0;
	}	
	
	// Update is called once per frame
	void LateUpdate () 
	{	
		
		pointText.text = points.ToString ();
	}	

	public void IncrementPoints()
	{
		points++;

	}


}
