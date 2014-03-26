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
		
	}
	
	void GetLocalInfluences()
	{
		Vector3[] surroudings = 
		{
			new Vector3(transform.position.x - 1, 	transform.position.y, 	transform.position.z + 1),
			new Vector3(transform.position.x, 		transform.position.y, 	transform.position.z + 1),
			new Vector3(transform.position.x + 1, 	transform.position.y, 	transform.position.z + 1),
			new Vector3(transform.position.x - 1, 	transform.position.y, 	transform.position.z),
			new Vector3(transform.position.x, 		transform.position.y, 	transform.position.z),
			new Vector3(transform.position.x + 1, 	transform.position.y, 	transform.position.z),
			new Vector3(transform.position.x - 1, 	transform.position.y, 	transform.position.z - 1),
			new Vector3(transform.position.x, 		transform.position.y, 	transform.position.z - 1),
			new Vector3(transform.position.x + 1, 	transform.position.y, 	transform.position.z - 1),
		};
		
		for(int i=0; i < local_values_num; i++)
		{
			Vector3 pos = surroudings[i];
			local_friend_influence_values	[i] = FriendInfluence(pos);
			local_foe_influence_values		[i] = FoeInfluence(pos);
			local_influences_values			[i] = TotalInfluence(local_friend_influence_values[i], local_foe_influence_values[i]);
			local_tension_values			[i] = TotalTension(local_friend_influence_values[i], local_foe_influence_values[i]);
			local_vulnerability_values		[i] = TotalVulnerability(local_tension_values[i], local_influences_values[i]);
			terrain_values 					[i] = 0;
		}
		
	}
	
	float FriendInfluence(Vector3 pos)
	{
		string sTeam;
		if(this.team.Equals(TEAM.A))
			sTeam = "A";
		else
			sTeam = "B";
		
		return TeamInfluence(pos, sTeam);
	}
	
	float FoeInfluence(Vector3 pos)
	{
		string sTeam;
		if(this.team.Equals(TEAM.A))
			sTeam = "B";
		else
			sTeam = "A";
		
		return TeamInfluence(pos, sTeam);
	}
	
	float TeamInfluence(Vector3 pos, string sTeam)
	{
		float total = 0;
		foreach (GameObject other in GameObject.FindGameObjectsWithTag("Team_"+sTeam))
		{
			Unit o = other.GetComponent<Unit>();
			total += InfluenceValue (pos, o);
		}
		
		return total;
	}
	
	// friendly influence - foe influence
	float TotalInfluence(float friend_influence, float foe_influence)
	{
		return friend_influence - foe_influence;
	}
	
	// friendly influence + foe influence
	float TotalTension(float friend_influence, float foe_influence)
	{
		return friend_influence + foe_influence;
	}
	
	// tension map - |influence map|
	float TotalVulnerability(float tension, float influence)
	{
		return tension - Mathf.Abs (influence);
	}
	
	
	float InfluenceValue(Vector3 pos, Unit other)
	{
		// take into account the various fields of other
		// to adjust the influence value
		float r = Vector3.Distance(pos, other.transform.position);
		
		float ans = 1.0f/(r*r);
		
		//if (!other.team.Equals(this.team))
		//	ans = -ans;
			
		return ans;
			
	}
	
	float TerrainValue(Vector3 pos)
	{
		// use information from a manager/god-class
		return 0;
	}
}
