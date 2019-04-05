
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;
using System.Threading;



[CreateAssetMenu(fileName = "ArduinoThread", menuName = "Generator/ArduinoThread", order = 2)]
public class ArduinoThread : ScriptableObject
{
    bool isArduinoConnected = false;
    public bool IsArduinoConnected
    {get {return isArduinoConnected;}}   

    public ArduinoInfo arduinoInfo = new ArduinoInfo();
    private Thread activeThread;
    private Queue<Thread> threadQueue = new Queue<Thread>();

    public void Initialisation()
    {
        if (activeThread != null && activeThread.IsAlive)
        { activeThread.Abort();}

        threadQueue.Enqueue((new Thread(AsychronousAutoDetectArduino)));
    }

    void AddToThreadQueue(Thread toAdd)
    {
        if (threadQueue.Count < 10)
        {
            threadQueue.Enqueue(toAdd);
        }
        else
        {   
            Debug.Log("Max thread reached in thread queue.");
            Debug.Log("In " + this.name);
        }

    }   

    public void RunThread()
    {   
        if (threadQueue.Count > 0  && (activeThread==null ||!activeThread.IsAlive))
        {
            try
            {
                activeThread = threadQueue.Dequeue();
                activeThread.Start();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
    private void AsynchronousReadFromArduino()
	{   

        if (!arduinoInfo.arduinoPort.IsOpen)
        {
            AddToThreadQueue(new Thread(AsychronousAutoDetectArduino));
            return;
        }   

        try
        {   
            arduinoInfo.WriteToArduino("ALL");
        }   
        catch (System.IO.IOException e)
        {   
            Debug.LogWarning("Handled Error -> " + e.GetType() + " : " + e.Message);
            AddToThreadQueue(new Thread(AsychronousAutoDetectArduino));
            return;
        }   

        string rotString = null;
        string speedString = null;

        try
        {

            rotString = arduinoInfo.arduinoPort.ReadLine();
            speedString = arduinoInfo.arduinoPort.ReadLine();

            arduinoInfo.arduinoValueStorage.SetValue(UInt16.Parse(rotString), UInt16.Parse(speedString));

            AddToThreadQueue(new Thread(AsynchronousReadFromArduino));

        }
        catch (TimeoutException e)
        {
            Debug.LogWarning("Handled Error -> " + e.GetType() + " : " + e.Message);
            arduinoInfo.arduinoValueStorage.SetValue(0, 0);
            AddToThreadQueue(new Thread(AsynchronousReadFromArduino));
        }
        catch (OverflowException e)
        {
            Debug.LogWarning("Handled Error -> " + e.GetType() + " : " + e.Message);
            arduinoInfo.arduinoValueStorage.SetValue(0, 0);
            AddToThreadQueue(new Thread(AsynchronousReadFromArduino));

        }
        catch (System.IO.IOException e)
        {
            arduinoInfo.arduinoValueStorage.SetValue(0, 0);
            Debug.LogWarning("Handled Error -> " + e.GetType() + " : " + e.Message);
            AddToThreadQueue(new Thread(AsychronousAutoDetectArduino));

        }
    }   

    private void AsychronousAutoDetectArduino()
    {   
        arduinoInfo.comPort = AutoDetectArduinoPort();
    }   
    
    private string AutoDetectArduinoPort()
    {
        isArduinoConnected = false;
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

        foreach (string port in ports) {   
            
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
                        AddToThreadQueue(new Thread(AsynchronousReadFromArduino));
                        isArduinoConnected = true;
                        return port;
                    }
                }
            }   
        }

        Debug.Log("Auto detect failed");

        AddToThreadQueue(new Thread(AsychronousAutoDetectArduino));
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
