using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour 
{
	public Terrain terrain;


	// unit data
	public enum TEAM {A,B};
	public TEAM team;
	public enum TROOP_TYPE {INFANTRY, ARTILLERY, CAVALRY};
	public int soldier_number = 1;
	public TROOP_TYPE type = TROOP_TYPE.INFANTRY;

		
	//AI data
	private int local_values_num = 9;
	private float[] local_friend_influence_values;
	private float[] local_foe_influence_values;
	private float[] local_influences_values;
	private float[] local_tension_values;
	private float[] local_vulnerability_values;
	private float[] terrain_values;

	void Start ()
	{

	}

	void Awake () 
	{
		// initiliase arrays of local values (neighbours + own location)
		// arranged as follows: [TL,TM,TR,ML,MM,MR,LL,LM,LR]

		local_friend_influence_values 	= 	new float[local_values_num];
		local_foe_influence_values 		= 	new float[local_values_num];
		local_influences_values 		= 	new float[local_values_num];
		local_tension_values 			=	new float[local_values_num];
		local_vulnerability_values 		= 	new float[local_values_num];
		terrain_values 					= 	new float[local_values_num];
	}

	void Update () 
	{
		GetLocalInfluences();
		RenderValues(local_influences_values);
	}

	/// <summary>
	/// Displays the values of the local map chosen.
	/// </summary>
	/// <param name="values">The value array to be rendered.</param>
	void RenderValues (float[] values)
	{
		for(int i = 0; i < local_values_num; i++)
		{
			Color c;
			if (values[i] < 0)
				c = new Color(-values[i], 0, 0);
			else
				c = new Color(0, values[i], 0);

			this.transform.FindChild("ValueQuad_" + i).GetComponent<MeshRenderer>().material.color = c;
		}
	}
	
	void GetLocalInfluences()
	{
		Vector3[] surroundings = 
		{
			new Vector3(transform.position.x - 1, 	transform.position.y, 	transform.position.z + 1),
			new Vector3(transform.position.x, 		transform.position.y, 	transform.position.z + 1),
			new Vector3(transform.position.x + 1, 	transform.position.y, 	transform.position.z + 1),
			new Vector3(transform.position.x - 1, 	transform.position.y, 	transform.position.z),
			this.transform.position,
			new Vector3(transform.position.x + 1, 	transform.position.y, 	transform.position.z),
			new Vector3(transform.position.x - 1, 	transform.position.y, 	transform.position.z - 1),
			new Vector3(transform.position.x, 		transform.position.y, 	transform.position.z - 1),
			new Vector3(transform.position.x + 1, 	transform.position.y, 	transform.position.z - 1),
		};

		//Debug.Log("COLLECTING LOCAL VALUES FOR: " + this.transform.position);

		for(int i=0; i < local_values_num; i++)
		{
			Vector3 pos = surroundings[i];
			//Debug.Log (surroundings[i]);
			local_friend_influence_values	[i] = FriendInfluence(pos);
			//Debug.Log (local_friend_influence_values[i]);
			local_foe_influence_values		[i] = FoeInfluence(pos);
			//Debug.Log (local_foe_influence_values[i]);
			local_influences_values			[i] = TotalInfluence(local_friend_influence_values[i], local_foe_influence_values[i]);
			//Debug.Log (local_influences_values[i]);
			local_tension_values			[i] = TotalTension(local_friend_influence_values[i], local_foe_influence_values[i]);
			//Debug.Log (local_tension_values[i]);
			local_vulnerability_values		[i] = TotalVulnerability(local_tension_values[i], local_influences_values[i]);
			//Debug.Log (local_vulnerability_values[i]);
			terrain_values 					[i] = TerrainValue(pos);
		}
		
	}
	
	public float FriendInfluence(Vector3 pos)
	{
		string sTeam;
		if(this.team.Equals(TEAM.A))
			sTeam = "A";
		else
			sTeam = "B";
		
		return TeamInfluence(pos, sTeam);
	}
	
	public float FoeInfluence(Vector3 pos)
	{
		string sTeam;
		if(this.team.Equals(TEAM.A))
			sTeam = "B";
		else
			sTeam = "A";
		
		return TeamInfluence(pos, sTeam);
	}
	
	public float TeamInfluence(Vector3 pos, string sTeam)
	{
		float total = 0;
		foreach (GameObject other in GameObject.FindGameObjectsWithTag("Team_"+sTeam))
		{
			Unit o = other.GetComponent<Unit>();

			// we WANT to count ourselves as a friendly unit,
			// so as to not distort the results
			// uncomment to not count yourself
			//if (!o.Equals(this))

			total += InfluenceValue (pos, o);
		}
		
		return total;
	}
	
	// friendly influence - foe influence
	public float TotalInfluence(float friend_influence, float foe_influence)
	{
		return friend_influence - foe_influence;
	}
	
	// friendly influence + foe influence
	public float TotalTension(float friend_influence, float foe_influence)
	{
		return (friend_influence + foe_influence);
	}
	
	// tension map - |influence map|
	public float TotalVulnerability(float tension, float influence)
	{
		return tension - Mathf.Abs (influence);
	}
	
	
	public float InfluenceValue(Vector3 pos, Unit other)
	{
		// take into account the various fields of other
		// to adjust the influence value
		float r = Vector3.Distance(pos, other.transform.position);
		
		float ans = 1/Mathf.Pow(1.1f,r);
		
		//if (!other.team.Equals(this.team))
		//	ans = -ans;
			
		return ans;
			
	}
	
	public float TerrainValue(Vector3 pos)
	{
		// use information from a manager/god-class
		return terrain.SampleHeight(pos);
	}
}
