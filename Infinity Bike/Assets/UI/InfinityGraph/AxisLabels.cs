using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

//[ExecuteInEditMode]
[RequireComponent(typeof(DataGraph))]
public class AxisLabels : MonoBehaviour {

    public string verticalName = "verticalName";
    public string verticalUnit = "units";

    public string horizontalName = "horizontalName";
    public string horizontalUnit = "units";

    public GameObject verticalLabel = null;
    public GameObject horizontalLabel = null;

    public GameObject textObj = null; 
    public List<RectTransform> horizontalNumbers = new List<RectTransform>();
    public List<RectTransform> verticalNumbers = new List<RectTransform>();

    private float parentWidth = 150;
    private float parentHeight = 150;

    public Vector2 horizontalOffset = new Vector2(0, 0);
    public Vector2 verticalOffset = new Vector2(0, 0);

    private DataGraph dataPlot;

    // Use this for initialization
    void Start ()
    {   
        parentWidth = this.gameObject.GetComponent<RectTransform>().rect.width;
        parentHeight = this.gameObject.GetComponent<RectTransform>().rect.height;

        if (textObj == null)
        {   
            Debug.LogError("Textbox prefab is missing.");
            gameObject.SetActive(false);
        }
        
        ClearBoxes();
        CreateLabelBoxes();

        dataPlot = GetComponent<DataGraph>();
        PrepareAxisLAbels(dataPlot.horizontalAxis,horizontalNumbers);
        PrepareAxisLAbels(dataPlot.verticalAxis,verticalNumbers);

        StartCoroutine(WaitForMapUpdate());
    }

    void setLabels(GameObject labels, String labelName )
    {   
        if (labels.name == "")
        { labels.name = "Unnamed label"; }

        TextMeshProUGUI txtmeshProGui = labels.GetComponent<TextMeshProUGUI>();
        txtmeshProGui.text = labelName;
        txtmeshProGui.alignment = TextAlignmentOptions.Center;
    }   

    void CreateLabelBoxes()
    {
        if (verticalLabel == null)
        {   
            verticalLabel = GameObject.Instantiate(textObj);
            verticalLabel.transform.SetParent(this.gameObject.transform);
            setLabels(verticalLabel, verticalName+" ( " + verticalUnit + " )");
            RectTransform vertialRect = verticalLabel.GetComponent<RectTransform>();
            
            vertialRect.Rotate(Vector3.forward, 90f);
            vertialRect.sizeDelta = new Vector2(parentHeight, vertialRect.rect.height);
            vertialRect.anchoredPosition = new Vector2(vertialRect.rect.height, 0);
        }   

        if (horizontalLabel == null)
        {   
            horizontalLabel = GameObject.Instantiate(textObj);
            horizontalLabel.transform.SetParent(this.gameObject.transform);
            setLabels(horizontalLabel, horizontalName + " ( " + horizontalUnit + " )");
            RectTransform horizontalRect = horizontalLabel.GetComponent<RectTransform>();

            horizontalRect.sizeDelta = new Vector2(parentHeight, 2f * horizontalRect.rect.height);
            horizontalRect.anchoredPosition = new Vector2(0, 0);
        }   
            
    }       

    void ClearBoxes()
    {
        List<RectTransform>  textBoxes = new List<RectTransform>(this.GetComponentsInChildren<RectTransform>());
        for (int i = 0; i < textBoxes.Count; i++)
        {
            string name = textBoxes[i].gameObject.name;
            if (textBoxes[i] != null && textBoxes[i].gameObject != this.gameObject && textBoxes[i].gameObject != verticalLabel && textBoxes[i].gameObject != horizontalLabel)
            {
                if (Application.isPlaying)
                    Destroy(textBoxes[i].gameObject);
                else
                    DestroyImmediate(textBoxes[i].gameObject);
            }
        }
        textBoxes.Clear();
        horizontalNumbers.Clear();
        verticalNumbers.Clear();
    }
    
    // Update is called once per frame
    bool coroutineBlocker = false;
	void Update ()
    {   
        
        if (!coroutineBlocker)
        {StartCoroutine(WaitForMapUpdate());}   
        
    }

