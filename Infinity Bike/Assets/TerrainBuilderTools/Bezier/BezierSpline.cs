using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class BezierSpline : MonoBehaviour
{   
    private bool loop;    
    [SerializeField]
    private Vector3[] points;

    public SaveLoad saveLoad = new SaveLoad();
    private void OnValidate()
    {
        saveLoad.dataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if(points.Length ==0)
        Load();

        UpdateParametricMapping();
    }


    [SerializeField]
    private BezierControlPointMode[] modes;
    
    [ContextMenu("Save track data")]
    public void Save()
    {
            Vector3Serialisable nodeListSerialisable = new Vector3Serialisable(new List<Vector3>(points), loop);
        SaveLoad.SaveLoadUtilities<Vector3Serialisable>.Save(nodeListSerialisable, "TrackBesierPoints.Besier");
    }
    [ContextMenu("Load track data")]
    public void Load()
    {
        Vector3Serialisable nodeListSerialisable = new Vector3Serialisable();
        SaveLoad.SaveLoadUtilities<Vector3Serialisable>.Load(out nodeListSerialisable, "TrackBesierPoints.Besier");
        nodeListSerialisable.SetValuesToNodeList(out points, out loop);


    }

    public bool Loop
    {
        get
        {
            return loop;
        }
        set
        {
            loop = value;
            if (value == true)
            {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }

    public int ControlPointCount{
        get{
            return points.Length; 
        }
    }


	//Parametric Mapping parameters
	private float splineLenght;

	private int linearApproxSteps = 1000; 
	public int LinearApproxSteps {
		get {
			return linearApproxSteps; 
			}
		set {
			linearApproxSteps = value; 
			UpdateParametricMapping (); 
		}
	}
	private float[] cumulLenghts; 
	private float[] discreteT; 
	public GameObject markerPrefab; 

    public Vector3 GetControlPoint (int index)
    {return points[index];}

    public void SetControlPoint(int index, Vector3 point){
        if (index % 3 == 0)
        {
            Vector3 delta = point - points[index];

            if (loop){
                if (index == 0)
                {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                else if (index == points.Length - 1)
                {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else {
                    points[index - 1] += delta;
                    points[index + 1] += delta; 
                }
                
            }
            else {
                if (index > 0)
                {
                    points[index - 1] += delta;
                }
                if (index + 1 < points.Length)
                {
                    points[index + 1] += delta;
                }
            }

        }

        points[index] = point;
        EnforceMode(index);
    }
    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        int modeIndex = (index + 1) / 3;

        modes[modeIndex] = mode;
        if (Loop) {
            if (modeIndex ==0){
                modes[modes.Length - 1] = mode;

            }
            else if (modeIndex == modes.Length-1){
                modes[0] = mode; 
             }
        }

        EnforceMode(index);
         }
    private void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if (mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == modes.Length - 1)
        {
			UpdateParametricMapping (); 
            return;
        }
        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0){
                fixedIndex = points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;

            if (enforcedIndex >= points.Length){
                enforcedIndex = 1; 
            }
        }
        else
        {
            
            fixedIndex = middleIndex + 1;
            if (fixedIndex>= points.Length){
                fixedIndex = 1; 
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0)
            {
                enforcedIndex = points.Length - 2;
            }
        }
        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];

        points[enforcedIndex] = middle + enforcedTangent;  
      
        if (mode == BezierControlPointMode.Aligned){
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;

		UpdateParametricMapping (); 
    }

    public BezierControlPointMode GetControlPointMode(int index){
        return modes[(index + 1) / 3];
    }
   
    public int CurveCount {
        get {
            return (points.Length - 1) / 3;
        }
    }

	public void Reset()
	{
		points = new Vector3[] {
			new Vector3(0f, 0f, 10f),
			new Vector3(0f, 0f, 20f),
			new Vector3(0f, 0f, 30f),
			new Vector3(0f, 0f, 40f)
		};
        modes = new BezierControlPointMode[] {
            BezierControlPointMode.Mirrored,
            BezierControlPointMode.Mirrored 
        };
	}

    public Vector3 GetPoint(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        Vector3 toReturn = Vector3.zero;
        try
        {
            toReturn = transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
        }
        catch
        {
            Debug.Log(" Error in BesierSpline.cs - > Get point \n\tPoint length : " + points.Length + " \tindex : " + i);
            // temporary. I keept getting an error from this chunk of code but I can't find out why.
            // 1. The problem was the script didn't reliably keep the variable points populated. It should be fixed but I keep this herre so see.


        }

        return toReturn;

    }
	public  Vector3 GetVelocity(float t)
	{
        int i; 
        if(t >= 1f) {
            t = 1f;
            i = points.Length - 4; 

        }
        else {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        float factor = 1f/((float)CurveCount); 
        return factor*transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i+1], points[i+2],points[i+3], t));// -
			//transform.position;
	}

	public Vector3 GetDirection(float t)
	{
		return GetVelocity(t).normalized;
	}
    public Vector3 GetDirection(float t,bool forward)
    {
        float sign = 1; 
        if (!forward){
            sign = -1; 
        }
        return sign*GetVelocity(t).normalized;
    }

	public Quaternion GetOrientation(float t) {
		return Quaternion.LookRotation (GetDirection (t), Vector3.up); 
	}

	public void AddCurve() {
		Vector3 point = points [points.Length - 1];
		Array.Resize (ref points, points.Length + 3);
		point.z += 10f; 
		points [points.Length - 3] = point;
		point.z += 10f; 
		points [points.Length - 2] = point;
		point.z += 10f; 
		points [points.Length - 1] = point;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);

        if (loop) {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }
	/// <summary>
	/// The parametric mapping is an array in wich this scrips calculate the spline cummulative distance 
	/// for a serie of points starting at the spline origin. This function must be updated every time the
	/// spline is modified. 
	/// </summary>
	void UpdateParametricMapping () {
		//Calculate Spline Lenght while stocking cummulative lenght in an array
		cumulLenghts = new float[linearApproxSteps]; 	
		discreteT = new float[linearApproxSteps]; 

		cumulLenghts [0] = 0f; 
		discreteT [0] = 0f; 

		float increment = 1f / ((float)linearApproxSteps-1f); 

		Vector3 lastPoint = GetPoint(0); 
		for (int i = 1; i < linearApproxSteps; i++) {
			discreteT [i] = (float)i * increment; 
			Vector3 currentPoint = GetPoint (discreteT[i]); 
			cumulLenghts [i] = cumulLenghts [i - 1] + (currentPoint - lastPoint).magnitude;
			lastPoint = currentPoint;

		}



	}

	///<summary>
	///Returns the sline lenght approximation (not analytics).
	///</summary>
	public float GetSplineLenght() {

        return cumulLenghts [LinearApproxSteps-1]; 
		
	}

	/// <summary>
	/// Inputs a parametric value p (a distance on the spline) and returns the t value associated.
	/// </summary>
	/// <returns>The T from parametric value.</returns>
	/// <param name="p">P.</param>
	public float GetTFromParametricValue(float p) {
		float splineLenght = GetSplineLenght (); 
		if (p > splineLenght) {
			Debug.Log ("P is too big");
			return 0f; 
		}
		//Find closest point in the array
		int pClosestIndex = FindLowestNeighbourIndex(cumulLenghts,p);
		float pClosest = cumulLenghts [pClosestIndex];
		float tClosest = discreteT [pClosestIndex];

		//float pNext = cumulLenghts [pClosestIndex + 1]; 
		//float tNext = discreteT [pClosestIndex + 1];

		float t = (p - pClosest) / splineLenght + tClosest; 

		return t; 
	}

	/// <summary>
	/// Return a point from the spline, but associated to the parametric value p. 
	/// </summary>
	/// <returns>The point from parametric value.</returns>
	/// <param name="p">P.</param>
	public Vector3 GetPointFromParametricValue(float p) {
		return GetPoint(GetTFromParametricValue(p)); 
		}
    
	public Quaternion GetOrientationFromParametricValue(float p) {
		return GetOrientation (GetTFromParametricValue (p)); 
	}
    
	/// <summary>
	/// This function is used for interpolation. For the input array, returns find the closest lower float 
	/// to value and returns its index. 
	/// </summary>
	/// <returns>The lowest neighbour index.</returns>
	/// <param name="array">Array.</param>
	/// <param name="value">Value.</param>
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
	/// <summary>
	/// A debug function used to test the parametric spline functions. 
	/// </summary>
	public void AddDistanceMarker() {

		while (this.transform.childCount != 0) {
			DestroyImmediate (this.transform.GetChild (0).gameObject);
		}

		int numMarker = 10; 
		for (int i = 0; i < numMarker; i++) {
			float distance = GetSplineLenght () / (float) (numMarker-1)  * (float) i; 
			Vector3 position = GetPointFromParametricValue (distance);
			GameObject marker = Instantiate (markerPrefab, position, Quaternion.identity);
			marker.transform.parent = this.transform; 
			marker.GetComponent<TextMesh> ().text = distance.ToString ();
		}
	}

}

