 using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TerrainManager : MonoBehaviour
{
	public Texture2D map; // RED is the height index
	public int[,] height_array;
	
	public int sea_level_height = 10;
	public float tile_size = 1.0f;
	public float slope_size = 1.0f;
	public float height_scale = 1.0f;
	
	private int _width;
	private int _height;
	
	private int _w_face_num;
	private int _h_face_num;
	
	public Texture2D tiles_strip;
	private enum TILES {GRASS, MOUNTAIN, WATER};
	private int tile_resolution;
	private Color[][] tiles_array;
	
	void Start ()
	{
		ConvertTiles();
		CreateHeightArray();
		CreateMap();
		CreateMapTexture();
	}

	void Update ()
	{

	}
	
	/// <summary>
	/// Gets texture index value of face at (w,h).
	/// </summary>
	/// <returns>The texture index.<see cref="System.Int32"/>.</returns>
	/// <param name="face_x">w coord of the face.</param>
	/// <param name="face_y">h coord of the face.</param>
	TILES GetTextureValueAt(int face_w, int face_h)
	{
		Vector3[] verts = GetFaceVerts(face_w, face_h);
	
		// case 1: if all neighbouring vertices are below sea level
		// use water tile (index 2)
		// case 2: if the face is sloped (jump of y=1)
		// default case: use grass tile
		
		bool water =  true;
		bool mountain = false;
		foreach(Vector3 v in verts)
		{
			// this is POST height converstion (i.e. water level is at 0)
			if(v.y > 0)
				water = false;
			
			foreach(Vector3 v_again in verts)
			{
				// if sufficiently sloped
				if(Mathf.Abs(v.y - v_again.y) >= slope_size * height_scale)
					mountain = true;
			}
		}
		
		if(water)
			return TILES.WATER;
		if(mountain)
				return TILES.MOUNTAIN;
		
		return TILES.GRASS;	
	}
	
	Vector3[] GetFaceVerts(int face_w, int face_h)
	{
		Vector3[] verts = new Vector3[4];
		
		MeshFilter mesh_filter = GetComponent<MeshFilter>();
		
		verts[0] = mesh_filter.sharedMesh.vertices[face_h * _width + face_w];
		verts[1] = mesh_filter.sharedMesh.vertices[face_h * _width + (face_w + 1)];
		verts[2] = mesh_filter.sharedMesh.vertices[(face_h+1) * _width + face_w];
		verts[3] = mesh_filter.sharedMesh.vertices[(face_h+1) * _width + (face_w + 1)];
		
		return verts;
	}
	
	/// <summary>
	/// Populates 2d array of height data from
	/// flattened array of pixel values.
	/// We use RED as our height index.
	/// </summary>
	void CreateHeightArray()
	{
		_width = map.width;
		_height = map.height;
		
		height_array = new int[_width, _height];
		Color[] pixels = map.GetPixels();
		
		for(int w = 0; w < _width; w++)
		{
			for(int h = 0; h < _height; h++)
			{
				height_array[w,h] = (int)(pixels[ w +_width *h ].r * 255);
			}
		}
	}
	
	/// <summary>
	/// Creates the mesh & collider for the map
	/// based on the heightmap.
	/// </summary>
	void CreateMap()
	{
		// #faces = #vertices -1
		_w_face_num = _width -1;
		_h_face_num = _height -1;
		
		// #triangles = #faces wide * #faces wide * 2 triangles/face
	 	int _triangle_num = _w_face_num * _h_face_num * 2;
		
		int _vertice_num = _width * _height;
		
		Vector3[] vertices = new Vector3[ _vertice_num ];
		Vector3[] normals = new Vector3[_vertice_num];
		Vector2[] uv = new Vector2[_vertice_num];
		
		int[] triangles = new int[ _triangle_num * 3 ];
		
		for(int h = 0; h < _height; h++) 
		{
			for(int w = 0; w < _width; w++) 
			{
				vertices[ h * _width + w ] = new Vector3( w*tile_size, (height_array[w,h] - sea_level_height) * height_scale, h*tile_size );
				normals	[ h * _width + w ] = Vector3.up;
				uv		[ h * _width + w ] = new Vector2( (float)w / _w_face_num, (float)h / _h_face_num );
			}
		}
		
		for(int h = 0; h < _h_face_num; h++) 
		{
			for(int w = 0; w < _w_face_num; w++) 
			{
				int squareIndex = h * _w_face_num + w;
				int triOffset = squareIndex * 6;
				triangles[triOffset + 0] = h * _width + w + 		   	0;
				triangles[triOffset + 1] = h * _width + w + _width + 	0;
				triangles[triOffset + 2] = h * _width + w + _width + 	1;
				
				triangles[triOffset + 3] = h * _width + w + 		   	0;
				triangles[triOffset + 4] = h * _width + w + _width + 	1;
				triangles[triOffset + 5] = h * _width + w + 		   	1;
			}
		}
		
		
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
		
		MeshFilter mesh_filter = GetComponent<MeshFilter>();
		MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		MeshCollider mesh_collider = GetComponent<MeshCollider>();
		
		mesh_filter.mesh = mesh;
		mesh_collider.sharedMesh = mesh;
	}
	
	/// <summary>
	/// Converts the tile strip into a convenient array structure.
	/// </summary>
	void ConvertTiles() 
	{
		// e.g. 96/32 = 3, here assuming tiles are square
		tile_resolution = tiles_strip.height;
		int num_tiles = tiles_strip.width / tile_resolution;
		
		tiles_array = new Color[num_tiles][];
		
		for(int i=0; i<num_tiles; i++) 
		{
			tiles_array[i] = tiles_strip.GetPixels( i*tile_resolution , 0, tile_resolution, tile_resolution );
		}
	}
	
	/// <summary>
	/// Creates a large texture for the whole of the map
	/// by combining tiles together.
	/// </summary>
	void CreateMapTexture() 
	{	
		int texWidth = _w_face_num * tile_resolution;
		int texHeight = _h_face_num * tile_resolution;
		Texture2D texture = new Texture2D(texWidth, texHeight);
		
		for(int h=0; h < _h_face_num; h++) 
		{
			for(int w=0; w < _w_face_num; w++) 
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
	}
}

