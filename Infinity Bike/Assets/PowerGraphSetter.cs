using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerGraphSetter : MonoBehaviour {

    public DataGraph dataGraph;

    // Use this for initialization
    void Start()
    {
        CreateCurve();
    }

    // Update is called once per frame
    void Update()
    {   
        UpdateTestCurve();
    }   

    void CreateCurve()
    {
        DataGraph.GraphtList.Clear();
        dataGraph.graphCurves.Clear();

        List<Vector2> data = new List<Vector2>();
        data.Add(Vector2.zero);
        dataGraph.AddDataSeries(data, Color.red, "HelloCurve");
    }

    IEnumerator UpdateTestCurve()
    {

        yield return new WaitForSeconds(0.01f);
    }
}
