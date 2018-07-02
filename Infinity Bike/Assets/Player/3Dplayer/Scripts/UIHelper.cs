using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelper : MonoBehaviour {

    public Rigidbody player;
    private MenuButton menuButton;
    public float menuReapearDelay = 1f;
    public int menuID = 0;
    private bool block = false;

	// Use this for initialization
	void Start () {
        menuButton = GetComponent<MenuButton>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (menuButton.activeGroup != menuID && !block && player.velocity.magnitude < 0.01f)
        {
            block = true;
            StartCoroutine(MakeMenuAppear());

        }   

		
	}

    IEnumerator MakeMenuAppear()
    {
        yield return new WaitForSeconds(menuReapearDelay);

        if (player.velocity.magnitude < 0.01f)
        {
            menuButton.ActivateMenuButtons(menuID);

        }
        block = false;


    }
    



}
