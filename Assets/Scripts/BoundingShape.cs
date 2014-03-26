using UnityEngine;
using System.Collections;

public class BoundingShape : MonoBehaviour
{

	// bounding data
	public enum BOUNDING_SHAPE {RECTANGULAR, CIRCULAR};
	public BOUNDING_SHAPE shape;
	// vector to corner of rectangle, or point on circle
	public Vector2 shape_corner;

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}
}

