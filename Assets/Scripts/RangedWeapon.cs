using UnityEngine;
using System.Collections;

public class RangedWeapon : MonoBehaviour
{
	
	// weapon data
	public float range;
	public float accuracy;
	public float reload_time;
	public int capacity = 1;
	public float time_between_shots;
	private int current_ammo;
	
	public bool smoke = true;

	public bool ready_to_fire = true;

	private float time;


	// Use this for initialization
	void Start ()
	{
		current_ammo = capacity;
		time = 0;
	}

	// Update is called once per frame
	void Update ()
	{
		time+= Time.deltaTime;

		if(!ready_to_fire)
		{
			if(current_ammo <= 0)
			{
				if(time >= reload_time)
				{
					time = 0;
					current_ammo = capacity;
					ready_to_fire = true;
				}
			}
			else
			{
				if(time >= time_between_shots)
				{
					time = 0;
					ready_to_fire = true;
				}
			}
		}
	}

	public void Fire (GameObject target, Vector3 origin)
	{
		if(ready_to_fire)
		{
			//Debug.Log ("Firing!");
			RaycastHit hitInfo;
			
			//fire ray based on accurate values, etc
			
			Vector3 error = new Vector3((1-accuracy) * Random.Range(-10.0f, 10.0f), 
			                            (1-accuracy) * Random.Range(-10.0f, 10.0f), 
			                            (1-accuracy) * Random.Range(-10.0f, 10.0f));
			
			Ray ray = new Ray(origin, target.transform.position + error);
			
			if(Physics.Raycast(origin, target.transform.position, out hitInfo))
			{
				if(hitInfo.collider.gameObject.tag.Equals("Soldier"))
				{
					Debug.Log ("hit target");

					target.GetComponent<Soldier>().Die();
				}
			}
			
			
			current_ammo--;
			ready_to_fire = false;
			time = 0;
		}
	}

}

