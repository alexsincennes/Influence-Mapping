using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Abstract class with the functionality to render some set of data onto a texture.
/// </summary>
public abstract class MapRenderer : MonoBehaviour
{
	public Terrain terrain;
	protected int size;
	protected float[,] values;

	void Start ()
	{
		size = (int)terrain.terrainData.size.x;
		values = new float[size,size];

		GetValues();
		CreateMapTexture();
	}

	private void CreateMapTexture() 
	{	
		float[] flattened_values = values.Cast<float>().ToArray();

		float max = flattened_values.Max();
		float min = flattened_values.Min();

		Texture2D texture = new Texture2D(size, size);
		
		for(int h=0; h < size; h++) 
		{
			for(int w=0; w < size; w++) 
			{
				
				Color c;
				if (values[w, h] < 0)
					c = new Color(values[w, h]/min, 0, 0);
				else
					c = new Color(0, values[w, h]/max, 0);
				
				texture.SetPixel(w,h,c);
			}
		}
		
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply();
		
		MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		mesh_renderer.sharedMaterials[0].mainTexture = texture;

	}

	protected abstract void GetValues ();
}

