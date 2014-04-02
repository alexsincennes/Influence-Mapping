using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Collider))]
public class Soldier : MonoBehaviour
{
	public Unit unit;
	
	public Movement mov;
	
	private Vector3 target;
	
	private bool inside_unit_bounds = true;
	

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}
	
	void Melee ()
	{
		// do some animation
		
	}
	
	void RangedAttack ()
	{
		// do some animation
		
		// cast some shot ray
	}
	
	void FormUp ()
	{
		// move to target as set by unit-level AI
	}
	
	void FollowBounding ()
	{
		
	}	
	
	void OnTriggerEnter (Collider col)
	{
		if(col.gameObject.GetComponent<Unit>() != null)
		{
			if(col.gameObject.GetComponent<Unit>().Equals(unit))
			{
				inside_unit_bounds = true;
			}
		}
	}
	
	void OnTriggerStay (Collider col)
	{
	
	}
	
	void OnTriggerExit (Collider col)
	{
		if(col.gameObject.GetComponent<Unit>() != null)
		{
			if(col.gameObject.GetComponent<Unit>().Equals(unit))
			{
				inside_unit_bounds = false;
			}
		}
	}
}