using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepObjOnLoad : MonoBehaviour {
	private bool isCreated = false;
	// Use this for initialization
	void Start () {
		if (!isCreated) 
		{
			isCreated = true;
			DontDestroyOnLoad (this.gameObject);

		}


	}

}
