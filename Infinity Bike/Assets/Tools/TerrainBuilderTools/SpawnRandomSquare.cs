using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomSquare : MonoBehaviour {

	public BezierSpline bS; 
	public GameObject squarePrefab; 
	// Use this for initialization
	void Start () {
		float bezierLenght = bS.GetSplineLenght (); 
		for (int i = 0; i < 100; i++) {
			Instantiate (squarePrefab, bS.GetPointFromParametricValue((float)i*bezierLenght/100f), Quaternion.identity);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
