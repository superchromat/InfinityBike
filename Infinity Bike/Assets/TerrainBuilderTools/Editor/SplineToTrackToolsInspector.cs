using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineToTrackTools))]
public class SplineToTrackToolsInspector : Editor
{
    
	//private BezierSpline spline;
	private SplineToTrackTools sTTT; 







	public override void OnInspectorGUI() {

		sTTT = target as SplineToTrackTools;
		DrawDefaultInspector ();


		if (GUILayout.Button ("Update Track Marker")) {
			Undo.RecordObject (sTTT, "Update Track Marker");
			sTTT.UpdateTrackMarker ();
			EditorUtility.SetDirty (sTTT);
		}

       

		if (GUILayout.Button ("Smooth Terrain For Track")) {
			Undo.RegisterCompleteObjectUndo (sTTT.tCE.terrainData, "Smooth Terrain For Track");
			sTTT.SmoothTerrainForTrack ();
			EditorUtility.SetDirty (sTTT);
		}
		if (GUILayout.Button ("Refresh heigh profile")) {
			sTTT.CalculateHeightsProfile();
			Repaint ();
		}
		EditorGUILayout.LabelField ("Heightmap Profile");
		EditorGUILayout.CurveField (sTTT.heightProfileCurve); 

		EditorGUILayout.LabelField ("Smoothet Heightmap Profile");
		EditorGUILayout.CurveField (sTTT.smoothHeightsProfileCurve); 


	}
    

}