using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Collider))]
public class Soldier : MonoBehaviour
{
	public Unit unit;
	
	public Movement mov;
	
	private Vector3 target;
	
	//private bool inside_unit_bounds = true;

	public bool attacking_mode = false;

	public int number_in_unit;

	public RangedWeapon rangedWeapon;
	

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{
		if(attacking_mode)
		{
			RangedAttack ();
		}
	}
	
	void Melee ()
	{
		// optional
		
	}
	
	void RangedAttack ()
	{
		int max_length = ((Unit)unit.enemies_in_range[0]).soldiersInUnit.Length-1;
		int index = Random.Range(0, max_length);

		rangedWeapon.Fire( ((Unit)unit.enemies_in_range[0]).soldiersInUnit[index], this.transform.position);
	}

	public void Die ()
	{
		Debug.Log("DIED!");
		unit.SignalDeath(number_in_unit);

		GameObject.Destroy(this.gameObject);
	}

	/*
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
*/
}