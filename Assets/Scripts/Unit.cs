using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;
using System.Linq;

[RequireComponent (typeof (Movement))]
[RequireComponent (typeof (BoundingShape))]
public class Unit : MonoBehaviour 
{
	public Terrain terrain;

	public Movement mov;
	public BoundingShape boundingShape;
	
	// the soldier object to spawn
	public GameObject soldierObject;
	
	public GameObject[] soldiersInUnit;
	
	// the AI-determined target vector, which we use to set the direction vector
	private Vector3 target = new Vector3(0,0,0);


	// unit data
	public enum TEAM {A,B};
	public TEAM team;
	public enum TROOP_TYPE {INFANTRY, ARTILLERY, CAVALRY};
	public TROOP_TYPE type = TROOP_TYPE.INFANTRY;
	
	public int soldier_number = 1;
	
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

	private Node root;
	

	void Start ()
	{
		// spawn soldiers of unit
		soldiersInUnit = new GameObject[soldier_number];
			
		// spawn troops
		for(int i=0; i < soldier_number; i++)
		{	
			// assume soldier takes 1.2 x 1.2 space. change if this is untrue later
			Vector3 pos = new Vector3( -boundingShape.GetPaddedCornerX() + this.transform.position.x + (i * 1.2f) % boundingShape.shape_corner.x, 0 , boundingShape.GetPaddedCornerY() + this.transform.position.z - 1.2f * (int)((i* 1.2f)/boundingShape.shape_corner.x) );
			// adjust position for height
			pos = new Vector3( pos.x, terrain.SampleHeight(pos), pos.z);
			
			GameObject soldier_obj = GameObject.Instantiate(soldierObject, pos, Quaternion.identity) as GameObject;
			Soldier s = soldier_obj.GetComponent<Soldier>();
			
			Movement m = soldier_obj.GetComponent<Movement>();
			m.moveSpeed = mov.moveSpeed;
			
			soldiersInUnit[i] = soldier_obj;
		}
		
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
		
		root = InitUnitAI();
	}

	void Update () 
	{
		GetLocalInfluences();
		root.Execute();
		LineFormation();
		//RenderValues(local_vulnerability_values);
		
	}

	Node InitUnitAI ()
	{
		Leaf query_enemies_remain = new Leaf(new System.Func<bool>( () => {
			return 0 != CountUnitsOfTeam(GetOpposingTeam());
		}));
		
		Leaf stop_moving = new Leaf(new System.Func<bool>( () => {
			mov.moving = false;
			return true;
		}));
	
		Leaf query_locally_outnumbered = new Leaf(new System.Func<bool>( () => {
			CountUnitsClose();
			return enemies_close > friendlies_close;
		}));


		Leaf target_highest_influence = new Leaf(new System.Func<bool>( () => {
			target = surroundings[local_influences_values.ToList().IndexOf(local_influences_values.Max())];
			return true;
		}));

		Leaf target_highest_vulnerability = new Leaf(new System.Func<bool>( () => {
			target = surroundings[local_vulnerability_values.ToList().IndexOf(local_vulnerability_values.Max())];
			return true;
		}));

		Leaf eliminate_collision_vectors = new Leaf(new System.Func<bool>( () => {
			foreach(Unit u in GetFriendsOverlapping() )
			{
				// avoid moving in +x direction
				if(u.transform.position.x > this.transform.position.x)
				{
					if(target.x > this.transform.position.x)
						target.x = this.transform.position.x;
				}
				else // avoid -x direction
				{
					if(target.x < this.transform.position.x)
						target.x = this.transform.position.x;
				}
				
				// avoid +z direction
				if(u.transform.position.z > this.transform.position.z)
				{
					if(target.z > this.transform.position.z)
						target.z = this.transform.position.z;
				}
				else // avoid -z direction
				{
					if(target.z < this.transform.position.z)
						target.z = this.transform.position.z;
				}
			}
			return true;
		}));
		
		Leaf set_direction = new Leaf(new System.Func<bool>( () => {
			mov.moving = true;
			mov.target = target;
			//Debug.Log (mov.direction);
			return true;
		}));
		
		SequenceNode no_enemies_tree = new SequenceNode(new List<Node> {
			new NotNode(query_enemies_remain),
			stop_moving
		});
		
		SequenceNode outnumbered_tree = new SequenceNode(new List<Node> {
			query_locally_outnumbered,
			target_highest_influence
		});
		
		SequenceNode not_outnumbered_tree = new SequenceNode(new List<Node> {
			target_highest_vulnerability
		});

		SelectorNode target_setting_tree = new SelectorNode(new List<Node> {
			outnumbered_tree,
			not_outnumbered_tree
		});
		
		
		SequenceNode enemies_tree = new SequenceNode(new List<Node> { 
			target_setting_tree,
			eliminate_collision_vectors,
			set_direction
		});
		
		SelectorNode root = new SelectorNode(new List<Node> {
			no_enemies_tree,
			enemies_tree
		});


		return root;
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
	
	/// <summary>
	/// Calculates the influence of all units on this unit's position and the 8 unit-length, manhattan distance
	/// positions away from it. Also transforms influence data into tension and vulnerability values through
	/// simple mathematical transformations (subtraction, addition).
	/// </summary>
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
	
	/// <summary>
	/// Gets count of friends and foes within a certain
	/// spherical range (configurable in unity editor). 
	/// Useful to determined if outnumbered
	/// within a certain locality
	/// </summary>
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
	
	/// <summary>
	/// Get friendlies units too 'close' to our unit.
	/// This is to maintain some cohesiveness and prevent overlap.
	/// </summary>
	/// <returns>Array of friendly units too close.</returns>
	Unit[] GetFriendsOverlapping()
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
	
	int CountUnitsOfTeam(TEAM team)
	{
		GameObject[] units = GameObject.FindGameObjectsWithTag("Team_" + team.ToString());
		
		return units.Length;
	}
	
	TEAM GetOpposingTeam()
	{
		if(team.Equals(TEAM.A))
			return TEAM.B;
		else
			return TEAM.A;
	}
	
	void ChangeBounding(BoundingShape.BOUNDING_SHAPE shape, float x, float y)
	{
		boundingShape.shape = shape;
		boundingShape.shape_corner = new Vector2(x,y);
	}
	
	void LineFormation()
	{
	
		int i = 0;
		foreach(GameObject s_obj in soldiersInUnit)
		{
			Movement m = s_obj.GetComponent<Movement>();
			m.target = new Vector3( -boundingShape.GetPaddedCornerX() + this.transform.position.x + (i * 1.2f) % boundingShape.shape_corner.x, 0 , boundingShape.GetPaddedCornerY() + this.transform.position.z - 1.2f * (int)((i* 1.2f)/boundingShape.shape_corner.x) );
			m.moving = true;
			i++;
		}	
	}

}
