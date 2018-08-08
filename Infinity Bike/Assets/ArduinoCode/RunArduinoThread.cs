using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunArduinoThread : MonoBehaviour
{   

    public ArduinoThread arduinoThread;
    public bool useKeyBoard = false;
    public ushort keyBoardSpeed = 80;
    public ushort keyBoardRotation = 200;
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

        if (useKeyBoard || !arduinoThread.IsArduinoConnected)
        {   
            arduinoThread.CurrentActiveValueGetter = null;
            if (Input.GetKey(KeyCode.Space))
                arduinoThread.arduinoInfo.arduinoValueStorage.rawSpeed = keyBoardSpeed;
            else
                arduinoThread.arduinoInfo.arduinoValueStorage.rawSpeed = 0;
            float val = 0;
            if (Input.GetKey(KeyCode.A))
                val = (ushort)(512 - (int)keyBoardRotation);

            if (Input.GetKey(KeyCode.D))
                val = (ushort)(512 + (int)keyBoardRotation);

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
