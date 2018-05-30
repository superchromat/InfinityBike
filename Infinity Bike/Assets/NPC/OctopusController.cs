using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusController : MonoBehaviour
{

    public float finalHeight;

    private float currentHeight = 0;
    private Vector3 initialPosition;

    public bool isActivated = false;

    public SkinnedMeshRenderer octopusMesh; 

    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
        {
            currentHeight = Mathf.Lerp(currentHeight, finalHeight, Time.deltaTime);
            transform.position = new Vector3(initialPosition.x, currentHeight, initialPosition.z);

            if (Mathf.Abs((finalHeight - currentHeight)) < 0.1f)
            { isActivated = false; }

        }
    }

    public void ActivateOctopus()
	{
        isActivated = true;
        octopusMesh.enabled = true; 
       
    }
}
