
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


    public void Initialisation()
    {
        ThreadPool.QueueUserWorkItem(AsychronousAutoDetectArduino);
    }   

	private void AsynchronousReadFromArduino(object data)
	{   
        arduinoInfo.WriteToArduino("ALL");
        string rotString = null;
        string speedString = null;

        try
        {
            rotString = arduinoInfo.arduinoPort.ReadLine();
            speedString = arduinoInfo.arduinoPort.ReadLine();
            arduinoInfo.arduinoValueStorage.SetValue(UInt16.Parse(rotString), UInt16.Parse(speedString));

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsynchronousReadFromArduino));
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

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsychronousAutoDetectArduino));
        }
    }   

    private void AsychronousAutoDetectArduino(object data)
    {   
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
                        ThreadPool.QueueUserWorkItem(new WaitCallback(AsynchronousReadFromArduino));

                        return port;
                    }
                }
            }   
        }
        ThreadPool.QueueUserWorkItem(new WaitCallback(AsychronousAutoDetectArduino));
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
