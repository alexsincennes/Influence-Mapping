using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;
using System.Linq;

public class Unit : MonoBehaviour 
{
	public Terrain terrain;

	// directional data
	public float speed = 3;
	public Vector3 direction = new Vector3(0,0,0);
	public Vector3 facing = new Vector3(0,0,0);


	// unit data
	public enum TEAM {A,B};
	public TEAM team;
	public enum TROOP_TYPE {INFANTRY, ARTILLERY, CAVALRY};
	public int soldier_number = 1;
	public TROOP_TYPE type = TROOP_TYPE.INFANTRY;


	// enemy counting
	int friendlies_close;
	int enemies_close;
	public float local_unit_detection_range = 5.0f;
	public float friend_too_close_range = 1.0f;
		
	// mapping data
	private Vector3[] surroundings;

	private int local_values_num = 9;
	private float[] local_friend_influence_values;
	private float[] local_foe_influence_values;
	private float[] local_influences_values;
	private float[] local_tension_values;
	private float[] local_vulnerability_values;
	private float[] terrain_values;



	void Start ()
	{
		Debug.Log (TEAM.A.ToString());
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
	}

	Node InitUnitAI ()
	{

		Leaf locally_outnumbered = new Leaf(new System.Func<bool>( () => {
			CountUnitsClose();
			return enemies_close > friendlies_close;
		}));


		Leaf target_highest_influence = new Leaf(new System.Func<bool>( () => {
			direction = surroundings[local_influences_values.ToList().IndexOf(local_influences_values.Max())];
			return true;
		}));

		Leaf target_highest_vulnerability = new Leaf(new System.Func<bool>( () => {
			direction = surroundings[local_vulnerability_values.ToList().IndexOf(local_vulnerability_values.Max())];
			return true;
		}));

		Leaf eliminate_collision_vectors = new Leaf(new System.Func<bool>( () => {
			//Collider[] hits = 
			return true;
		}));

		SelectorNode root = new SelectorNode(new List<Node> { 

		});


		return root;
	}

	/*
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
	*/
	
	void GetLocalInfluences()
	{
		surroundings = new Vector3[]
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
			local_friend_influence_values	[i] = InfluenceMapper.FriendInfluence(this, pos);
			//Debug.Log (local_friend_influence_values[i]);
			local_foe_influence_values		[i] = InfluenceMapper.FoeInfluence(this, pos);
			//Debug.Log (local_foe_influence_values[i]);
			local_influences_values			[i] = InfluenceMapper.TotalInfluence(local_friend_influence_values[i], local_foe_influence_values[i]);
			//Debug.Log (local_influences_values[i]);
			local_tension_values			[i] = InfluenceMapper.TotalTension(local_friend_influence_values[i], local_foe_influence_values[i]);
			//Debug.Log (local_tension_values[i]);
			local_vulnerability_values		[i] = InfluenceMapper.TotalVulnerability(local_tension_values[i], local_influences_values[i]);
			//Debug.Log (local_vulnerability_values[i]);
			terrain_values 					[i] = InfluenceMapper.TerrainValue(terrain, pos);

		}
		
	}

	void CountUnitsClose()
	{
		int team_A_count = 0;
		int team_B_count = 0;

		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, local_unit_detection_range);

		foreach (Collider hit in hitColliders)
		{
			if(hit.gameObject.tag.Equals("Team_A"))
			   team_A_count++;
			 else
			   if(hit.gameObject.tag.Equals("Team_B"))
			   team_B_count++;
		}


		if(this.team.Equals(TEAM.A))
		{
			friendlies_close = team_A_count;
			enemies_close = team_B_count;
		}
		else
		{
			enemies_close = team_A_count;
			friendlies_close = team_B_count;
		}
	}

	Unit[] GetFriendsTooClose()
	{
		List<Unit> friends = new List<Unit>();

		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, friend_too_close_range);

		foreach (Collider hit in hitColliders)
		{
			if(hit.gameObject.tag.Equals("Team_A"))
				friends.Add (hit.gameObject.GetComponent<Unit>());

		}

		return friends.ToArray();
	}
	

}
