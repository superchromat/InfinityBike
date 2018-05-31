using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArduinoDisplayUpdate : MonoBehaviour {


	public Text rotation = null;
	public Text speed = null;

	private ArduinoThread arduinoValue;


	// Use this for initialization
	void Start () {
		arduinoValue = GetComponent<ArduinoThread> ();
		
		
	}
	
	// Update is called once per frame
	void Update () {
		if(rotation != null)
		rotation.text = arduinoValue.values.rotation.ToString();
		if(speed != null)
		speed.text = arduinoValue.values.speed.ToString();
		
	}
}
