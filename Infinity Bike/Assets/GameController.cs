using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class GameController : MonoBehaviour {

    public PlayerMovement playerMovement;
    public Button startRaceButton;
    public bool setKeyBoardAsInput = false;
    private bool lastFrameKeyBoardInput = false;

	// Use this for initialization
	void Start () {
        //Enable start game button
        playerMovement.isScriptActivated = false;
        lastFrameKeyBoardInput = setKeyBoardAsInput;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (lastFrameKeyBoardInput != setKeyBoardAsInput)
        {   
            playerMovement.serialValues.SetKeyBoardAsInput(setKeyBoardAsInput);
        }   
        lastFrameKeyBoardInput = setKeyBoardAsInput;

    }

    public void StartRace()
	{
        playerMovement.isScriptActivated = true;
        startRaceButton.gameObject.SetActive(false);
        //Disable start game buttron
	}
}
