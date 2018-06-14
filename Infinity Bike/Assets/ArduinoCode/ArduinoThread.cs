
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;


using System.Threading;

[CreateAssetMenu(fileName = "ArduinoThread", menuName = "Generator/ArduinoThread", order = 2)]
public class ArduinoThread : ScriptableObject 
{
    public bool useArduinoPort = true;
    public Action CurrentActiveValueGetter = null;

    public ArduinoAgent arduinoAgent = new ArduinoAgent();
    public ArduinoValueStorage values = new ArduinoValueStorage ();
    public KeboardAlternative keboardAlternative = new KeboardAlternative(1,550,100);
    
    private Thread activeThread = null;
    
    public void InitiateInitialisation()
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
                InitiateInitialisation();

            }
        }
    } 
    
    private void SynchronousReadFromKeyBoard()
    {
        values.speed = (UInt16)(Mathf.Abs(Input.GetAxis("Vertical")) * keboardAlternative.keyboardSpeed);
        values.rotation = (UInt16)((keboardAlternative.keyboardAngle + Input.GetAxis("Horizontal") * keboardAlternative.keyboardMultiplier));
    }
	private void AsynchronousReadFromArduino()
	{   
        arduinoAgent.WriteToArduino("ALL");
        string rotString = null;
        string speedString = null;

        try
        {
            rotString = arduinoAgent.arduinoPort.ReadLine();
            speedString = arduinoAgent.arduinoPort.ReadLine();
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
        arduinoAgent.comPort = AutoDetectArduinoPort();
    }
    
    private string AutoDetectArduinoPort()
    {
        Debug.Log("autoDectect");
        try
        {
            if (arduinoAgent.arduinoPort.IsOpen)
            { arduinoAgent.arduinoPort.Dispose(); }
        }
        catch (NullReferenceException) {}
        
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

            arduinoAgent.openArduinoPort(port);

            if (arduinoAgent.arduinoPort.IsOpen)
            {   
                const int tryCount = 10;
                for (int i = 0; i < tryCount; i++)
                {
                    try
                    {   
                        arduinoAgent.WriteToArduino("READY");
                        Thread.Sleep(50);
                        result = arduinoAgent.arduinoPort.ReadLine();
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
    


    string[] GetPortNamesOSX() //Function retreived from : 
    //https://answers.unity.com/questions/643078/serialportsgetportnames-error.html
    {
       
        List<string> serial_ports = new List<string>();

      
        string[] ttys = System.IO.Directory.GetFiles("/dev/", "tty.*");
        foreach (string dev in ttys)
        {
            serial_ports.Add(dev);
        }

        return serial_ports.ToArray();
    }


    [Serializable]
    public struct KeboardAlternative
    {
        public float keyboardSpeed;
        public float keyboardAngle;
        public float keyboardMultiplier;

        public KeboardAlternative(float KeyboardSpeed, float KeyboardAngle, float KeyboardMultiplier)
        {
            keyboardSpeed = KeyboardSpeed;
            keyboardAngle = KeyboardAngle;
            keyboardMultiplier = KeyboardMultiplier;
        }


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
