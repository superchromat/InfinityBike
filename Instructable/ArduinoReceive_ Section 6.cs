
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO.Ports;

using System.Threading;
public class ArduinoReceive : MonoBehaviour 
{	
	
	private SerialPort arduinoPort;
	private Thread activeThread = null;

	// Use this for initialization
	void Start () 
	{
		arduinoPort = new SerialPort("COM7", 9600);
		arduinoPort.Open ();
		arduinoPort.ReadTimeout = 100;

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (activeThread == null || !activeThread.IsAlive)
		{	
			activeThread = new Thread (AsynchronousReadFromArduino);
			activeThread.Start ();
		}	

	}


	private void AsynchronousReadFromArduino()
	{
		string rotString = null;
		string speedString = null;

		WriteToArduino("TRIGG");
		try {rotString = arduinoPort.ReadLine();}
		catch (TimeoutException) {}

		try {speedString = arduinoPort.ReadLine();}
		catch (TimeoutException) {}

		if (rotString != null && speedString != null)
		{
			OnArduinoInfoReceived(rotString, speedString);
		}
		else
		{
			OnArduinoInfoFail();
		}
	}

	private void WriteToArduino(string message)
	{
		if (arduinoPort.IsOpen) 
		{	
			message = message + "\r\n";
			arduinoPort.Write (message);
			arduinoPort.BaseStream.Flush ();
		}	 
		else 
		{	
			Debug.Log ("PortNotOpen");
		}	
	}
	void OnDestroy()
	{
		arduinoPort.Close();
	}

	private void OnArduinoInfoFail()
	{
		Debug.Log ("Reading failed");
	}

	private void OnArduinoInfoReceived(string rotation, string speed)
	{
		Debug.Log ("Readin Sucessfull");
		Debug.Log("First Value : " +rotation);
		Debug.Log("Second Value : " + speed);
	}


}
