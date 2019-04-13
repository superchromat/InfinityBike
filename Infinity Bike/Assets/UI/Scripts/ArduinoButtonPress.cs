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

    void Start () {
        if (onScreenButtonParent.Length > 0)
        foreach (Transform item in onScreenButtonParent)
        {
            Button[] butList = item.GetComponentsInChildren<Button>(true);
            foreach (Button button in butList)
            {
                onScreenButton.Add(button);
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        SelectClosestButton();
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

    void SelectClosestButton()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        Button lastButton = closestButton;

        FindClosestActiveButton();

        if (lastButton != null)
        {
            lastButton = closestButton;

        }

        if (closestButton != null)
        {closestButton.Select();}


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
