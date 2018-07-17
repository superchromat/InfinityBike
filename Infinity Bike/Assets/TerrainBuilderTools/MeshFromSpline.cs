using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFromSpline : MonoBehaviour {

	private MeshFilter mF; 
	private BezierSpline bezierSpline; 
	public Shape2D shape2D; 
	private OrientedPoint[] path; 
	public float pathIncrementSize; 

	// Use this for initialization
	//void Start () {
	//	mF = GetComponent<MeshFilter> (); 
	//	UpdateMesh (); 
	//}

	public void UpdateMesh() {
		path = GeneratePath ();
		if (shape2D == null) {
			shape2D = new Shape2D (); 
		}

		if (path == null) {
			path = new OrientedPoint[5]; 
		}
		mF = GetComponent<MeshFilter> (); 
		if (mF.sharedMesh == null) {
			mF.sharedMesh = new Mesh (); 
		}
		Mesh mesh = mF.sharedMesh; 
		shape2D.CalculateUS (); 
		Extrude (mesh, shape2D, path); 

	}
	public void UpdateMesh2() {

		mF = GetComponent<MeshFilter> (); 
		if (mF.sharedMesh == null) {
			mF.sharedMesh = new Mesh (); 
		}
		Mesh mesh = mF.sharedMesh; 

		//
		Vector3[] vertices = new Vector3[4]; 
		Vector3[] normals = new Vector3[4]; 
		Vector2[] uvs = new Vector2[4]; 

		int[] trianglesIndices = new int[6];

		vertices [0] = Vector3.back + Vector3.left; 
		vertices [1] = Vector3.back + Vector3.right;
		vertices [2] = Vector3.forward + Vector3.left; 
		vertices [3] = Vector3.forward + Vector3.right; 

		for (int i = 0; i < 4; i++) {
			normals [i] = Vector3.up;
		}

		uvs [0] = Vector2.zero;
		uvs [1] = Vector2.right; 
		uvs [2] = Vector2.up; 
		uvs [3] = Vector2.up + Vector2.right; 

		trianglesIndices [0] = 1;
		trianglesIndices[1] = 0; 
		trianglesIndices [2] = 2; 

		trianglesIndices [3] = 1; 
		trianglesIndices [4] = 2; 
		trianglesIndices [5] = 3;
		//




		mesh.Clear (); 
		mesh.vertices = vertices; 
		mesh.normals = normals; 
		mesh.uv = uvs;
		mesh.triangles = trianglesIndices; 


		
	}

	public void Extrude( Mesh mesh, Shape2D shape, OrientedPoint[] path) {
		int vertsInShape = shape.verts.Length; 
		int segments = path.Length - 1;
		int edgeLoops = path.Length;
		int vertCount = vertsInShape * edgeLoops;
		int triCount = (shape.lines.Length-1) * segments*2; 
		int triIndexCount = triCount * 3; 


		int[] trianglesIndices = new int[ triIndexCount]; 
		Vector3[] vertices = new Vector3[vertCount]; 
		Vector3[] normals = new Vector3[vertCount]; 
		Vector2[] uvs = new Vector2[vertCount];


		for (int i = 0; i < path.Length; i++) { //for each oriented point
			int offset = i * vertsInShape; 
			for(int j = 0; j< vertsInShape; j++) {
				int id = offset + j; 
				vertices [id] = path[i].LocalToWorld (shape.verts [j]);
				normals [id] = path [i].LocalToWorldDirection (shape.normals [j]);
				uvs [id] = new Vector2 (shape.us [j], path [i].cumulDistance);//i/((float)edgeLoops));
			}
		}

		int ti = 0; 
		for (int i = 0; i < segments; i++) {
			int offset = i * vertsInShape; 
			for (int l = 0; l< shape.lines.Length-1; l ++) {
				int a = offset + shape.lines [l] + vertsInShape; 
				int b = offset + shape.lines [l]; 
				int c = offset + shape.lines [l + 1]; 
				int d = offset + shape.lines [l + 1] + vertsInShape;
				trianglesIndices [ti] = a; ti++;
				trianglesIndices [ti] = c; ti++;
				trianglesIndices [ti] = b; ti++;
				trianglesIndices [ti] = c; ti++;
				trianglesIndices [ti] = a; ti++;
				trianglesIndices [ti] = d; ti++;

			}
		}


		mesh.Clear (); 
		mesh.vertices = vertices; 
		mesh.normals = normals; 
		mesh.uv = uvs;
		mesh.triangles = trianglesIndices; 

	}
	public OrientedPoint[] GeneratePath() {
		BezierSpline bezierSpline = GetComponent<BezierSpline> (); 
		if (bezierSpline == null) {
			Debug.Log ("No bezier spline curve assigned to the game object"); 
			return null;
		}
		int numPoints = (int) Mathf.Ceil (bezierSpline.GetSplineLenght () / pathIncrementSize);
		float realIncrementSize = bezierSpline.GetSplineLenght () / numPoints; 


		OrientedPoint[] path = new OrientedPoint[numPoints];

		for (int i = 0; i < numPoints; i++) {
			path [i].position = bezierSpline.GetPointFromParametricValue (i * realIncrementSize); 
			path [i].rotation = bezierSpline.GetOrientationFromParametricValue (i * realIncrementSize);
			path [i].cumulDistance = i * realIncrementSize; 
		}

		return path; 



	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
[System.Serializable]
public class Shape2D {
	public Vector2[] verts; 
	public Vector2[] normals; 
	public float[] us; 
	public int[] lines; 

	public Shape2D(){
		verts = new Vector2[2]; 
		verts [0] = Vector2.left;
		verts [1] = Vector2.right; 

		normals = new Vector2[2]; 
		normals [0] = Vector2.up; 
		normals [1] = Vector2.up;

		lines = new int[2];
		lines [0] = 0; 
		lines [1] = 1; 
	}
	public int GetLenght(){
		if (verts != null) {
			return verts.Length;
		} else {
			return 0; 
		}

	}

	public Vector3[] Get3DPoints() {//Usefull to draw the shape with the inspector tool
		if (GetLenght () > 0) {
			
			Vector3[] points = new Vector3[verts.Length]; 
			for (int i = 0; i < verts.Length; i++) {
				points [i] = new Vector3 (verts [i].x, verts [i].y, 0);
			}
			return points;
		} else {
			return null;
		}

	}
	public void CalculateUS() {
		us [0] = 0; 
		for (int i = 1; i < verts.Length; i++) {
			us [i] = us [i - 1] + (verts [i] - verts [i - 1]).magnitude;
		}
		
	}
	public float GetShapePerimeter() {
		float perimeter = 0; 
		for (int i = 0; i < verts.Length-1; i++) {
			perimeter += (verts [i + 1] - verts [i]).magnitude;
		}
		return perimeter;
	}
}


[System.Serializable]
public struct OrientedPoint {
	public Vector3 position;
	public Quaternion rotation;
	public float cumulDistance; 

	public OrientedPoint( Vector3 position, Quaternion rotation,float cumulDistance) {
		this.position = position; 
		this.rotation = rotation;
		this.cumulDistance = cumulDistance; 
	}

	public  Vector3 LocalToWorld( Vector3 point) {
		return position + rotation * point; 
	}

	public Vector3 WorldToLocal( Vector3 point) {
		return Quaternion.Inverse (rotation) * (point - position); 
	}

	public Vector3 LocalToWorldDirection( Vector3 dir) {
		return rotation * dir; 
	}
}
