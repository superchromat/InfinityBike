using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelper : MonoBehaviour {

    public CommonCanvasVariables commonCanvasVariables;

    public float menuReapearDelay = 1f;
    private bool block = false;
    public DataGraph dataGraph;

    private Rigidbody playerRB;
    private MenuButtonManager menuButton;

    // Use this for initialization
    void Start ()
    {
        playerRB = commonCanvasVariables.playerRB.GetComponent<Rigidbody>();
        menuButton = GetComponent<MenuButtonManager>();

        if (playerRB == null)
        { throw new System.Exception("playerRB is set to null in script attached to " + this.gameObject.name); }
        dataGraph.pauseDataAppending = true;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {   
        
        if (menuButton.CurrentActivatedID == MenuButtonManager.Menu.DEFAULT && !block && playerRB.velocity.magnitude < 0.01f)
        {   
            block = true;
            StartCoroutine(MakeMenuAppear());
        }   

        dataGraph.pauseDataAppending = (menuButton.CurrentActivatedID == MenuButtonManager.Menu.PAUSESCREEN);
    }



    IEnumerator MakeMenuAppear()
    {   
        yield return new WaitForSeconds(menuReapearDelay);

        if (playerRB.velocity.magnitude < 0.01f)
        {menuButton.ActivateMenuButtons(MenuButtonManager.Menu.PAUSESCREEN);}
        block = false;
    }   


}
