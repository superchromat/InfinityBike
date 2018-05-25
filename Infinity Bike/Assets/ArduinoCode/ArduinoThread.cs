
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;

using System.Threading;

public class ArduinoThread : MonoBehaviour 
{
    public bool useArduinoPort = true;
    public float keyboardSpeed = 100 ;
    public float keyboardAngle = 550;
    public float keyboardMultiplier = 100; 


	// Use this for initialization
	public enum COM{ COM1 = 1,COM2 = 2,COM3 = 3,COM4 = 4,COM5 = 5 , COM6 = 6 , COM7 = 7,COM8 = 8, COM9 = 9,COM10 = 10,COM11 = 11, COM12 = 12, COM13 = 13, COM14 = 14, COM15 = 15, macPort = 16} ;
	public COM comPort;

	public enum BaudRate{ _9600 = 9600,_14400= 14400} ;
	public BaudRate baudRate;

	public int arduinoReadTimeout = 1;

	public ArduinoValueStorage values;
	private Thread activeThread;

	private SerialPort arduinoPort;


	void Start () 
	{	
        if (useArduinoPort) 
		{	
            if (SystemInfo.operatingSystemFamily.ToString() == "Windows" && !((int)comPort < (int)COM.macPort))
            { throw new ArgumentException("Wrong OS"); }
            
			arduinoPort = new SerialPort(ComPortList.get((int)comPort - 1), 9600);
			values = new ArduinoValueStorage();

			arduinoPort.Open ();
        }	
		arduinoPort.ReadTimeout = (int)arduinoReadTimeout;

	}	

	void Update () {
        if (useArduinoPort) 
		{	
            ArduinoCommandQueueUpdate();
        }	
        else 
		{	
            values.speed = (UInt16) (Mathf.Abs(Input.GetAxis("Vertical")) *keyboardSpeed);
            values.rotation = (UInt16) ((keyboardAngle + Input.GetAxis("Horizontal")*keyboardMultiplier));
        }	
        

	}

    private void ArduinoCommandQueueUpdate() 
	{	
		if (activeThread == null || !activeThread.IsAlive)
        {	
			
			activeThread = new Thread (()=>{AsynchronousReadFromArduino(OnArduinoInfoReceived,OnArduinoInfoFail);});
			activeThread.Start ();
        }	
    }	

	private void OnArduinoInfoReceived(string rotation, string speed)
	{
		values.rotation = UInt16.Parse (rotation);
		values.speed = UInt16.Parse (speed);
	}

	private void OnArduinoInfoFail()
	{
		values.rotation = 1000;
		values.speed = 1000;
	}

	private void AsynchronousReadFromArduino(Action<string, string> success , Action fail)
	{			
		string rotString = null;
		string speedString = null;

		WriteToArduino ("ALL");
		try 
		{
			rotString = arduinoPort.ReadLine();
		} 
		catch (TimeoutException){}

		try 
		{
			speedString = arduinoPort.ReadLine();
		} 
		catch (TimeoutException){}
				
		if (rotString != null && speedString!=null) 
		{
			success(rotString,speedString);
		}
		else
		{
			fail ();
		}

	}
    
    private void WriteToArduino(string message)
	{
		if (arduinoPort.IsOpen ) 
		{
			message = message + "\r\n";
			arduinoPort.Write (message);
			arduinoPort.BaseStream.Flush ();
		}


	}

	[Serializable]
	public class ArduinoValueStorage
	{	
		public UInt16 rotation;
		public UInt16 speed;
		public ArduinoValueStorage()
		{
			rotation = 0;
			speed = 0;
		}

		public ArduinoValueStorage(		
			UInt16 rotation,
			UInt16 speed)
		{
			this.rotation = rotation;
			this.speed = speed;
		}
	}	

	void OnDestroy()
	{
        if (useArduinoPort){
            WriteToArduino("DONE");
            arduinoPort.Close();
        }
		
	}

	public struct ComPortList
	{	
		private static readonly string[] COMLIST = new string[]{"COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9","COM10","COM11","COM12","COM13","COM14","COM15","/dev/cu.wchusbserial1420"};

		public static string get(int comID)
		{return COMLIST [comID];}
	}	

}	
