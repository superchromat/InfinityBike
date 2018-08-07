using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelper : MonoBehaviour {

    public CommonCanvasVariables commonCanvasVariables;
    private Rigidbody playerRB;
    private MenuButton menuButton;
    public float menuReapearDelay = 1f;
    public int menuID = 0;
    private bool block = false;

	// Use this for initialization
	void Start ()
    {
        playerRB = commonCanvasVariables.playerRB.GetComponent<Rigidbody>();
        menuButton = GetComponent<MenuButton>();

        if (playerRB == null)
        { throw new System.Exception("playerRB is set to null in script attached to " + this.gameObject.name); }

    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (menuButton.activeGroup != menuID && !block && playerRB.velocity.magnitude < 0.01f)
        {
            block = true;
            StartCoroutine(MakeMenuAppear());

        }   		
	}

    IEnumerator MakeMenuAppear()
    {   
        yield return new WaitForSeconds(menuReapearDelay);

        if (playerRB.velocity.magnitude < 0.01f)
        {
            menuButton.ActivateMenuButtons(menuID);

        }
        block = false;
    }   


}
