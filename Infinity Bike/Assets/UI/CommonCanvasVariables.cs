using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonCanvasVariables : MonoBehaviour
{   

    public GameObject playerRB;

    private void Start()
    {
        if (playerRB == null)
        {
            throw new System.Exception("Please attach player object to the script attached to " + gameObject.name);
        }
    }


    public void RespawnPlayer()
    {playerRB.GetComponent<Respawn>().CallRespawnAction();}

    public void StartRace()
    {

        playerRB.GetComponent<PlayerMovement>().IdleMode = false;

    }

}
