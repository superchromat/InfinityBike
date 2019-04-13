using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NotificationsManager : MonoBehaviour {

    TextMeshProUGUI tMPro;
    public float persistance;
    private float lastMessageTime;
    bool textVisible = false; 

	// Use this for initialization
	void Start () {
        tMPro = GetComponent<TextMeshProUGUI>();
    
		
	}
	
	// Update is called once per frame
	void Update () {
        if (textVisible)
        {
            if(Time.time > lastMessageTime + persistance)
            {
                tMPro.enabled = false;
                textVisible = false;
            }
        }

    }

    public void PushNotification(string notificationText,float pers)
    {
        lastMessageTime = Time.time;
        tMPro.text = notificationText; 
        tMPro.enabled = true;
        textVisible = true;
        persistance = pers; 
        
        

    }
}
