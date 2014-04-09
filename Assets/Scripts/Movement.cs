using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
	public Terrain terrain;

	public bool rotates = true;
	public bool followTerrainHeight = true;

	public bool moving = false;
	private Vector3 target;

	public float turnSpeed = 30.0f;

	public float maxSpeed = 10.0f;

	private float moveSpeed;

	void Start ()
	{
		moveSpeed = maxSpeed;
		target = this.transform.position;
	}

	// Update is called once per frame
	void Update ()
	{
		Move ();
	}
		
	void Move()
	{
		if(moving)
		{

			float move_step = moveSpeed * Time.deltaTime;
			
			if(rotates)
			{
				float rotate_step = turnSpeed * Time.deltaTime;

				//find the vector pointing from our position to the target
				Vector3 look_direction = (target - transform.position);
				
				//rotate us over time according to speed until we are in the required rotation
				Vector3 newDir = Vector3.RotateTowards(transform.forward,look_direction, rotate_step, 0.0f);
				transform.rotation = Quaternion.LookRotation(newDir);
					
			
				// move if close enough in facing
				//float close_to_facing_angle =  10;
				//if (Quaternion.Angle(transform.rotation, look_rotation) < close_to_facing_angle)
				//{

				//}	

				transform.position = Vector3.MoveTowards(transform.position, target, move_step);
					
			}
			else
			{
				transform.position = Vector3.MoveTowards(transform.position, target, move_step);
			}

		}
	}

	public void SetTarget(Vector3 t)
	{
		float  y = t.y;
		if(followTerrainHeight)
			y = terrain.SampleHeight(t);

		target = new Vector3( t.x, y, t.z);
	}

	public void ChangeSpeed(float scale)
	{
		if(scale > 1.0f)
			scale = 1.0f;

		moveSpeed = scale * maxSpeed;
	}
}

