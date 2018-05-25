using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PerlinGenerator", menuName = "Generator/PerlinData",order = 1)]
public class PerlinGenerator : ScriptableObject {

	public int meshDimensions = 512;

	public int octave = 1;
	public float persistance = 0.5f;
	public float amplitude = 1;
	public float frequency = 1;
	public float freqOctMult = 1;

	private float GetPerlinNoiseValue(float x, float y)
	{
		float value = 0;
		float tempAmplitude = amplitude;
		float tempFrequency = frequency;

		for (int oct = 0; oct < octave; ++oct) 
		{	
			value += Mathf.PerlinNoise (x * tempFrequency , y * tempFrequency) * tempAmplitude;
			
			tempAmplitude *= persistance;
			tempFrequency *= freqOctMult;
		}	

		return value;
	}

	public PerlinData GenerateDataValue(Vector2 offSet)
	{	
		PerlinData meshData = new PerlinData (meshDimensions);
		float maxValue = float.MinValue;
		
		for (int x = 0; x < meshDimensions; ++x) 
		{
			for (int y = 0; y < meshDimensions; ++y) 
			{	
				float val = GetPerlinNoiseValue ((float)x + offSet.x , y + (float)offSet.y);
				meshData.SetData (x, y,val);

				if (val > maxValue) 
				{
					maxValue = val;
				}
			}	
		}
		Debug.Log (maxValue);
		return meshData;
	}	

	public class PerlinData
	{
		private float[] data;
		private int dim;

		public PerlinData(int dim)
		{
			data = new float[dim*dim];
			this.dim = dim;
		}

		public float GetData(int x,int y)
		{return data[x+dim*y];}

		public void SetData(int x, int y , float value)
		{data [x + dim * y] = value;}



	}	


	void OnValidate()
	{
		if (persistance > 1) {persistance = 1;}
		else if (persistance < 0) {persistance = 0;}

		if (freqOctMult < 0) {freqOctMult = 0;}

	}

}
