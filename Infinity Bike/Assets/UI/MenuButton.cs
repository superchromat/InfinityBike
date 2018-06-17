using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public enum Menu
    {
        STARTSCREEN = 0, OPTIONSCEEN = 1
    }
    public List<MenueObjects> optionButtons = new List<MenueObjects>();

    public void ToggleMenuButtons(int id)
    {
        foreach (GameObject item in optionButtons[id].menuObject)
        {
            item.SetActive(!item.activeSelf);
        }   
    }

    
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

    public void LoadScene(int scene)
    {

        SceneManager.LoadScene(scene);
    }

    [Serializable]
    public class MenueObjects
    {
        public string menuName;
        public List<GameObject> menuObject = new List<GameObject>();   

    }

}
