using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunArduinoThread : MonoBehaviour {
    public ArduinoThread arduinoThread;
    public bool useKeyBoard = false;
    public ushort speed = 80;
    public ushort rotation = 200;

    // Use this for initialization
    void Start ()
    {   
        if (arduinoThread.arduinoInfo.arduinoPort == null || !arduinoThread.arduinoInfo.arduinoPort.IsOpen)
        {
            arduinoThread.Initialisation();
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            arduinoThread.arduinoInfo.arduinoValueStorage.rawSpeed = speed;
        else
            arduinoThread.arduinoInfo.arduinoValueStorage.rawSpeed = 0;

        if (Input.GetKey(KeyCode.A))
            arduinoThread.arduinoInfo.arduinoValueStorage.rawRotation =(ushort)(512 - (int)rotation);

        if (Input.GetKey(KeyCode.D))
            arduinoThread.arduinoInfo.arduinoValueStorage.rawRotation = (ushort)(512 + (int)rotation);

        if (!(Input.GetKey(KeyCode.A) ^ Input.GetKey(KeyCode.D)))
        { arduinoThread.arduinoInfo.arduinoValueStorage.rawRotation = 512; }



    }

    void OnApplicationQuit()
    {
        arduinoThread.arduinoInfo.ArduinoAgentCleanup();
    }
}
