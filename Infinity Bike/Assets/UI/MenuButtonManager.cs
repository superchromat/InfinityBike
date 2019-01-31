using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonManager : MonoBehaviour
{

    public enum Menu{   DEFAULT = -1, PAUSESCREEN = 0, LOADSCREEN = 1  }

    public Slider slider;
    public Transform[] onScreenButtonGroup;

    private Menu activeGroup = Menu.PAUSESCREEN;
    public Menu CurrentActivatedID
    {
        get {
            return activeGroup;
        }
    }




    public void ActivePauseScreen()
    {// for unity buttons
        ActivateMenuButtons(Menu.PAUSESCREEN);
    }

    public void ActivateMenuButtons(int id)
    {   
        // this exist for the unity onClick() event on the start race button.
        ActivateMenuButtons((Menu)id);
    }   
    
    public void ActivateMenuButtons(Menu id)
    {   
        foreach (Transform item in onScreenButtonGroup)
        { item.gameObject.SetActive(false); }
        
        onScreenButtonGroup[(int)id].gameObject.SetActive(true);
        activeGroup = id;
    }
    
    public void DeactivateMenuButtons()
    {   
        foreach (Transform item in onScreenButtonGroup)
        { item.gameObject.SetActive(false); }
        activeGroup = Menu.DEFAULT;
    }   
        
    public void ExitGame()
    {   
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }   
        
    public void LoadScene(int sceneIndex)
    {   
        StartCoroutine(LoadAsynch(sceneIndex));
    }   

    IEnumerator LoadAsynch(int sceneIndex)
    {   
        AsyncOperation aOpr = SceneManager.LoadSceneAsync(sceneIndex);
        while (!aOpr.isDone)
        {
            float progess = Mathf.Clamp01(aOpr.progress / 0.9f);
            if (slider != null)
            { slider.value = progess; }
            yield return null;
        }
    }   
        
}       
