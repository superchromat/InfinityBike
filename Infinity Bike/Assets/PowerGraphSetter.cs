using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerGraphSetter : MonoBehaviour {

    public DataGraph dataGraph;
    public int curveID;
    private Vector2 nextValueToSet;
    // Use this for initialization
    void Start()
    {   
        CreateCurve();
    }
    bool blockUpdate = false;
    bool gotNewValue = false;
    // Update is called once per frame
    void Update()
    {
        if (!blockUpdate && gotNewValue)
        {
            gotNewValue = false;
            blockUpdate = true;
            StartCoroutine(UpdateTestCurve());
        }
    }

    public void AddToCurve(float time, float value)
    {
        gotNewValue = true;
        nextValueToSet = new Vector2(time, value);
    }

    void CreateCurve()
    {   
        List<Vector2> data = new List<Vector2>();
        data.Add(Vector2.zero);
        curveID = dataGraph.AddDataSeries(data, Color.red, "PowerCurve");
    }   

    IEnumerator UpdateTestCurve()
    {

        dataGraph.AddPointToExistingSeries(curveID, nextValueToSet);

        yield return new WaitForSeconds(0.01f);
        blockUpdate = false;

    }
}
