using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class GameController : MonoBehaviour {

    public PlayerMovementV2 playerMovement;
    public Button startRaceButton; 


	// Use this for initialization
	void Start () {
        //Enable start game button
        playerMovement.enabled = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartRace()
	{
        playerMovement.enabled = true;
        startRaceButton.gameObject.SetActive(false);
        //Disable start game buttron
	}
}
