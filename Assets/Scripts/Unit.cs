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
	
	public int spawnSoldierNumber = 1;

	public int currentSoldierNumber;
	
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
	private float[] local_formation_vuln_values;
	private float[] terrain_values;

	private Node root;
	

	void Start ()
	{
		// spawn soldiers of unit
		soldiersInUnit = new GameObject[spawnSoldierNumber];
			
		// spawn troops
		for(int i=0; i < spawnSoldierNumber; i++)
		{	
			// assume soldier takes 1.2 x 1.2 space. change if this is untrue later
			Vector3 pos = SetSoldierLineFormation(i);
			// adjust position for height
			pos = new Vector3( pos.x, terrain.SampleHeight(pos), pos.z);
			
			GameObject soldier_obj = GameObject.Instantiate(soldierObject, pos, Quaternion.identity) as GameObject;
			Soldier s = soldier_obj.GetComponent<Soldier>();
			s.unit = this;

			Movement m = soldier_obj.GetComponent<Movement>();
			//m.moveSpeed = mov.moveSpeed;
			m.terrain = this.terrain;

			
			soldiersInUnit[i] = soldier_obj;
		}
		
	}

	void Awake () 
	{
		currentSoldierNumber = spawnSoldierNumber;

		// initiliase arrays of local values (neighbours + own location)
		// arranged as follows: [TL,TM,TR,ML,MM,MR,LL,LM,LR]

		local_friend_influence_values 	= 	new float[local_values_num];
		local_foe_influence_values 		= 	new float[local_values_num];
		local_influences_values 		= 	new float[local_values_num];
		local_tension_values 			=	new float[local_values_num];
		local_vulnerability_values 		= 	new float[local_values_num];
		local_formation_vuln_values  	= 	new float[local_values_num];
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
			mov.SetTarget(target);
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
			if(hit.gameObject.tag.Equals("Team_" + team.ToString()))
				if(!hit.gameObject.GetComponent<Unit>().Equals(this))
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
			m.SetTarget(SetSoldierLineFormation(i));
			m.moving = true;
			i++;
		}	
	}

	Vector3 SetSoldierLineFormation(int index)
	{


		float angle = Vector3.Angle(new Vector3(0,0,1), this.transform.forward);

		Vector3 cross_product = Vector3.Cross(new Vector3(0,0,1), this.transform.forward);
		if (cross_product.y < 0) 
			angle = -angle;
		
		Vector3 angle_vec = new Vector3(0,angle,0);


		const float scale = 1.2f;

		float x_offset = boundingShape.GetPaddedCornerX() - scale * ( index % (int)(1 +  boundingShape.GetPaddedCornerX()*2/scale)) + this.transform.position.x;
		float z_offset = boundingShape.GetPaddedCornerY() - scale * (int)(scale*index / (boundingShape.GetPaddedCornerX()*2)) + this.transform.position.z;
		

		Vector3 unrotated = new Vector3(x_offset, this.transform.position.y, z_offset);

		return RotatePointAroundPivot(unrotated, this.transform.position, angle_vec);
	}

	Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
	{
		Vector3 dir = point - pivot;
		dir = Quaternion.Euler(angle) * dir;
		return dir + pivot;
	}
}
