using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;

[Serializable]
public class ArduinoAgent
{
    public string comPort = "Code will select automatically.";
    public BaudRate baudRate = BaudRate._9600;
    public int arduinoReadTimeout = 50;

    public SerialPort arduinoPort = null;
    
    public enum BaudRate { _9600 = 9600, _14400 = 14400 };

    public void openArduinoPort(string port)
    {   
        arduinoPort = new SerialPort(port, (int)baudRate);
        arduinoPort.ReadTimeout = arduinoReadTimeout;
        arduinoPort.Open();
    }

    public void WriteToArduino(string message)
    {
        if (arduinoPort.IsOpen)
        {
            message = message + "\r\n";
            arduinoPort.Write(message);
            arduinoPort.BaseStream.Flush();
        }
    }

    public void ArduinoAgentCleanup()
    {
        try
        {
            WriteToArduino("DONE");
            arduinoPort.BaseStream.Flush();
            arduinoPort.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogError("Unhandled Error -> " + e.GetType() + " : " + e.Message);
        }


    }
    



}
