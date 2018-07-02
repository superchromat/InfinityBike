using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySerialData : MonoBehaviour {

    // Use this for initialization
    public Text speedText;
    public Text rotationText;

    public ArduinoThread serialData;

	void Start ()
    {   
		
	}   
	
	// Update is called once per frame
	void Update ()
    {
        speedText.text = serialData.arduinoInfo.arduinoValueStorage.rawSpeed.ToString();
        rotationText.text = serialData.arduinoInfo.arduinoValueStorage.rawRotation.ToString();

    }   
}
