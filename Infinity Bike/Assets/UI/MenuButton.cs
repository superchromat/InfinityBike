using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public Slider slider;

    public enum Menu
    {
        STARTSCREEN = 0, OPTIONSCEEN = 1 , GAMESCREEN = 2
    }
    public Transform[] onScreenButtonGroup;
    private int activeGroup = -1;
    public int CurrentActivatedID
    {
        get { return activeGroup; }
    }

    public void ActivateMenuButtons(int id)
    {
        foreach (Transform item in onScreenButtonGroup)
        { item.gameObject.SetActive(false); }

        onScreenButtonGroup[id].gameObject.SetActive(true);
        activeGroup = id;
    }   

    public void DeactivateMenuButtons(int id)
    {
        foreach (Transform item in onScreenButtonGroup)
        { item.gameObject.SetActive(false); }
        activeGroup = -1;
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
                slider.value = progess;
            yield return null;
        }


    }



}
