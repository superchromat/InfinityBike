
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
	public string comPort = "Code will select automatically.";
	public enum BaudRate{ _9600 = 9600,_14400= 14400} ;
	public BaudRate baudRate = BaudRate._9600;

	public int arduinoReadTimeout = 50;

	public ArduinoValueStorage values = new ArduinoValueStorage ();

	private Thread activeThread = null;
	private SerialPort arduinoPort = null;

	void Start () 
	{
		comPort = AutoDetectArduinoPort ();
	}	

	void Update () 
	{
        if (useArduinoPort) 
		{	
			ArduinoReadThread();
        }	
        else 
		{	
            values.speed = (UInt16) (Mathf.Abs(Input.GetAxis("Vertical")) *keyboardSpeed);
            values.rotation = (UInt16) ((keyboardAngle + Input.GetAxis("Horizontal")*keyboardMultiplier));
        }	
	}		

    private void ArduinoReadThread() 
	{	
		if (activeThread == null || !activeThread.IsAlive)
        {	
			activeThread = new Thread (()=>{AsynchronousReadFromArduino(OnArduinoInfoReceived);});
			activeThread.Start ();
        }	
    }	

	private void OnArduinoInfoReceived(string rotation, string speed)
	{values.SetValue (UInt16.Parse(rotation), UInt16.Parse(speed));}

	private void AsynchronousReadFromArduino(Action<string, string> callback)
	{			
		string rotString = null;
		string speedString = null;

		WriteToArduino ("ALL");

		try {rotString = arduinoPort.ReadLine();}
		catch (TimeoutException){}

		try {speedString = arduinoPort.ReadLine();}
		catch (TimeoutException){}
				
		if (rotString != null && speedString!=null) 
		{callback(rotString,speedString);}
		else
		{callback(null,null);}

	}	
    
    private void WriteToArduino(string message)
	{
		if (arduinoPort.IsOpen ) 
		{
			message = message + "\r\n";
			arduinoPort.Write (message);
		}
	}

    private string AutoDetectArduinoPort()
    {
        //Find ports list in Windows or Mac
        string[] ports;
        int p = (int)Environment.OSVersion.Platform;

        if (p == 4 || p == 128 || p == 6)
        {
            ports = GetPortNamesOSX();
        }
        else {
            ports = SerialPort.GetPortNames();
        }


        foreach (string port in ports) 
		{
            
			arduinoPort = new SerialPort(port, (int)baudRate);
			arduinoPort.ReadTimeout = arduinoReadTimeout;
            string result = null;
            try
            {
                arduinoPort.Open();
                if (arduinoPort.IsOpen)
                {

                    WriteToArduino("READY");

                    result = arduinoPort.ReadLine();

                    if (result == "READY")
                    {
                        return port;
                    }

                }

            }
            catch
            {
                arduinoPort.DiscardOutBuffer();
                arduinoPort.DiscardInBuffer();
                arduinoPort.Dispose();
            }

		}	
		return null;
	}





	[Serializable]
	public class ArduinoValueStorage
	{	
		public UInt16 rotation;
		public UInt16 speed;

		public void SetValue(UInt16 rotation,UInt16 speed)
		{
			this.rotation = rotation;
			this.speed = speed;
		}
		
		public ArduinoValueStorage()
		{SetValue(0,0);}

		public ArduinoValueStorage(UInt16 rotation,UInt16 speed)
		{SetValue(rotation,speed);}
	}	

	void OnDestroy()
	{
        if (useArduinoPort)
        {
            try
            {
                WriteToArduino("DONE");
                arduinoPort.Dispose();
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }
		
	}

	public struct ComPortList
	{	
		private static readonly string[] COMLIST = new string[]{"COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9","COM10","COM11","COM12","COM13","COM14","COM15","/dev/cu.wchusbserial1420"};

		public static string get(int comID)
		{return COMLIST [comID];}
	}	

    string[] GetPortNamesOSX() //Function retreived from : 
    //https://answers.unity.com/questions/643078/serialportsgetportnames-error.html
    {
       
        List<string> serial_ports = new List<string>();

      
        string[] ttys = System.IO.Directory.GetFiles("/dev/", "tty.*");
        foreach (string dev in ttys)
            {
                //if (dev.StartsWith()
                serial_ports.Add(dev);
                //Debug.Log(String.Format(dev));
            }

        return serial_ports.ToArray();
    }

}	
