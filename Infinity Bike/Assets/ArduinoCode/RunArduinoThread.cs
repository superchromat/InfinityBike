using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunArduinoThread : MonoBehaviour {
    public ArduinoThread arduinoThread;

    // Use this for initialization
    void Start ()
    {   
        if (arduinoThread.arduinoInfo.arduinoPort == null || !arduinoThread.arduinoInfo.arduinoPort.IsOpen)
        {
            arduinoThread.Initialisation();
        }
    }   
    
    void OnApplicationQuit()
    {
        arduinoThread.arduinoInfo.ArduinoAgentCleanup();
    }
}
