using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MeshFromSpline))]
public class MeshFromSplineInspector : Editor {
	MeshFromSpline meshFromSpline; 

	void OnSceneGUI(){

		meshFromSpline = target as MeshFromSpline;
		if (meshFromSpline.shape2D != null) {
			if (meshFromSpline.shape2D.GetLenght()>0) {
				Handles.color = Color.red;

				Handles.DrawPolyLine (meshFromSpline.shape2D.Get3DPoints()); 
			}
		}


	}


	public override void OnInspectorGUI() {
		DrawDefaultInspector (); 
		meshFromSpline = target as MeshFromSpline; 
		if (GUILayout.Button ("Update Mesh")) {
			Undo.RecordObject (meshFromSpline, "Update Mesh");
			meshFromSpline.UpdateMesh (); 



			EditorUtility.SetDirty (meshFromSpline);

		}

	}
}
