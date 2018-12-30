using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;


//[ExecuteInEditMode]
[RequireComponent(typeof(RawImage))]
public class DataGraph : MonoBehaviour {

    static private Dictionary<int, DataGraph> graphList = new Dictionary<int, DataGraph>();
    /// <summary>
    /// Use this to acces any graph present in the scene.
    /// </summary>
    static public Dictionary<int, DataGraph> GraphtList
    { get { return graphList; } }

    /// <summary>
    /// Use this to acces any curvres on this graph.
    /// </summary>
    public Dictionary<int,Curve> graphCurves = new Dictionary<int, Curve>();

    public string graphName;
    public int height = 640;
    public int width = 640;

    public Color backgroundColor = Color.black;
    public Axis horizontalAxis = new Axis(new Vector2(0.15f, 0.15f), new Vector2(1, 0.15f), new Vector2(0,1), Color.white);
    public Axis verticalAxis = new Axis(new Vector2(0.15f, 0.15f), new Vector2(0.15f, 1), new Vector2(0, 1), Color.white);

    public Texture2D graphTexture = null;
    private Color32[] colorArr;
    
    private RawImage image;
    private Material material;


    ThreadStart threadStart;
    public Thread mapUpdate;

    public float timeUpdatedPeriod = 0.01f;

    public bool autoFitAxis = true;
    public bool pauseTextureUpdate = false;
    public bool pauseDataAppending = false;


    void Start()
    {   
        blockUpdate = false;

        if (graphName == "")
        { graphName = "GraphNumber : " + graphList.Values.Count.ToString(); }

        if(!graphList.ContainsKey(this.GetHashCode()))
        graphList.Add(this.GetHashCode(), this);

        colorArr = new Color32[width * height];

        graphTexture = new Texture2D(width, height);

        graphTexture.name = graphName;

        material = new Material(Shader.Find("UI/Default"));

        image = GetComponent<RawImage>();
        image.uvRect = new Rect(0, 0, 1, 1);
        image.texture = graphTexture;
        image.material = material;

        graphTexture.wrapMode = TextureWrapMode.Clamp;
        Axis.Validate(ref verticalAxis);
        Axis.Validate(ref horizontalAxis);


        threadStart = () => {
            SetBackGroundColor(backgroundColor);
            DrawAxis(ref verticalAxis);
            DrawAxis(ref horizontalAxis);
            foreach (Curve item in graphCurves.Values)
            {   
                if (item.GetCurveData().Count > 1)
                {
                    DrawCurve(item);
                }
            }   
        };  

    }   
    
    private bool blockUpdate = false;
    void Update()
    {
        if (!blockUpdate && !pauseTextureUpdate)
        {
            StartCoroutine(TimedUpdate());
        }
    }

    IEnumerator TimedUpdate()
    {
        blockUpdate = true;
        if (mapUpdate == null || !mapUpdate.IsAlive)
        {   
            if (autoFitAxis)
            {FitAxisToAllData();}
            


            mapUpdate = (new Thread(threadStart));
            mapUpdate.Start();
            
            while (mapUpdate.IsAlive)
            { yield return null; }
            
            UpdateTexture();
            yield return new WaitForSeconds(timeUpdatedPeriod);
            blockUpdate = false;
        }   
    }

    void FitAxisToAllData()
    {   
        verticalAxis.axisValueMinMax.x = float.PositiveInfinity;
        verticalAxis.axisValueMinMax.y = float.NegativeInfinity;
        horizontalAxis.axisValueMinMax.x = float.PositiveInfinity;
        horizontalAxis.axisValueMinMax.y = float.NegativeInfinity;
                
        foreach (Curve item in graphCurves.Values)
        {   
            item.AppendBufferToCurveData();
            FitAxisToCurve(item);
        }
        //AxisRangeLock rangeLockType;
               
        //float verticalBuffer = Mathf.Abs(verticalAxis.axisValueMinMax.x - verticalAxis.axisValueMinMax.y) * 0.05f;

        //verticalAxis.axisValueMinMax.x -= verticalBuffer;
        //verticalAxis.axisValueMinMax.y += verticalBuffer;
    }

