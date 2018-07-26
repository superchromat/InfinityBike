using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;

[Serializable]
public class ArduinoInfo
{
    public enum BaudRate { _9600 = 9600, _19200 = 19200, _38400 = 38400 , _57600 = 57600 , _74880 = 74880 , _115200 = 115200 ,_230400 = 230400,_250000 = 250000};
    
    public string comPort = "Code will select automatically.";
    public BaudRate baudRate = BaudRate._9600;
    public int arduinoReadTimeout = 50;
    
    public SerialPort arduinoPort = null;
    public AnalogRange rotationAnalogRange = new AnalogRange();
    public ArduinoValueStorage arduinoValueStorage = new ArduinoValueStorage();

    public void OpenArduinoPort(string port)
    {
        try { ArduinoAgentCleanup(); } catch (Exception) { }

        arduinoPort = new SerialPort(port, (int)baudRate);
        arduinoPort.ReadTimeout = arduinoReadTimeout;
        arduinoPort.WriteTimeout = arduinoReadTimeout;
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
            if (arduinoPort.IsOpen)
            {
                WriteToArduino("DONE");
                arduinoPort.BaseStream.Flush();
                arduinoPort.Dispose();
            }
        }
        catch/* (System.NullReferenceException e)*/
        {
            //    Debug.LogWarning("Handled Error -> " + e.GetType() + " : " + e.Message);
        }


    }

    [Serializable]
    public class AnalogRange
    {
        public float center;
        public float range;

        public AnalogRange(float center, float range)
        {
            this.center = center;
            this.range = range;
        }

        public AnalogRange()
        {
            this.center = 512;
            this.range = 1024;
        }
    }


    [Serializable]
    public class ArduinoValueStorage
    {

        public UInt16 rawRotation;
        public UInt16 rawSpeed;


        public void SetValue(UInt16 rawRotation, UInt16 rawSpeed)
        {
            this.rawRotation = rawRotation;
            this.rawSpeed = rawSpeed;
        }

        public ArduinoValueStorage()
        { SetValue(0, 0); }


    }


}
