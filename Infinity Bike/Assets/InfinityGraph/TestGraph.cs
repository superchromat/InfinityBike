using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGraph : MonoBehaviour {

    public DataGraph dataGraph;

	// Use this for initialization
	void Start () {
        TestCurve();
    }
	
	// Update is called once per frame
	void Update () {
            UpdateTestCurve();
    }

    void TestCurve()
    {
        DataGraph.GraphtList.Clear();
        dataGraph.curves.Clear();

        List<Vector2> data = new List<Vector2>();
        data.Add(Vector2.zero);
        dataGraph.AddDataSeries(data, Color.red, "HelloCurve");
    }

    void UpdateTestCurve()
    {
        foreach (int item in dataGraph.curves.Keys)
        {   
            Vector2 curvePoint;
            
            int count = 0;
            float val = 0;
            if ((count = dataGraph.curves[item].GetCurveDataBuffer().Count) != 0)
            {val = dataGraph.curves[item].GetCurveDataBuffer()[count - 1].x;}   
            else if ((count = dataGraph.curves[item].GetCurveData().Count) != 0)
            {val = dataGraph.curves[item].GetCurveData()[count - 1].x;}   

            curvePoint.x = val + 0.01f;
            curvePoint.y = Mathf.Sin(curvePoint.x * 10f) * Mathf.Sin(curvePoint.x * 5f) * Mathf.Sin(curvePoint.x);

            dataGraph.AddPointToExistingSeries(item, curvePoint);
        }
    }
}
