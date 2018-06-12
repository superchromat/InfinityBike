
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
    Action CurrentActiveValueGetter = null;

	private Thread activeThread = null;
	private SerialPort arduinoPort = null;

	void Start () 
	{
        ResetCurrentActiveValueGetter();
    }

    void Update () 
	{
        
        CurrentActiveValueGetter();
	}

    private void ResetCurrentActiveValueGetter()
    {
        if (useArduinoPort)
        {
            activeThread = new Thread(() => { AsychronousAutoDetectArduino(); });
            CurrentActiveValueGetter = ArduinoReadThread;
        }
        else
        {
            CurrentActiveValueGetter = SynchronousReadFromKeyBoard;
        }
    }

    private void ArduinoReadThread() 
	{	
        if (!activeThread.IsAlive)
        {
            try
            {
                activeThread.Start();
            }
            catch(ThreadStateException e)
            {
                Debug.LogError("Handled Error -> " + e.GetType()+ " : "+ e.Message);
                ResetCurrentActiveValueGetter();

            }
        }
    }   
    private void SynchronousReadFromKeyBoard()
    {
        values.speed = (UInt16)(Mathf.Abs(Input.GetAxis("Vertical")) * keyboardSpeed);
        values.rotation = (UInt16)((keyboardAngle + Input.GetAxis("Horizontal") * keyboardMultiplier));
    }

	private void AsynchronousReadFromArduino()
	{   
        WriteToArduino("ALL");
        string rotString = null;
        string speedString = null;

        try
        {
            rotString = arduinoPort.ReadLine();
            speedString = arduinoPort.ReadLine();
            values.SetValue(UInt16.Parse(rotString), UInt16.Parse(speedString));
            activeThread = new Thread(() => { AsynchronousReadFromArduino(); });
        }
        catch (TimeoutException e)
        {
            Debug.LogError("Handled Error -> " + e.GetType() + " : " + e.Message);
            rotString = null;
            speedString = null;
        }
        catch (InvalidOperationException e)
        {
            Debug.LogError("Handled Error -> " + e.GetType() + " : " + e.Message);
            activeThread = new Thread(() => { AsychronousAutoDetectArduino(); });
        }


    }
    private void AsychronousAutoDetectArduino()
    {
        comPort = AutoDetectArduinoPort();
    }
    
    private void WriteToArduino(string message)
	{
        if (arduinoPort.IsOpen)
        {
            message = message + "\r\n";
            arduinoPort.Write(message);
            arduinoPort.BaseStream.Flush();
        }
	}

    private string AutoDetectArduinoPort()
    {
        Debug.Log("autoDectect");
        try
        {
            if (arduinoPort.IsOpen)
            { arduinoPort.Dispose(); }
        }
        catch (NullReferenceException) { }
        
        //Find ports list in Windows or Mac
        string[] ports;
        int p = (int)Environment.OSVersion.Platform;
        if (p == 4 || p == 128 || p == 6)
        {ports = GetPortNamesOSX();}
        else
        {ports = SerialPort.GetPortNames();}

        foreach (string port in ports) 
		{
            string result = null;
            
            arduinoPort = new SerialPort(port, (int)baudRate);
            arduinoPort.ReadTimeout = arduinoReadTimeout;
            arduinoPort.Open();                   

            if (arduinoPort.IsOpen)
            {   
                const int tryCount = 10;
                for (int i = 0; i < tryCount; i++)
                {
                    try
                    {   
                        WriteToArduino("READY");
                        Thread.Sleep(50);
                        result = arduinoPort.ReadLine();
                    }   
                    catch (TimeoutException)
                    {
                        result = null;
                    }
                        
                    if (result == "READY")
                    {
                        Debug.Log(port + " " + result);
                        activeThread = new Thread(() => { AsynchronousReadFromArduino(); });
                        return port;
                    }
                }
            }   
        }
        activeThread = new Thread(() => { AsychronousAutoDetectArduino(); });
        return null;
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


    [Serializable]
    public class ArduinoValueStorage
    {
        public UInt16 rotation;
        public UInt16 speed;

        public void SetValue(UInt16 rotation, UInt16 speed)
        {
            this.rotation = rotation;
            this.speed = speed;
        }

        public ArduinoValueStorage()
        { SetValue(0, 0); }

        public ArduinoValueStorage(UInt16 rotation, UInt16 speed)
        { SetValue(rotation, speed); }
    }
}	
