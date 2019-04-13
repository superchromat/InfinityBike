using UnityEngine;
using UnityEditor; 

[CustomEditor (typeof(TerrainCustomEditor))]
public class TerrainCustomEditorInspector : Editor {
	private TerrainCustomEditor tCE; 
	private Vector2 p1; 
	private float circleTransformRadius;

	private float trackWidth = 7.5f; 
	private int trackSteps=200; 

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector (); 
		
		tCE = target as TerrainCustomEditor; 
		if (GUILayout.Button ("ProcedurallyGenerateHeight")) {
			Undo.RegisterCompleteObjectUndo  (tCE.terrainData, "map height"); 
			tCE.MapHeight (); 
			EditorUtility.SetDirty (target); 
		}

		EditorGUILayout.Space();
		p1 = EditorGUILayout.Vector2Field("CircleTranformCenter:", p1);
		circleTransformRadius = EditorGUILayout.FloatField ("CircleTransformRadius:", circleTransformRadius);

		if (GUILayout.Button ("ApplyCircleHeightTransform")) {
			Undo.RegisterCompleteObjectUndo (tCE.terrainData, "CircleTransform");
			tCE.ApplyCircleHeightTrans (p1,circleTransformRadius,0.25f); 

			EditorUtility.SetDirty (target); 
		}
		if (GUILayout.Button ("ApplyCircleAlphaTransform")) {
			Undo.RegisterCompleteObjectUndo (tCE.terrainData, "CircleTransform");
			tCE.ApplyCircleAlphaTrans (p1,circleTransformRadius,1); 
			EditorUtility.SetDirty (target); 
		}

		EditorGUILayout.Space (); 
		trackWidth = EditorGUILayout.FloatField ("Track Width", trackWidth);
		trackSteps = EditorGUILayout.IntField ("Track Steps", trackSteps); 
		EditorGUILayout.LabelField ("Track generation");
		if (GUILayout.Button ("CreateRoadFromBezier")) {
			Undo.RegisterCompleteObjectUndo (tCE.terrainData, "Generate road");
			tCE.GenerateTrackFromBezier (trackSteps, trackWidth); 
			EditorUtility.SetDirty (target); 
			
		}


	}
}
