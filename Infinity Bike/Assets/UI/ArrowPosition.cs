using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowPosition : MonoBehaviour {
    RectTransform rectTransform;

    public float height;
    public float width;
    public ArduinoThread arduinoThread;

    public List<Button> onScreenButtonList = new List<Button>();
    private float lastSpinValue = 0f;
    Button closestButton = null;


    public float delayTimer = 0.5f;
    private float timer = 0f;

    // Use this for initialization
    void Start () {
        rectTransform = GetComponent<RectTransform>();
        height = (float)Screen.height/2f;
        width = (float)Screen.width/2f;
        lastSpinValue = float.MaxValue;
    }   

    // Update is called once per frame
    void Update()
    {

        float val = arduinoThread.values.rotation / arduinoThread.arduinoAgent.rotationAnalogRange.range * Mathf.PI;// - Mathf.PI/2f;

        Vector3 position = (new Vector3(-width * Mathf.Cos(val), -height * (1 - Mathf.Sin(val)), 0));
        rectTransform.localPosition = position;
        rectTransform.rotation = Quaternion.Euler(0, 0, 270f - val * 180f / Mathf.PI);



        ActivateClosestButton();

    }


    void ActivateClosestButton()
    {
        if (timer < delayTimer)
        {
            if (lastSpinValue < arduinoThread.values.speed)
            {
                timer += Time.deltaTime;
                float distance = float.MaxValue;
                foreach (Button item in onScreenButtonList)
                {
                    float temp_distance = Vector3.Distance(item.GetComponent<RectTransform>().position, rectTransform.localPosition);
                    if (distance > temp_distance)
                    {
                        closestButton = item;
                        distance = temp_distance;
                    }
                }
            }
            else
            {   
                timer = 0;
                lastSpinValue = arduinoThread.values.speed;
                closestButton = null;
            }   
        }
        else
        {
            lastSpinValue = arduinoThread.values.speed;
            timer = 0;
            if(closestButton!=null)
            closestButton.onClick.Invoke();
        }


    }
    
}
