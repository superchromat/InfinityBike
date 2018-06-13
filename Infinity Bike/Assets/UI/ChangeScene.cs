using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

    public ArduinoThread startThread;
    public void LoadScene()
    {
        SceneManager.LoadScene(0);

    }

    private void Start()
    {

        if (startThread.arduinoAgent.arduinoPort == null || !startThread.arduinoAgent.arduinoPort.IsOpen)
        {
            startThread.InitiateInitialisation();
            startThread.CurrentActiveValueGetter();
        }
       
    }

    private void FixedUpdate()
    {
        startThread.CurrentActiveValueGetter();
    }

    void OnApplicationQuit()
    {
        startThread.arduinoAgent.ArduinoAgentCleanup();
    }

}