    void FitAxisToCurve(Curve curve)
    {
        List<Vector2> curveData = curve.GetCurveData();
        for (int ax = 0; ax < 2; ax++)
        {
            Vector2 range = Vector2.zero;
            range.x = float.PositiveInfinity;
            range.y = float.NegativeInfinity;
            foreach (Vector2 item in curveData)
            {
                float val = (ax == 0)? item.x: item.y;
                if (val < range.x)
                {
                    range.x = val;
                }
                if (val >= range.y)
                {
                    range.y = val;
                }
            }

            Axis currAxis = (ax == 0) ? horizontalAxis:verticalAxis;
            Vector2 hold = currAxis.AxisValueMinMax;

            if (range.x < currAxis.AxisValueMinMax.x)
            { hold.x = range.x; }
            if (range.y >= currAxis.AxisValueMinMax.y )
            { hold.y = range.y; }

            currAxis.AxisValueMinMax = hold;
        }   
    }

    void SetBackGroundColor(Color backgroundColor)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                colorArr[i + j * width] = backgroundColor;
            }
        }
    }
        
    void DrawAxisMarker(Axis axis)
    {   
        if (axis.majorMarker != 0 || axis.minorMarker != 0)
        {   
            const float majorLength = 0.05f;
            Vector2 start = new Vector2(axis.start.x * width, axis.start.y * width);
            Vector2 stop = new Vector2(axis.stop.x * width, axis.stop.y * width);
            float angle = VectorAngle(stop - start);

            
            for (int majorMinor = 0; majorMinor < 2; majorMinor++)
            {   
                bool drawMinor = ((majorMinor+1) == (int)Axis.MarkerType.Type.Minor) ? true : false;
                List<Axis.MarkerType> markerList = drawMinor ? axis.minorMarkerList : axis.majorMarkerList;

                float widthHolder = ((float)axis.lineWidth) / 2f;
                float lengthHolder = majorLength;
                if (drawMinor)
                {
                    widthHolder = widthHolder / 2f;
                    lengthHolder = lengthHolder / 2f;
                }

                foreach (Axis.MarkerType markerItem in markerList)
                {
                    float t = markerItem.markerPosition;
                    Vector2 pos = start + (stop - start) * t;
                    Vector2 markerStart = new Vector2(pos.x / (float)width, pos.y / (float)height);
                    Vector2 markerStop = new Vector2(pos.x / (float)width, pos.y / (float)height + (axis.flipMarkers ? 1 : -1) * lengthHolder);
                    markerStop = RotateVector(markerStop - markerStart, angle) + markerStart;

                    Line axisMarker = new Line(
                        markerStart,
                        markerStop,
                        axis.color,
                        Mathf.RoundToInt(widthHolder));
                    DrawLine(axisMarker);
                }
            }
        }   
    }       
        
    void DrawLine(Line axis)
    {   
        Color colorToSet = axis.color;
        
        Vector2 start = new Vector2(axis.start.x * width, axis.start.y * width);
        Vector2 stop = new Vector2(axis.stop.x * width, axis.stop.y * width);

        const float step = 0.001f;
        float t = 0;

        do
        {
            Vector2 pos = start + (stop - start) * t;
            int xWidth_ = Mathf.RoundToInt(pos.x);
            int yHeight_ = Mathf.RoundToInt(pos.y);

            int pass = 0;
            if (Mathf.Abs(stop.x - start.x) > Mathf.Abs(stop.y - start.y))
            {pass = 1;}   

            for (int border = -axis.lineWidth; border < axis.lineWidth; border++)
            {
                int xWidth = xWidth_ + (pass == 0 ? border : 0); ;
                int yHeight = yHeight_ + (pass == 1 ? border : 0);

                if (yHeight >= 0 && yHeight < height && xWidth >= 0 && xWidth < width)
                {
                    colorArr[xWidth + yHeight * width] = colorToSet;
                }
             }
            t += step;
        } while (t< 1);

    }
    
    void DrawAxis(ref Axis axis)
    {
        Axis.PopulateMarkerList(axis);

        DrawAxisMarker(axis);
        DrawLine(axis);
    }
    
    Vector2 ConverteDataSpaceToPercentageSpace(Vector2 delta, Vector2 startOffset, Vector2 endOffset, Vector2 data)
    {   
        float imagePercentageX = (data.x- startOffset.x) * delta.x + endOffset.x;
        float imagePercentageY = (data.y - startOffset.y) * delta.y + endOffset.y;
        Vector2 result = new Vector2(imagePercentageX, imagePercentageY);
        return result;
    }

    void LinearClampLineExtremumToBox(ref Vector2 point, float lineSlope, float lineOffsets, bool xValid, bool yValid)
    {
        if (xValid && !yValid)
        {   
            float newY = 0;
            if (point.y < Mathf.Min(horizontalAxis.start.y, verticalAxis.stop.y))
            {
                newY = Mathf.Min(horizontalAxis.start.y, verticalAxis.stop.y);
            }
            else
            {
                newY = Mathf.Max(verticalAxis.start.y, verticalAxis.stop.y);
            }

            point = new Vector2((newY - lineOffsets) / lineSlope, newY);
        }
        else if (!xValid && yValid)
        {
            float newX = 0;

            if (point.x < Mathf.Min(horizontalAxis.start.x, horizontalAxis.stop.x))
            { newX = Mathf.Min(horizontalAxis.start.x, horizontalAxis.stop.x); }
            else
            { newX = Mathf.Max(horizontalAxis.start.x, horizontalAxis.stop.x); }
            point = new Vector2(newX, newX * lineSlope + lineOffsets);
        }
    }
    
    void ClampLineExtremumToBox(ref Vector2 point,float lineSlope , float lineOffsets,bool xValid, bool yValid)
    {
        LinearClampLineExtremumToBox(ref point, lineSlope, lineOffsets, xValid, yValid);
    }   
    
    void DrawCurve(Curve curve)
    {   
        // converte from data space to graph percentage
        float deltaX = ((horizontalAxis.stop.x - horizontalAxis.start.x)/(horizontalAxis.axisValueMinMax.y - horizontalAxis.axisValueMinMax.x));
        float deltaY = ((verticalAxis.stop.y - verticalAxis.start.y)/(verticalAxis.axisValueMinMax.y - verticalAxis.axisValueMinMax.x));

        Vector2 scaleDelta = new Vector2(deltaX, deltaY);
        Vector2 endOffset = new Vector2(Mathf.Min(horizontalAxis.start.x, horizontalAxis.stop.x), Mathf.Min(verticalAxis.start.y, verticalAxis.stop.y));
        Vector2 startOffset = new Vector2(Mathf.Min(horizontalAxis.axisValueMinMax.x, horizontalAxis.axisValueMinMax.x), Mathf.Min(verticalAxis.axisValueMinMax.x, verticalAxis.axisValueMinMax.x));
        
        List<Vector2> dotLocation = curve.GetCurveData();
        Color color = curve.GetCurveColor();

        for (int i = 0; i < dotLocation.Count-1; i++)
        {   

            Vector2 firstPoint = ConverteDataSpaceToPercentageSpace(scaleDelta, startOffset, endOffset, dotLocation[i] );
            Vector2 secondPoint = ConverteDataSpaceToPercentageSpace(scaleDelta, startOffset, endOffset, dotLocation[i + 1] );

            bool isFirstValid_x = false;
            bool isFirstValid_y = false;
            bool isSecondValid_x = false;
            bool isSecondValid_y = false;

            bool areBothPointsInTextureRange = (secondPoint.x <= 1 && secondPoint.x >= 0 && secondPoint.y <= 1 && secondPoint.y >= 0) && (firstPoint.x <= 1 && firstPoint.x >= 0 && firstPoint.y <= 1 && firstPoint.y >= 0);

            if (areBothPointsInTextureRange)
            {


                isFirstValid_x = (firstPoint.x >= horizontalAxis.start.x) ^ (firstPoint.x >= horizontalAxis.stop.x);
                isFirstValid_y = ((firstPoint.y >= verticalAxis.start.y) ^ (firstPoint.y >= verticalAxis.stop.y) );
                isSecondValid_x = (secondPoint.x >= horizontalAxis.start.x) ^ (secondPoint.x >= horizontalAxis.stop.x);
                isSecondValid_y = ((secondPoint.y >= verticalAxis.start.y) ^ (secondPoint.y >= verticalAxis.stop.y));

                if (((isFirstValid_x && isFirstValid_y) || (isSecondValid_x && isSecondValid_y)) || (isFirstValid_x ^ isSecondValid_x) && (isFirstValid_y ^ isSecondValid_y))
                {   
                    float m = (secondPoint.y - firstPoint.y) / (secondPoint.x - firstPoint.x);
                    float b = secondPoint.y - m * secondPoint.x;

                    bool isDiagonal = (isSecondValid_x != isSecondValid_y) && (isFirstValid_x != isSecondValid_x);
                    if (m > 0 || !isDiagonal || (isDiagonal && b > Mathf.Min(verticalAxis.stop.y, verticalAxis.start.y))    )
                    {
                        ClampLineExtremumToBox(ref firstPoint, m, b, isFirstValid_x, isFirstValid_y);
                        ClampLineExtremumToBox(ref secondPoint, m, b, isSecondValid_x, isSecondValid_y);

                        Line line = new Line(firstPoint, secondPoint, color, 2);
                        DrawLine(line);
                    }
                }   
            }
        }   



    }
    
    void UpdateTexture()
    {   
        graphTexture.SetPixels32(colorArr);
        graphTexture.Apply();
    }


    public int AddDataSeries(List<Vector2> data, Color color, string curveName)
    {
        
        Curve curve = new Curve(data, color, curveName);
        graphCurves.Add(curve.GraphID, curve);
        return curve.GraphID;
        
        //if (data.Count >= 2)
        //FitAxisToCurve(curve);
    }
    public void AddPointToExistingSeries(int curveID, Vector2 vect)
    {   
        if (!pauseDataAppending)
            graphCurves[curveID].GetCurveDataBuffer().Add(vect);
    }

    [System.Serializable]
    public class Line
    {
        public int lineWidth;
        /// <summary>
        /// The start location of the line expressed as the percentage of the texture
        /// </summary>
        public Vector2 start;
        /// <summary>
        /// The end location of the line expressed as the percentage of the texture
        /// </summary>
        public Vector2 stop;
        public Color color;

        public Line()
        {
            start = Vector2.zero;
            stop = Vector2.one;
            color = Color.red;
            lineWidth = 0;
        }

        public Line(Vector2 start,Vector2 stop,Color color,int lineWidth)
        {
            this.lineWidth = lineWidth;
            this.start = start;
            this.stop = stop;
            this.color = color;
        }
        
    }   
    
    [System.Serializable]
    public class Axis: Line
    {
        /// <summary>
        /// Change the side of theline the markers are drawn.
        /// </summary>
        public bool flipMarkers;
        /// <summary>
        /// Number of major markers
        /// </summary>
        public int majorMarker;
        /// <summary>
        /// Number of minor markers between the major markers.
        /// </summary>
        public int minorMarker;

        public enum AxisRangeLock { FREE = 0, MAX_RANGE = 1, MIN_RANGE = 2, MAX_VALUE = 3, MIN_VALUE = 4 }
        public AxisRangeLock rangeLockType = AxisRangeLock.FREE;

        public Vector2 axisValueMinMax;
        public float axisValueMinMaxCapRange = 1;
        public float axisValueMinMaxCapValue = 0;
        public float axisValueBuffer = 0.05f;

        public Vector2 AxisValueMinMax
        {
            get
            {
                return axisValueMinMax;
            }
            set
            {   
                axisValueMinMax = value;
                if (rangeLockType == AxisRangeLock.FREE)
                {

                }
                else if (rangeLockType == AxisRangeLock.MAX_RANGE)
                {
                    if (axisValueMinMaxCapRange < Mathf.Abs(axisValueMinMax.y - axisValueMinMax.x))
                    {axisValueMinMax.x = axisValueMinMax.y - axisValueMinMaxCapRange;}
                }
                else if (rangeLockType == AxisRangeLock.MIN_RANGE)
                {
                    if (axisValueMinMaxCapRange < Mathf.Abs(axisValueMinMax.y - axisValueMinMax.x))
                    axisValueMinMax.y = axisValueMinMax.x + axisValueMinMaxCapRange;
                }
                else if (rangeLockType == AxisRangeLock.MAX_VALUE)
                {
                    axisValueMinMax.y = axisValueMinMaxCapValue;
                }
                else if (rangeLockType == AxisRangeLock.MIN_VALUE)
                {
                    axisValueMinMax.x = axisValueMinMaxCapValue;
                }
            }   
        }
        
        public List<MarkerType> majorMarkerList = new List<MarkerType>();
        public List<MarkerType> minorMarkerList = new List<MarkerType>();

        public Axis()
        {
            majorMarker = 0;
            minorMarker = 0;
            flipMarkers = false;
            axisValueMinMax = new Vector2(0, 1);

            axisValueMinMaxCapRange = 1;
            axisValueMinMaxCapValue = 0;
            majorMarkerList.Clear();
            minorMarkerList.Clear();
        }

        public Axis(Vector2 start, Vector2 stop,Vector2 axisMinMax, Color color, int LineWidth = 4, bool flipMarkers = false, float majorMarker = 10, float minorMarker = 5)
        {   
            ClampVector01(ref start);
            ClampVector01(ref stop);
            
            this.start = start;
            this.stop = stop;
            
            this.lineWidth = LineWidth;
            this.color = color;
            this.majorMarker = (int)Mathf.Clamp((float)majorMarker,0f,10f);
            this.minorMarker = (int)Mathf.Clamp((float)minorMarker,0f,10f);
            this.flipMarkers = flipMarkers;
            this.axisValueMinMaxCapRange = Mathf.Abs( axisValueMinMax.y - axisValueMinMax.x);
            this.axisValueMinMaxCapValue = 0;

            PopulateMarkerList(this);

        }   
        
        static public void Validate(ref Axis axis)
        {   
            ClampVector01(ref axis.start);
            ClampVector01(ref axis.stop);

            if (axis.start.x > axis.stop.x)
            {
                float val = axis.start.x;
                axis.start.x = axis.stop.x;
                axis.stop.x = val;
            }
            if (axis.start.y > axis.stop.y)
            {
                float val = axis.start.y;
                axis.start.y = axis.stop.y;
                axis.stop.y = val;
            }

            axis.majorMarker = (int)Mathf.Clamp((float)axis.majorMarker, 0f, 10f);
            axis.minorMarker = (int)Mathf.Clamp((float)axis.minorMarker, 0f, 10f);



        }
        static public void PopulateMarkerList(Axis axis)
        {
            axis.majorMarkerList.Clear();
            axis.minorMarkerList.Clear();

            float majorMarkerStep = axis.majorMarker == 0 ? 0 : 1f / (float)(axis.majorMarker);
            float minorMarkerStep = axis.minorMarker == 0 ? 0 : majorMarkerStep / (float)(axis.minorMarker);
            for (int majorCount = 0; majorCount <= axis.majorMarker; majorCount++)
            {
                MarkerType marker = new MarkerType(majorMarkerStep * majorCount, MarkerType.Type.Major);
                axis.majorMarkerList.Add(marker);
                if(majorCount != axis.majorMarker)
                for (int minorCount = 0; minorCount <= axis.minorMarker; minorCount++)
                {
                    marker = new MarkerType(minorMarkerStep * (minorCount) + majorMarkerStep * majorCount, MarkerType.Type.Minor);
                    axis.minorMarkerList.Add(marker);
                }
            }
        }

        [System.Serializable]
        public class MarkerType
        {
            public enum Type { Major = 1, Minor = 2 };

            public Type markerType;
            public float markerPosition;

            public MarkerType(float markerPosition, Type markerType)
            {
                this.markerType = markerType;
                this.markerPosition = markerPosition;
            }
        }
    }

    [System.Serializable]
    public class Curve
    {
        public string curveName;
        int graphID;
        public int GraphID{get{return graphID;}}

        List<Vector2> curveData;
        List<Vector2> curveDataBuffer;
        Color curveColor;


        public List<Vector2> GetCurveDataBuffer()
        { return curveDataBuffer; }

        public List<Vector2> GetCurveData()
        {return curveData;}

        public Color GetCurveColor()
        {return curveColor;}        
        public Curve(List<Vector2> curveData, Color curveColor, string curveName)
        {
            curveDataBuffer = new List<Vector2>();
            curveDataBuffer.Clear();

            this.graphID = curveName.GetHashCode();
            this.curveName = curveName;
            this.curveData = curveData;
            this.curveColor = curveColor;

        }

        public void AppendBufferToCurveData()
        {   
            curveData.AddRange(curveDataBuffer);
            curveDataBuffer.Clear();
        }   
    }
    
    private void OnValidate()
    {
#if INLUDE_EXAMPLE_CURVE
        TestCurve();
#endif
    }
    
    private static void ClampVector01(ref Vector2 vector)
    {
        if (vector.x > 1)
            vector.x = 1;
        else if (vector.x < 0)
            vector.x = 0;
        if (vector.y > 1)
            vector.y = 1;        
        else if (vector.y < 0)
            vector.y = 0;
    }
    private static Vector2 RotateVector(Vector2 direction, float angle)
    {
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);

        float res1 = direction.x * cos - direction.y * sin;
        float res2 = direction.x * sin + direction.y * cos;

        Vector2 result = new Vector2(res1, res2);
        return result;
    }
    private static float VectorAngle(Vector2 direction)
    {
        float theta = 0;
        if (direction.x > 0)
        {
            if (direction.y >= 0)
            { theta = Mathf.Atan(direction.y / direction.x); }
            else
            { theta = Mathf.Abs(Mathf.Atan(direction.x / direction.y)) + Mathf.PI * 3f / 2f; }
        }
        else if (direction.x < 0)
        {
            if (direction.y >= 0)
            { theta = Mathf.Abs(Mathf.Atan(direction.x / direction.y)) + Mathf.PI / 2f; }
            else
            { theta = Mathf.Abs(Mathf.Atan(direction.y / direction.x)) + Mathf.PI; }
        }
        else
        {
            theta = Mathf.Sign(direction.y) * Mathf.PI / 2f;
        }

        return theta;
    }


}
