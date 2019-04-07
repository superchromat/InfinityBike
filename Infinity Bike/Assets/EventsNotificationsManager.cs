using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsNotificationsManager : MonoBehaviour {
    public Mode mode;
    public TrainingEvent[] trainingEvents;
    public NotificationsManager notificationsManager; 

    private int eventNum;  
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (eventNum < trainingEvents.Length)
        {
            if (Time.time > trainingEvents[eventNum].eventTime)
            {
                notificationsManager.PushNotification(trainingEvents[eventNum].eventMessages);

                eventNum++;
            }

        }


    }
}

[System.Serializable]
public class Mode
{
    public string modeName; 

}
[System.Serializable]
public class TrainingEvent
{
    public string eventName;
    public string eventMessages; 
    public float eventTime; 
}