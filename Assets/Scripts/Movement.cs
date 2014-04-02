using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
	public bool rotates = true;

	public bool moving = false;
	public Vector3 target = new Vector3(0,0,0);
	
	public float turnSpeed = 30.0f;
	
	public float moveSpeed = 10.0f;

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
				
				float close_to_facing_angle =  10;
				
				Debug.Log (Vector3.Angle(this.transform.forward, this.transform.position - target));
			
				//find the vector pointing from our position to the target
				Vector3 look_direction = (target - transform.position).normalized;
				
				//create the rotation we need to be in to look at the target
				Quaternion look_rotation = Quaternion.LookRotation(look_direction);
				
				//rotate us over time according to speed until we are in the required rotation
				transform.rotation = Quaternion.Slerp(transform.rotation, look_rotation, Time.deltaTime * turnSpeed);
				
				//Vector3.RotateTowards(transform.position, direction, rotate_step * Mathf.PI / 180, 0.0f);
			
			
				// move if close enough in facing
				if (Vector3.Angle(this.transform.forward, this.transform.position - target) < close_to_facing_angle)
				{
					Vector3.MoveTowards(transform.position, target, move_step);
					
				}	
			
			}
			else
			{
				Vector3.MoveTowards(transform.position, target, move_step);
			}
			
		}
	}
}

