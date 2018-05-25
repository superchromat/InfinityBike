using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusController : MonoBehaviour
{

    public float finalHeight;

    private float currentHeight = 0;
    private Vector3 initialPosition;

    public bool isMoving = false;
    bool isActivated = true;

    public SkinnedMeshRenderer octopusMesh; 

    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;
        //octopusMesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            currentHeight = Mathf.Lerp(currentHeight, finalHeight, Time.deltaTime);
            transform.position = new Vector3(initialPosition.x, currentHeight, initialPosition.z);

            if (Mathf.Abs((finalHeight - currentHeight)) < 0.1f)
            { isMoving = false; }

        }
    }
    public void ActivateOctopus(){
        isActivated = true;
        isMoving = true;
        //octopusMesh.gameObject.SetActive(true);
        octopusMesh.enabled = true; 
       
    }
}
