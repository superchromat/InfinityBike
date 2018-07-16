using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FormatUIAnchor : MonoBehaviour
{   
    public float width = 0.1f;
    public float height = 0.05f;
    public Vector2 centerPosition = new Vector2(0.5f,0);
    RectTransform objRect;

    private void Start()
    {
        GetRectTran();
    }
    
    void GetRectTran()
    {
        objRect = GetComponent<RectTransform>();
    }

    void PlaceAnchors()
    {   
        objRect.anchorMin = centerPosition - new Vector2(width/2, height/2); ;
        objRect.anchorMax = centerPosition + new Vector2(width/2,height/2);

        objRect.anchoredPosition = new Vector2(0, 0);

    }      

    private void Update()
    {
        PlaceAnchors();
    }


    private void OnValidate()
    {
        objRect = GetComponent<RectTransform>();

    }





}
