using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineToTerrainTools : MonoBehaviour {

	public TerrainCustomEditor tCE;
	public BezierSpline spline;
	public TextMesh textMesh; 

	[HideInInspector]
	public AnimationCurve heightProfileCurve; //Tool to visualize the height profile curbe
	[HideInInspector]
	public AnimationCurve smoothHeightsProfileCurve; 

	// Smoothing parameters 

	public bool markerEnabled = true; 

	public float samplingStepSize;
	public float smoothingStepSize; 

	public int windowSize; 
	public float smoothRadius; 
	public float gaussianFactor;


	// Heigh profile parameters
	private float[] pArray; // parametric distance parameter 
	private float[] heightsProfile;
	private float[] smoothHeightsProfile;

	private float equiStepSize;
	private int equiNumSteps;



	public void UpdateTrackMarker() {
		Debug.Log ("Applying track markers");
		float currentDistance = 0; 
		float splineLenght = spline.GetSplineLenght ();

		while (this.transform.childCount != 0) {
			DestroyImmediate (this.transform.GetChild (0).gameObject);
		}
		if (markerEnabled) {
			while (currentDistance < splineLenght) {
				Vector3 splinePoint = spline.GetPointFromParametricValue (currentDistance); 
				float yTerrainVal = tCE.WorldToTerrainHeight (new Vector2 (splinePoint.x, splinePoint.z));
				TextMesh tM = Instantiate (textMesh, new Vector3 (splinePoint.x, yTerrainVal, splinePoint.z), Quaternion.identity);
				tM.transform.parent = this.transform; 
				tM.text = yTerrainVal.ToString (); 


				currentDistance = currentDistance + samplingStepSize; 

			}
		}


	}

	public void CalculateHeightsProfile() {
		equiNumSteps = Mathf.CeilToInt (spline.GetSplineLenght () / samplingStepSize);
		equiStepSize = spline.GetSplineLenght () / (float)(equiNumSteps-1);
		AnimationCurve temporaryCurve =  new AnimationCurve (); 
		//heightProfileCurve = new AnimationCurve (); 
		//while (heightProfileCurve.length != 0) {
		//	heightProfileCurve.RemoveKey (0);
		//}


		pArray = new float[equiNumSteps]; 
		heightsProfile = new float[equiNumSteps]; 


		for (int i = 0; i < equiNumSteps; i++) {
			pArray [i] = (float) i* equiStepSize; 
			Vector3 splinePoint = spline.GetPointFromParametricValue (pArray[i]); 
			heightsProfile[i] = tCE.WorldToTerrainHeight (new Vector2 (splinePoint.x, splinePoint.z));
			temporaryCurve.AddKey (pArray [i], heightsProfile [i]);
		}
		heightProfileCurve = temporaryCurve; //Had to do it like it because the inspector didn't refresh. 
		CalculateSmoothHeigthsProfile(); 
	}

	public void SmoothTerrainForTrack() {
		CalculateHeightsProfile ();
		float currentDistance = 0;
		float splineLenght = spline.GetSplineLenght ();

		while (currentDistance < splineLenght) {
			Vector3 splinePoint = spline.GetPointFromParametricValue (currentDistance); 
			Vector2 splinePoint2D = new Vector2 (splinePoint.x, splinePoint.z);

			//float yTerrainVal = tCE.WorldToTerrainHeight (splinePoint2D);
			float yVal = GetValFromHeightProfile(smoothHeightsProfile,pArray,currentDistance); 

			tCE.ApplyRadialHeightBlend (splinePoint2D, smoothRadius, yVal ,gaussianFactor);//yTerrainVal);
			tCE.ApplyRadialAlphaBlend (splinePoint2D, smoothRadius ,gaussianFactor*2,1);
			//tCE.ApplyRadialAlphaBlend (splinePoint2D, smoothRadius ,2*gaussianFactor,2);
			currentDistance = currentDistance + smoothingStepSize; 
		}

		
	}

	float GetValFromHeightProfile(float [] heightProfile, float[] pArray, float P) {
		if (P > spline.GetSplineLenght ()) {
			Debug.Log ("P is too big"); 
			return 0; 
		}
		int lowerPIndex = FindLowestNeighbourIndex (pArray, P);
		//float lowerPHeight = heightProfile [lowerPIndex];

		return Mathf.Lerp (heightProfile [lowerPIndex], heightProfile [lowerPIndex + 1], (P - (float)pArray [lowerPIndex]) / equiStepSize);


	}

	public void CalculateSmoothHeigthsProfile() {//float [] heightsProfile, float [] smoothHeightsProfile) {
		smoothHeightsProfileCurve = new AnimationCurve();
		smoothHeightsProfile = new float[heightsProfile.Length];
		for (int i = 0; i < smoothHeightsProfile.Length; i++) {
			smoothHeightsProfile [i] = 0; 
			int avNumber = 0; 
			for (int j = -windowSize; j < windowSize; j++) {
				int k = i + j; 
				if (k >= 0 && k < smoothHeightsProfile.Length) {
					smoothHeightsProfile [i] += heightsProfile[k]; 
					avNumber++;
				}

			}
			smoothHeightsProfile[i] = smoothHeightsProfile[i] / (float)avNumber; 
			smoothHeightsProfileCurve.AddKey (pArray [i], smoothHeightsProfile [i]);



		}
	}

	public void AdaptSplineToTerrain() {
		int splineNodes = spline.ControlPointCount;
		Debug.Log (splineNodes); 
		for (int i = 0; i < splineNodes; i += 3) {
			Vector3 nodePos = spline.GetControlPoint (i);
			float terrainHeight = tCE.WorldToTerrainHeight (new Vector2 (nodePos.x, nodePos.z)); 
			nodePos.y = terrainHeight;
			spline.SetControlPoint (i, nodePos);
		}
	}

	/// <summary>
	/// There's probably a better way of doing this. I copy pasted code I wrote form BezierSpline
	/// </summary>
	/// <returns>The lowest neighbour index.</returns>
	/// <param name="array">Array.</param>
	/// <param name="value">Value.</param>
	///
	int FindLowestNeighbourIndex(float[] array, float value) {
		int lowestIndex = 0; 
		float lowestDistance = value - array[0]; 

		for (int i = 1; i < array.Length; i++) {
			float distance = value - array [i]; 
			if (distance >= 0f && distance < lowestDistance) {
				lowestDistance = distance;
				lowestIndex = i; 
			}
		}
		return lowestIndex; 


	}


		



}
