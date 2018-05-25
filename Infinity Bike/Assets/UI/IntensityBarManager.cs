using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntensityBarManager : MonoBehaviour {
    public ArduinoThread storageValue;
    public Image[] bars;
    public int maxSpeed = 100; 


    private int numBar;
    private int speed;
public  Color[] colors;

	// Use this for initialization
	void Start () {
        numBar = bars.Length;

       colors = new Color[numBar];
        for (int i = 0; i < numBar; i++)
        {
            Image bar = bars[i];
            colors[i] = bar.color;

            //Debug.Log(bar.color);
            //bar.color = new Color(colors[barIndex].r, colors[barIndex].g, colors[barIndex].b, 0.1f);
           
        }
	}
	
	// Update is called once per frame
	void Update () {
        speed = storageValue.values.speed;



        for (int i = 0; i < numBar; i++)
        {
            Image bar = bars[i];
            float alpha = 0.1f;
            //float ratio = (float)speed / (float)maxSpeed;
            //Debug.Log(ratio);   
            if ((float) speed /(float) maxSpeed > (float)(i + 1) /(float) numBar){
                alpha = 1f; 
            }



            bar.color = new Color(colors[i].r,colors[i].g, colors[i].b, alpha);

        }
		
	}
}
