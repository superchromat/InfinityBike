using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowPosition : MonoBehaviour {
    RectTransform rectTransform;

    public ArduinoThread arduinoThread;
    public float radius = 0.25f;
    private FormatUIAnchor fUIA;

    void Start ()
    {   
        rectTransform = GetComponent<RectTransform>();
        fUIA = GetComponent<FormatUIAnchor>();
    }   
    
    void Update()
    {   
        float val = arduinoThread.arduinoInfo.arduinoValueStorage.rawRotation / arduinoThread.arduinoInfo.rotationAnalogRange.range * Mathf.PI;
        Vector3 position = new Vector3(-(Mathf.Cos(val))*radius + 0.5f, Mathf.Sin(val)*radius , 0);

        if(fUIA!= null)
        fUIA.centerPosition = position;


     //   rectTransform.position = position;
       rectTransform.rotation = Quaternion.Euler(0, 0, 270f - val * 180f / Mathf.PI);

    }   
 
}
