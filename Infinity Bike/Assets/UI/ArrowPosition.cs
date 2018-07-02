using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowPosition : MonoBehaviour {
    RectTransform rectTransform;

    public float height;
    public float width;
    public ArduinoThread arduinoThread;

    public RectTransform[] onScreenButtonParent;
    public List<Button> onScreenButton;
    private Button closestButton = null;

    public float delayTimer = 0.2f;
    private bool isCheckDone = true;

    void Start () {
        rectTransform = GetComponent<RectTransform>();
        height = (float)Screen.height/2f;
        width = (float)Screen.width/2f;

        if(onScreenButtonParent.Length > 0)
        foreach (RectTransform item in onScreenButtonParent)
        {
            Button[] butList = item.GetComponentsInChildren<Button>(true);
            
            foreach (Button button in butList)
            {onScreenButton.Add(button);} 
        }

    }
    
    void Update()
    {   
        float val = arduinoThread.arduinoInfo.arduinoValueStorage.rawRotation / arduinoThread.arduinoInfo.rotationAnalogRange.range * Mathf.PI;

        Vector3 position = (new Vector3(-width * Mathf.Cos(val), -height * (1 - Mathf.Sin(val)), 0));
        rectTransform.localPosition = position;
        rectTransform.rotation = Quaternion.Euler(0, 0, 270f - val * 180f / Mathf.PI);

        FindClosestActiveButton();

        if (isCheckDone)
        {   
            StartCoroutine(ActivateClosestButton());
        }


        if (closestButton != null)
        { closestButton.Select(); }

    }   

    Button FindClosestActiveButton()
    {
        float distance = float.MaxValue;
        foreach (Button item in onScreenButton)
        {

            if (item.IsActive() == true)
            {

                float distanceContender = Vector3.Distance(item.GetComponent<RectTransform>().localPosition, rectTransform.localPosition);

                if (distanceContender < distance)
                {
                    distance = distanceContender;
                    closestButton = item;

                }
            }   
        }

        return closestButton;
    }   
    
    IEnumerator ActivateClosestButton()
    {
        isCheckDone = false;

        float lastSpinValue = arduinoThread.arduinoInfo.arduinoValueStorage.rawSpeed;

        yield return new WaitForSeconds(delayTimer);

        if (arduinoThread.arduinoInfo.arduinoValueStorage.rawSpeed > lastSpinValue)
        {   
            if (closestButton != null)
            {closestButton.onClick.Invoke();}
        }

        isCheckDone = true;
    }

}
