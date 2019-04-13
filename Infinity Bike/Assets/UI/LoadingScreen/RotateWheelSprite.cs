using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWheelSprite : MonoBehaviour {

    public float angularRotation = 1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.rotation = Quaternion.Euler(0, 0, angularRotation * Time.deltaTime) * transform.rotation;

		
	}
}
