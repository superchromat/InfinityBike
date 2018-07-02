using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class TerrainCustomEditor : MonoBehaviour {
	public TerrainData terrainData; 

	public float mapScale = 1f; 

	//Procedural map Generation
	public float ground = 0; 
	public float mountain = 0.5f; 
	public float top = 1; 
	public float perlinAmplitude =1f; 

	public BezierSpline bezierTrack; 




	void Awake() {
		terrainData = GetComponent<Terrain> ().terrainData;

	}
	// Use this for initialization
	public void MapHeight() {
		Debug.Log ("HeightMapResolution");
		Debug.Log (terrainData.heightmapResolution);
		float[,,] alphaMap = new float[terrainData.alphamapResolution, terrainData.alphamapResolution, terrainData.alphamapLayers];
		float[,] newMap = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
		for( int h = 0; h < terrainData.heightmapResolution ; h++) {
			for (int v = 0; v < terrainData.heightmapResolution; v ++ ) {
				newMap[h,v] = Mathf.PerlinNoise((float)h/terrainData.heightmapResolution*mapScale,(float)v/terrainData.heightmapResolution*mapScale)*perlinAmplitude;

			}
	
		}
		for( int h = 0; h < terrainData.alphamapResolution ; h++) {
			for (int v = 0; v < terrainData.alphamapResolution; v ++ ) {
				
				//Ground
				if (newMap [h, v] < 0.25f) {
					alphaMap [h, v, 0] = 1; 
					alphaMap [h, v, 1] = 0; 
					alphaMap [h, v, 2] = 0; 
				} else if (newMap [h, v] >= 0.25f && newMap [h, v] < 0.75f) {
					alphaMap [h, v, 0] = 0; 
					alphaMap [h, v, 1] = 1; 
					alphaMap [h, v, 2] = 0; 
					
				} else {
					alphaMap [h, v, 0] = 0; 
					alphaMap [h, v, 1] = 0; 
					alphaMap [h, v, 2] = 1; 
				}
				//Middle 
				//Top 


			}

		}

		terrainData.SetHeights (0, 0, newMap); 
		terrainData.SetAlphamaps (0, 0, alphaMap);

	}

	public void ApplySquareRectTrans() {
		
	}



	public void ApplyCircleAlphaTrans(Vector2 worldCenter,float worldRadius,int alphaMapsIndex) {
		//Vector2 worldCenter = new Vector2 (200, 200);

		int[] pixCenter = WorldToTerrainPix(worldCenter,true); 
		int pixRadius = (int) (worldRadius / terrainData.alphamapResolution * terrainData.size.x); //WARNING: This only works with square worlds


		int[] lowerLeftPix = WorldToTerrainPix (worldCenter + new Vector2 (-1, -1) * worldRadius,true); 
		int[] upperRightPix = WorldToTerrainPix (worldCenter + new Vector2 (1, 1) * worldRadius,true); 

		int rectangleWidth = upperRightPix [0] - lowerLeftPix [0];
		int rectangleLenght = upperRightPix [1] - lowerLeftPix [1]; 

		float[,,] alphamaps = terrainData.GetAlphamaps (lowerLeftPix [0], lowerLeftPix [1], rectangleWidth, rectangleLenght);//new float[rectangleWidth,rectangleLenght]; //A square to contain the transformation process

		for (int h = 0; h < rectangleWidth  ; h ++) {
			int xPix = lowerLeftPix [0] + h;
			for (int v = 0; v < rectangleLenght ; v ++) {
				int yPix = lowerLeftPix [1] + v; 

				//Check if its inside the circle 
				if ((xPix - pixCenter [0]) * (xPix - pixCenter [0]) + (yPix - pixCenter [1]) * (yPix - pixCenter [1]) <= pixRadius * pixRadius) {
					alphamaps [v, h,2] = 1; //Very weird (the matrix is stocked y first and then x). 
				} else {
					//Do something else. 
				}
			}
		}
		terrainData.SetAlphamaps (lowerLeftPix[0], lowerLeftPix [1], alphamaps);
	}


	public Vector2 TerrainPixToWorld(int xPix, int yPix){
		if (xPix < 0 || xPix > (terrainData.heightmapResolution - 1) || yPix < 0 || yPix > terrainData.heightmapResolution)
		{
			return new Vector2 (0, 0); 
		}
		//Warning : This function will only work if the terrain has no rotation.
		Vector3 terrainOrigin = this.transform.position;
		float terrainWidth = terrainData.size.x;
		float terrainLenght = terrainData.size.z; 

		float xCoord = terrainOrigin.x + (float) xPix / terrainData.heightmapResolution * terrainWidth; 
		float yCoord = terrainOrigin.z + (float) yPix / terrainData.heightmapResolution * terrainLenght;
		return new Vector2(xCoord,yCoord);
	}

	//This function calculates the closest pixel associated to height map or alpha map from a world coordinates. If the coordinate is outsite of the terrain, it will return the bounds values. 
	//Warning : This function will only work if the terrain has no rotation.
	public int[] WorldToTerrainPix(Vector2 world, bool alphaMap = false) {
		

		int[] i = new int[2];
		int resolution; 
		if (alphaMap) {
			resolution = terrainData.alphamapResolution;
		} else {
			resolution = terrainData.heightmapResolution;
		}
		Vector3 terrainOrigin = this.transform.position; 
		float terrainWidth = terrainData.size.x; 
		float terrainLenght = terrainData.size.z; 

		i[0] = (int) ((world.x - terrainOrigin.x) / terrainWidth * resolution); 
		i[1] = (int) ((world.y - terrainOrigin.z) / terrainLenght * resolution); 

		//Return bounds if the calculated value is out of bound. 
		if (i [0] < 0) {
			i [0] = 0;
		}
		if (i [0] > resolution - 1) {
			i [0] = resolution-1;
		}
		if (i [1] < 0) {
			i [1] = 0;
		}
		if (i [1] > resolution - 1) {
			i [1] = resolution-1;
		}

		return i; 
		
	}
	public float WorldToTerrainHeight(Vector2 world) {
		int[] i = WorldToTerrainPix (world); 
		float height = terrainData.GetHeight (i [0], i [1]); 
		return height;
	}

	public void GenerateTrackFromBezier(int steps, float width) {//ProblyDelete
		float t = 0; 
		float increment = 1f / (float)steps; 
		for (int i = 0; i < steps; i++) {
			
			Vector3 bezierPoint = bezierTrack.GetPoint (t);
			ApplyCircleAlphaTrans (new Vector2(bezierPoint.x,bezierPoint.z), width,1);
			ApplyCircleHeightTrans (new Vector2 (bezierPoint.x, bezierPoint.z), width,0.25f);
			t = t + increment ;
		}
	}
	/// <summary>
	/// Applies map circle transform a the position worldCenter and a radius worldRadius.
	/// </summary>
	/// <param name="worldCenter">World center.</param>
	/// <param name="worldRadius">World radius.</param>
	/// <param name="height">Height.</param>
	public void ApplyCircleHeightTrans(Vector2 worldCenter, float worldRadius, float worldHeight) {
		//Vector2 worldCenter = new Vector2 (200, 200);

		int[] pixCenter = WorldToTerrainPix(worldCenter); 
		int pixRadius = (int) (worldRadius / terrainData.heightmapResolution * terrainData.size.x); //WARNING: This only works with square worlds


		int[] lowerLeftPix = WorldToTerrainPix (worldCenter + new Vector2 (-1, -1) * worldRadius); 
		int[] upperRightPix = WorldToTerrainPix (worldCenter + new Vector2 (1, 1) * worldRadius); 

		int rectangleWidth = upperRightPix [0] - lowerLeftPix [0];
		int rectangleLenght = upperRightPix [1] - lowerLeftPix [1]; 

		float[,] heights = terrainData.GetHeights (lowerLeftPix [0], lowerLeftPix [1], rectangleWidth, rectangleLenght);//new float[rectangleWidth,rectangleLenght]; //A square to contain the transformation process

		for (int h = 0; h < rectangleWidth  ; h ++) {
			int xPix = lowerLeftPix [0] + h;
			for (int v = 0; v < rectangleLenght ; v ++) {
				int yPix = lowerLeftPix [1] + v; 

				//Check if its inside the circle 
				if ((xPix - pixCenter [0]) * (xPix - pixCenter [0]) + (yPix - pixCenter [1]) * (yPix - pixCenter [1]) <= pixRadius * pixRadius) {
					heights [v, h] = worldHeight/(float) terrainData.heightmapHeight; //Very weird (the matrix is stocked y first and then x). 
				} else {
					//Do something else. 
				}
			}
		}
		terrainData.SetHeights (lowerLeftPix[0], lowerLeftPix [1], heights);
	}

	public void ApplyRadialHeightBlend(Vector2 worldCenter2D,float worldRadius, float worldHeight, float gaussianFactor) {
		int[] pixCenter = WorldToTerrainPix(worldCenter2D); 
		int pixRadius = (int) (worldRadius / terrainData.heightmapResolution * terrainData.size.x); //WARNING: This only works with square worlds
		float terrainHeight = terrainData.size.y; 

		int[] lowerLeftPix = WorldToTerrainPix (worldCenter2D + new Vector2 (-1, -1) * worldRadius); 
		int[] upperRightPix = WorldToTerrainPix (worldCenter2D + new Vector2 (1, 1) * worldRadius); 

		int rectangleWidth = upperRightPix [0] - lowerLeftPix [0];
		int rectangleLenght = upperRightPix [1] - lowerLeftPix [1]; 

		float[,] heights = terrainData.GetHeights (lowerLeftPix [0], lowerLeftPix [1], rectangleWidth, rectangleLenght);//new float[rectangleWidth,rectangleLenght]; //A square to contain the transformation process
		float[,] newHeights = new float[heights.GetLength(0),heights.GetLength(1)];
		for (int h = 0; h < rectangleWidth  ; h ++) {
			int xPix = lowerLeftPix [0] + h;
			for (int v = 0; v < rectangleLenght ; v ++) {
				int yPix = lowerLeftPix [1] + v; 

				//Check if its inside the circle
				float pixCenterDist = Mathf.Sqrt(((int)(pixCenter[0]-xPix)*(pixCenter[0]-xPix) + (pixCenter[1]-yPix)*(pixCenter[1]-yPix)));
				float alpha; 
				if (pixCenterDist == 0) {
					alpha = 1f; 
				} else {
					alpha = Mathf.Exp(-gaussianFactor*((float)pixCenterDist/(float)pixRadius) *((float)pixCenterDist/(float)pixRadius));//Clamp01((float)pixRadius/(float)pixCenterDist); 
				}

				newHeights [v, h] = (1f - alpha) * heights [v, h] + alpha * worldHeight/((float)terrainHeight); //Very weird (the matrix is stocked y first and then x). 

	
				}
			}

		terrainData.SetHeights (lowerLeftPix[0], lowerLeftPix [1], newHeights);
	}

	public void ApplyRadialAlphaBlend(Vector2 worldCenter2D,float worldRadius,float gaussianFactor, int alphaMapIndex) {
		int[] pixCenter = WorldToTerrainPix(worldCenter2D,true); //the true bool is associated with the fact it's an alphamap 
		int pixRadius = (int) (worldRadius / terrainData.alphamapResolution * terrainData.size.x); //WARNING: This only works with square worlds
		//float terrainHeight = terrainData.size.y; 

		int[] lowerLeftPix = WorldToTerrainPix (worldCenter2D + new Vector2 (-1, -1) * worldRadius); 
		int[] upperRightPix = WorldToTerrainPix (worldCenter2D + new Vector2 (1, 1) * worldRadius); 

		int rectangleWidth = upperRightPix [0] - lowerLeftPix [0];
		int rectangleLenght = upperRightPix [1] - lowerLeftPix [1]; 

		float[,,] alphas = terrainData.GetAlphamaps (lowerLeftPix [0], lowerLeftPix [1], rectangleWidth, rectangleLenght);//new float[rectangleWidth,rectangleLenght]; //A square to contain the transformation process
		float[,,] newAlphas = new float[alphas.GetLength(0),alphas.GetLength(1),alphas.GetLength(2)];
		newAlphas = alphas; 
		for (int h = 0; h < rectangleWidth  ; h ++) {
			int xPix = lowerLeftPix [0] + h;
			for (int v = 0; v < rectangleLenght ; v ++) {
				int yPix = lowerLeftPix [1] + v; 

				//Check if its inside the circle
				float pixCenterDist = Mathf.Sqrt(((int)(pixCenter[0]-xPix)*(pixCenter[0]-xPix) + (pixCenter[1]-yPix)*(pixCenter[1]-yPix)));
				float alpha; 
				if (pixCenterDist == 0) {
					alpha = 1f; 
				} else {
					alpha = Mathf.Exp(-gaussianFactor*((float)pixCenterDist/(float)pixRadius) *((float)pixCenterDist/(float)pixRadius));//Clamp01((float)pixRadius/(float)pixCenterDist); 
				}

				newAlphas [v, h, alphaMapIndex] = (1-alpha)* newAlphas [v, h, alphaMapIndex] + alpha;//(1f - alpha) * alphas [v, h,alphaMapIndex] + alpha ; //Very weird (the matrix is stocked y first and then x). 


			}
		}

		terrainData.SetAlphamaps (lowerLeftPix[0], lowerLeftPix [1], newAlphas);


	}

}


