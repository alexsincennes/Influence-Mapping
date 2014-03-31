using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GlobalInfluenceMapper : MonoBehaviour
{
	public GameObject unit_obj;
	private Unit unit;

	public Terrain terrain;

	private const int size = 100;
	private float[,] values;
	
	// Use this for initialization
	void Start ()
	{
		unit = unit_obj.GetComponent<Unit>();
		values = new float[size,size];

		GetValues();
		CreateMapTexture();
	}

	// Update is called once per frame
	void Update ()
	{

	}

	void GetValues()
	{
		for(int x = 0 ; x < size; x ++ )
		{
			for(int z = 0; z < size; z++)
			{
				/*
				Vector3 pos = new Vector3(x,0.5f, z);
				float friend_influence_value 	= unit.FriendInfluence(pos);
				float foe_influence_value		= unit.FoeInfluence(pos);
				float influences_value			= unit.TotalInfluence(friend_influence_value, foe_influence_value);
				// scaled for viewability
				float tension_value 			= unit.TotalTension(friend_influence_value, foe_influence_value);
				float vulnerability_value		= unit.TotalVulnerability(tension_value, influences_value);
				float terrain_value 			= unit.TerrainValue(pos);

				values[x,z] = vulnerability_value;
				*/
			}
		}		
	}

	void CreateMapTexture() 
	{	
		Texture2D texture = new Texture2D(size, size);
		
		for(int h=0; h < size; h++) 
		{
			for(int w=0; w < size; w++) 
			{
				
				Color c;
				if (values[w,h] < 0)
					c = new Color(-values[w,h], 0, 0);
				else
					c = new Color(0, values[w,h], 0);

				texture.SetPixel(w,h,c);
			}
		}
		
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply();
		
		MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		mesh_renderer.sharedMaterials[0].mainTexture = texture;

	}
}

