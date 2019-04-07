using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;





//[ExecuteInEditMode]
public class Emission : MonoBehaviour {

    public float frequency = 1;
    public float amplitude = 1;
    public Vector3 positionOffset = Vector3.zero;
    private float time = 0;
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = amplitude*(new Vector3(Mathf.Cos(time) * frequency,0, Mathf.Sin(time) * frequency));
        transform.position += positionOffset;
        time = Time.deltaTime;

    }
}
