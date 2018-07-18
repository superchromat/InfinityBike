using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineToTerrainTools))]
public class SplineToTerrainToolsInspector : Editor
{
    
	//private BezierSpline spline;
	private SplineToTerrainTools sTTT; 







	public override void OnInspectorGUI() {

		sTTT = target as SplineToTerrainTools;
		DrawDefaultInspector ();


		if (GUILayout.Button ("Update Track Marker")) {
			Undo.RecordObject (sTTT, "Update Track Marker");
			sTTT.UpdateTrackMarker ();
			EditorUtility.SetDirty (sTTT);
		}

		if (GUILayout.Button ("Adapt Spline to terrain")) {
			Undo.RegisterCompleteObjectUndo (sTTT.tCE.terrainData, "Adapt spline to terain");
			sTTT.AdaptSplineToTerrain ();
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