using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VelocityGetter : MonoBehaviour {

    public CommonCanvasVariables commonCanvasVariables;
    private Rigidbody playerRB;
    private Text vel;
    private float lastVelocity;
    public float Velocity{get{return lastVelocity; }}
    public TimerController timerController;

    public DataGraph dataGraph;
    int curveKey = 0;


    // Use this for initialization
    void Start () {

        playerRB = commonCanvasVariables.playerRB.GetComponent<Rigidbody>();
        vel = GetComponent<Text> ();

        if (playerRB == null)
        {throw new System.Exception("playerRB is set to null in script attached to " + this.gameObject.name);}
        CreateCurve();
    }
	
	// Update is called once per frame
	void LateUpdate () 
	{
        lastVelocity = Mathf.Floor(playerRB.velocity.magnitude * 10000) / 10000;
        vel.text = (lastVelocity).ToString();

        if (!blockUpdate)
        { StartCoroutine(UpdateTestCurve()); }
    }

    void CreateCurve()
    {   
        dataGraph.graphCurves.Clear();
        
        List<Vector2> data = new List<Vector2>();
        data.Add(Vector2.zero);
        curveKey = dataGraph.AddDataSeries(data, Color.red, "Power Curve");
    }   

    bool blockUpdate = false;
    IEnumerator UpdateTestCurve()
    {
        blockUpdate = true;

        dataGraph.AddPointToExistingSeries(curveKey, new Vector2(timerController.StartTime + Time.time, Velocity));

        yield return new WaitForSeconds(0.01f);
        blockUpdate = false;
    }

}
