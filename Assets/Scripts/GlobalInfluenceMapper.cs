using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GlobalInfluenceMapper : MonoBehaviour
{
	public Unit target;
	public Terrain terrain;
	
	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}
	
	void CreateMapTexture() 
	{	
		/*
		int texSide = terrain.terrainData.size * terrain.terrainData.size;
		Texture2D texture = new Texture2D(texSide, texSide);
		
		for(int h=0; h < texSide; h++) 
		{
			for(int w=0; w < texSide; w++) 
			{
				Color[] c =  tiles_array[ (int)GetTextureValueAt(w,h) ];
				texture.SetPixels(w*tile_resolution, h*tile_resolution, tile_resolution, tile_resolution, c);
			}
		}
		
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply();
		
		MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		mesh_renderer.sharedMaterials[0].mainTexture = texture;
		*/
	}
}

