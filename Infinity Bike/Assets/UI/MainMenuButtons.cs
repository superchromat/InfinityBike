using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour {

    public void ExitGame()
    {

    Debug.Log(GameObject.FindObjectsOfType(typeof(ArduinoThread)).Length);

        try
        {
            Application.Quit();
        }
        catch
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }


    public void OpenOptionMenue()
    {
        Debug.Log("Opened option menu");
    }





}
