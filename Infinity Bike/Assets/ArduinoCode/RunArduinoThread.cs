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
        if (useKeyBoard)
        {
            Debug.Log(Input.GetKey(KeyCode.Space));
            arduinoThread.CurrentActiveValueGetter = null;
            if (Input.GetKey(KeyCode.Space))
                arduinoThread.arduinoInfo.arduinoValueStorage.rawSpeed = speed;
            else
                arduinoThread.arduinoInfo.arduinoValueStorage.rawSpeed = 0;
            float val = 0;
            if (Input.GetKey(KeyCode.A))
                val = (ushort)(512 - (int)rotation);

            if (Input.GetKey(KeyCode.D))
                val = (ushort)(512 + (int)rotation);

            if (!(Input.GetKey(KeyCode.A) ^ Input.GetKey(KeyCode.D)))
            { val = 512; }
            arduinoThread.arduinoInfo.arduinoValueStorage.rawRotation = (ushort)Mathf.Lerp((float)(arduinoThread.arduinoInfo.arduinoValueStorage.rawRotation), val, 5.0f * Time.deltaTime);

        }
        else
        {
            arduinoThread.RunThread();
        }   

    }

    void OnApplicationQuit()
    {
        arduinoThread.arduinoInfo.ArduinoAgentCleanup();
    }
}
