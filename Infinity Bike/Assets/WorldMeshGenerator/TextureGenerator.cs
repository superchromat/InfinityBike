using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour 
{
	public PerlinGenerator pGenerator;

	private Color[] TransformDataToColor()
	{
		Color[] colorArr = new Color[pGenerator.meshDimensions * pGenerator.meshDimensions];
		PerlinGenerator.PerlinData data = pGenerator.GenerateDataValue (Vector2.zero);

		for (int x = 0; x < pGenerator.meshDimensions ; ++x) 
		{		
			for (int y = 0; y < pGenerator.meshDimensions ; ++y) 
			{	
				colorArr [x+pGenerator.meshDimensions*y] = new Color (data.GetData (x, y),0, 0, 0);
			}	
		}		

		return colorArr;
	}

	public Texture2D SetTexture()
	{
		Texture2D texture = new Texture2D(pGenerator.meshDimensions,pGenerator.meshDimensions);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (TransformDataToColor ());
		texture.Apply();


		return texture;
	}


	private MeshRenderer mr;
	void Start()
	{
		mr = GetComponent<MeshRenderer> ();
		Texture2D tex = SetTexture ();

		mr.material.mainTexture = tex;
	}



	void update()
	{
		Texture2D tex = SetTexture ();
		mr.material.mainTexture = tex;


	}



}