    void PrepareAxisLAbels(DataGraph.Axis axis, List<RectTransform> textBoxes)
    {
        for (int i = 0; i < axis.majorMarkerList.Count; i++)
        {
            GameObject obj = GameObject.Instantiate(textObj);
            obj.name = textObj.name +"_"+ textBoxes.Count.ToString();
            obj.transform.SetParent(this.gameObject.transform);
            textBoxes.Add(obj.GetComponent<RectTransform>());
        }

    }

    IEnumerator WaitForMapUpdate()
    {   
        coroutineBlocker = true;
        
        do
        { yield return null; }
        while (dataPlot.mapUpdate != null && dataPlot.mapUpdate.IsAlive);
        
        
        if (dataPlot.horizontalAxis.majorMarkerList.Count > 0)
        {   
            String unit = UpdateLabels(dataPlot.horizontalAxis,horizontalNumbers);
            setLabels(verticalLabel, verticalName + " ( " + unit + " " + verticalUnit + " )");
        }   

        if (dataPlot.verticalAxis.majorMarkerList.Count > 0)
        {   
            String unit = UpdateLabels(dataPlot.verticalAxis,verticalNumbers);
            setLabels(horizontalLabel, horizontalName + " ( " + unit + " " + horizontalUnit + " )");
        }   


        coroutineBlocker = false;
    }   

    string UpdateLabels(DataGraph.Axis axis, List<RectTransform> textBoxes)
    {   
        float unitAdjustement = 0;
        if(!float.IsInfinity(axis.axisValueMinMax.y))
        if (Mathf.Abs(axis.axisValueMinMax.y) > 10 || (Mathf.Abs(axis.axisValueMinMax.y) < 1))
        {   
            while (Mathf.Abs(axis.axisValueMinMax.y) * Mathf.Pow(10f, -unitAdjustement) > 10)
            { unitAdjustement++; }

            while (Mathf.Abs(axis.axisValueMinMax.y) * Mathf.Pow(10f, -unitAdjustement) < 1)
            { unitAdjustement--; }
        }   
        
        for (int i = 0; i < dataPlot.horizontalAxis.majorMarkerList.Count; i++)
        {   
            float width = Mathf.Abs(textBoxes[i].offsetMax.x - textBoxes[i].offsetMin.x);
            float height = Mathf.Abs(textBoxes[i].offsetMax.y - textBoxes[i].offsetMin.y);

            float posX = (axis.majorMarkerList[i].markerPosition) * (axis.stop.x - axis.start.x) + axis.start.x;
            float posY = (axis.majorMarkerList[i].markerPosition) * (axis.stop.y - axis.start.y) + axis.start.y;

            TextMeshProUGUI textMeshProUGUI = textBoxes[i].GetComponent<TextMeshProUGUI>();

            if ((axis.stop.x - axis.start.x) > (axis.stop.y - axis.start.y))
            {   
                textBoxes[i].anchoredPosition = new Vector2(posX * parentWidth, posY * parentHeight-height) + horizontalOffset;
                textMeshProUGUI.alignment = TextAlignmentOptions.Left;
            }   
            else
            {   
                textBoxes[i].anchoredPosition = new Vector2(posX * parentWidth-width, posY * parentHeight - height/2f) + verticalOffset;
                textMeshProUGUI.alignment = TextAlignmentOptions.Center;
            }

            double result = 0;
            //if (unitAdjustement < -20)
            //{
            //    result = 0;
            //}
            //else if (unitAdjustement  > 20)
            //{
            //    result = 0;
            //}   
            //else
            //{
                result = Math.Round( Math.Pow(10f, -unitAdjustement)* ((double)(axis.majorMarkerList[i].markerPosition) * (double)(axis.axisValueMinMax.y - axis.axisValueMinMax.x) + (double)axis.axisValueMinMax.x) * 100.0) / 100.0;
            //}   


            if (!double.IsNaN(result))
            {   
                string str = String.Format("{0:F2}", result);
                textMeshProUGUI.text = str;
            }   
            else
            {   
                textMeshProUGUI.text = "";
            }   


        }


        String unit = "10"+"<sup>"+ unitAdjustement.ToString() + "</sup>";
        return unit;
    }   


}
