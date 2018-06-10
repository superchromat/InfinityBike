using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGame : MonoBehaviour {

    // Use this for initialization
    Vector3 startPosition = Vector3.zero;
    public Camera cameraMain;
    public GameObject npcManager;
	void Start ()
    {
        startPosition = transform.position;



    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(cameraMain==null)
            cameraMain.transform.position = startPosition;

            GetComponent<Respawn>().RespawnObject();
            transform.position = startPosition;

            Transform[] npcTransforms = npcManager.GetComponentsInChildren<Transform>();
            for (int i = 1; i < npcTransforms.Length; i++)
            {
                npcTransforms[i].gameObject.SetActive(false);


            }



        }




	}
}
