
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO.Ports;

using System.Threading;
public class ArduinoReceive : MonoBehaviour
{

    private SerialPort arduinoPort;
    // Use this for initialization
    void Start()
    {
        arduinoPort = new SerialPort("COM5", 9600);
        arduinoPort.Open();
    }

    // Update is called once per frame
    void Update()
    {
        WriteToArduino("TRIGG");

        Debug.Log("First Value : " + arduinoPort.ReadLine());
        Debug.Log("Second Value : " + arduinoPort.ReadLine());
    }


    private void WriteToArduino(string message)
    {
        if (arduinoPort.IsOpen)
        {
            message = message + "\r\n";
            arduinoPort.Write(message);
            arduinoPort.BaseStream.Flush();
        }
        else
        {
            Debug.Log("PortNotOpen");
        }
    }

}
