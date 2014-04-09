using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour
{
	public float radius = 1.0f;
		
	// Use this for initialization
	void Start ()
	{
		this.gameObject.collider.transform.localScale = new Vector3(radius, radius, radius);
	}

	// Update is called once per frame
	void Update ()
	{

	}
}

