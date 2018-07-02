using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArduinoButtonPress : MonoBehaviour {

    public Transform[] onScreenButtonParent;
    public List<Button> onScreenButton = new List<Button>();
    public Button closestButton = null;
    public ArduinoThread arduinoThread;
    public float delayTimer = 0.2f;
    private bool isCheckDone = true;
    public Color startColour = new Color(1, 0, 0, 1);
    public Color finalColor = new Color(0, 1, 0, 1);
    ColorBlock colorBlock;

    void Start () {
        colorBlock.highlightedColor = finalColor;
        colorBlock.normalColor = startColour;
        colorBlock.pressedColor = Color.white;
        colorBlock.disabledColor = Color.white;
        colorBlock.colorMultiplier = 1;

        if (onScreenButtonParent.Length > 0)
        foreach (Transform item in onScreenButtonParent)
        {
            Button[] butList = item.GetComponentsInChildren<Button>(true);
            foreach (Button button in butList)
            {
                button.colors = colorBlock;
                onScreenButton.Add(button);
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        StartCoroutine(SelectClosestButton());
        if (isCheckDone)
        {
            StartCoroutine(ActivateClosestButton());
        }

    }


    Button FindClosestActiveButton()
    {
        float distance = float.MaxValue;
        foreach (Button item in onScreenButton)
        {
            if (item.IsActive() == true)
            {
                float distanceContender = Vector3.Distance(item.transform.localPosition,  transform.localPosition);

                if (distanceContender < distance)
                {
                    distance = distanceContender;
                    closestButton = item;

                }
            }
        }

        return closestButton;
    }

    IEnumerator SelectClosestButton()
    {
        Button lastButton = closestButton;

        FindClosestActiveButton();

        if (lastButton != null)
        {
            lastButton.colors = colorBlock;
            lastButton = closestButton;

        }

        if (closestButton != null)
        {
            ColorBlock blk = closestButton.colors;
            blk.normalColor = colorBlock.highlightedColor;// Color.Lerp(closestButton.colors.normalColor, colorBlock.highlightedColor, Time.deltaTime*50);

            closestButton.colors = blk ;
        }

        yield return null;

    }

    IEnumerator ActivateClosestButton()
    {
        isCheckDone = false;

        float lastSpinValue = arduinoThread.arduinoInfo.arduinoValueStorage.rawSpeed;

        yield return new WaitForSeconds(delayTimer);

        if (arduinoThread.arduinoInfo.arduinoValueStorage.rawSpeed > lastSpinValue)
        {
            if (closestButton != null)
            { closestButton.onClick.Invoke(); }
        }

        isCheckDone = true;
    }
}
