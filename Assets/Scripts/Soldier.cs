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
		int max_length = ((Unit)unit.enemies_in_range[0]).GetSoldiers().Length-1;
		int index = Random.Range(0, max_length);

		if( ((Unit)unit.enemies_in_range[0]).GetSoldiers().Length > 0)
		{
				
			rangedWeapon.Fire( ((Unit)unit.enemies_in_range[0]).GetSoldiers()[index], this.transform.position);
		}
	}

	public void Die ()
	{
		unit.SignalDeath(this.gameObject);

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
		}W
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