using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour {

    MeshFilter waterMeshFilter;
    Vector3[] waterVertices;

    public float power = 0.01f;
    public float scale = 1f;
    public float speed = 1;

    public float zOffSet = 0; 
    private float xOffSet;
    private float yOffSet;

	// Use this for initialization
	void Start () {
        waterMeshFilter = GetComponent<MeshFilter>();
       
		
	}
	
	// Update is called once per frame
	void Update () {
        xOffSet += Time.deltaTime * speed;
        yOffSet += Time.deltaTime * speed;
        Vector3[] normals = waterMeshFilter.mesh.normals;
        waterVertices = waterMeshFilter.mesh.vertices;
        for (int i = 0; i < waterVertices.Length; i++ ){
            waterVertices[i].z =  CalculateHeight(waterVertices[i].x, waterVertices[i].y);
            //normals[i] = Vector3.forward + CalculateHeight(waterVertices[i].x, waterVertices[i].y) * Vector3.right;
            //Debug.Log(i);
        }
        //Debug.Log(waterVertices[0]);
        waterMeshFilter.mesh.vertices = waterVertices;
        waterMeshFilter.mesh.RecalculateNormals();  
	}

    float CalculateHeight(float x, float y) {
        float xCor = x * scale;
        float yCor = y * scale; 

        return power * Mathf.PerlinNoise(xCor + xOffSet, yCor + yOffSet)+zOffSet;
   
    }
}

