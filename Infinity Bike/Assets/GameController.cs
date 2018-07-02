using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class GameController : MonoBehaviour {

    public PlayerMovement playerMovement;
    public Button startRaceButton;

	// Use this for initialization
	void Start ()
    {   
        //Enable start game button
        playerMovement.isScriptActivated = false;
    }   
	

    public void StartRace()
	{   
        playerMovement.isScriptActivated = true;
        startRaceButton.gameObject.SetActive(false);
        //Disable start game buttron
	}   
}
