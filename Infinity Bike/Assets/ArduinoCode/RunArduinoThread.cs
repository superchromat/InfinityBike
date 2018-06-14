using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunArduinoThread : MonoBehaviour {
    public ArduinoThread arduinoThread;

    // Use this for initialization
    void Start () {
        if (arduinoThread.arduinoAgent.arduinoPort == null || !arduinoThread.arduinoAgent.arduinoPort.IsOpen)
        {
            arduinoThread.InitiateInitialisation();
            arduinoThread.CurrentActiveValueGetter();
        }
    }

    private void FixedUpdate()
    {
        arduinoThread.CurrentActiveValueGetter();
    }

    void OnApplicationQuit()
    {
        arduinoThread.arduinoAgent.ArduinoAgentCleanup();
    }
}
