
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;
using System.Threading;



[CreateAssetMenu(fileName = "ArduinoThread", menuName = "Generator/ArduinoThread", order = 2)]
public class ArduinoThread : ScriptableObject 
{   
    public Action CurrentActiveValueGetter = null;
    public ArduinoInfo arduinoInfo = new ArduinoInfo();
    private ArduinoInfo threadSafeArduinoInfo = new ArduinoInfo();
    Thread arduinoThread;

    public void Initialisation()
    {
       arduinoThread = (new Thread(AsychronousAutoDetectArduino));
        CurrentActiveValueGetter = () => {if(!arduinoThread.IsAlive) arduinoThread.Start(); };
        //CurrentActiveValueGetter= AsychronousAutoDetectArduino;

        //  ThreadPool.QueueUserWorkItem(AsychronousAutoDetectArduino);
    }
    public void RunThread()
    {

        
        {
            arduinoInfo.arduinoValueStorage.rawRotation = threadSafeArduinoInfo.arduinoValueStorage.rawRotation;
            arduinoInfo.arduinoValueStorage.rawSpeed = threadSafeArduinoInfo.arduinoValueStorage.rawSpeed;

            CurrentActiveValueGetter();

        }

    }

	private void AsynchronousReadFromArduino()
	{
        try
        {
            arduinoInfo.WriteToArduino("ALL");
        }
        catch (System.IO.IOException e)
        {
            Debug.LogWarning("Handled Error -> " + e.GetType() + " : " + e.Message);
            arduinoThread = (new Thread(AsychronousAutoDetectArduino));

            //CurrentActiveValueGetter = AsychronousAutoDetectArduino;
            return;
        }

        string rotString = null;
        string speedString = null;
        if (!arduinoInfo.arduinoPort.IsOpen)
        {
            //   ThreadPool.QueueUserWorkItem(new WaitCallback(AsychronousAutoDetectArduino));
            arduinoThread = (new Thread(AsychronousAutoDetectArduino));
         //   CurrentActiveValueGetter = AsychronousAutoDetectArduino;
            return;
        }

        try
        {
            rotString = arduinoInfo.arduinoPort.ReadLine();
            speedString = arduinoInfo.arduinoPort.ReadLine();

            threadSafeArduinoInfo.arduinoValueStorage.SetValue(UInt16.Parse(rotString), UInt16.Parse(speedString));

            //    ThreadPool.QueueUserWorkItem(new WaitCallback(AsynchronousReadFromArduino));
                arduinoThread = (new Thread(AsynchronousReadFromArduino));
        }
        catch (TimeoutException e)
        {
            Debug.LogWarning("Handled Error -> " + e.GetType() + " : " + e.Message);
            rotString = null;
            speedString = null;
            //   ThreadPool.QueueUserWorkItem(new WaitCallback(AsychronousAutoDetectArduino));
               arduinoThread = (new Thread(AsychronousAutoDetectArduino));
            //CurrentActiveValueGetter = AsychronousAutoDetectArduino;
        }
        catch (System.IO.IOException e)
        {
            Debug.LogWarning("Handled Error -> " + e.GetType() + " : " + e.Message);

            //   ThreadPool.QueueUserWorkItem(new WaitCallback(AsychronousAutoDetectArduino));
              arduinoThread = (new Thread(AsychronousAutoDetectArduino));
           // CurrentActiveValueGetter = AsychronousAutoDetectArduino;
        }
    }   

    private void AsychronousAutoDetectArduino()
    {
        arduinoInfo.comPort = "";
        arduinoInfo.comPort = AutoDetectArduinoPort();
    }   
    
    private string AutoDetectArduinoPort()
    {
        Debug.Log("started auto Dectect");
        try
        {
            if (arduinoInfo.arduinoPort.IsOpen)
            { arduinoInfo.arduinoPort.Dispose(); }
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

            arduinoInfo.OpenArduinoPort(port);

            if (arduinoInfo.arduinoPort.IsOpen)
            {   
                const int tryCount = 10;
                for (int i = 0; i < tryCount; i++)
                {
                    try
                    {   
                        arduinoInfo.WriteToArduino("READY");
                        Thread.Sleep(50);
                        result = arduinoInfo.arduinoPort.ReadLine();
                    }   
                    catch (TimeoutException)
                    {
                        result = null;
                    }
                        
                    if (result == "READY")
                    {
                        Debug.Log(port + " " + result);
                      //  ThreadPool.QueueUserWorkItem(new WaitCallback(AsynchronousReadFromArduino));
                        arduinoThread = (new Thread(AsynchronousReadFromArduino));
                        //CurrentActiveValueGetter = AsynchronousReadFromArduino;

                        return port;
                    }
                }
            }   
        }
       // ThreadPool.QueueUserWorkItem(new WaitCallback(AsychronousAutoDetectArduino));
        arduinoThread = (new Thread(AsychronousAutoDetectArduino));
        //                CurrentActiveValueGetter = AsychronousAutoDetectArduino;
        return null;
    }   
    
    string[] GetPortNamesOSX() //Function retreived from : https://answers.unity.com/questions/643078/serialportsgetportnames-error.html
    {
       
        List<string> serial_ports = new List<string>();

      
        string[] ttys = System.IO.Directory.GetFiles("/dev/", "tty.*");
        foreach (string dev in ttys)
        {   
            serial_ports.Add(dev);
        }   

        return serial_ports.ToArray();
    }

}   
