using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArduinoDisplayUpdate : MonoBehaviour {


	public Text rotation;
	public Text speed;

	private ArduinoThread arduinoValue;


	// Use this for initialization
	void Start () {
		arduinoValue = GetComponent<ArduinoThread> ();
		
		
	}
	
	// Update is called once per frame
	void Update () {

		rotation.text = arduinoValue.values.rotation.ToString();
		speed.text = arduinoValue.values.speed.ToString();
		
	}
}
