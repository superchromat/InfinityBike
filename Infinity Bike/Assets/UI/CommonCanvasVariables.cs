using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonCanvasVariables : MonoBehaviour
{   

    public GameObject playerRB;

    public void RespawnPlayer()
    {playerRB.GetComponent<Respawn>().CallRespawnAction();}

    public void StartRace()
    {

        playerRB.GetComponent<PlayerMovement>().IdleMode = false;

    }






}
