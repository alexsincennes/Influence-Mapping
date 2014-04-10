
using System;
using UnityEngine;
namespace AssemblyCSharp
{
	// logical influence mapping
	public static class InfluenceMapper
	{
		public static float FriendInfluence(Unit u, Vector3 pos)
		{
			return TeamInfluence(pos, u.team);
		}
		
		public static float FoeInfluence(Unit u, Vector3 pos)
		{
			Unit.TEAM other_team;

			if(u.team.Equals(Unit.TEAM.A))
				other_team = Unit.TEAM.B;
			else
				other_team = Unit.TEAM.A;

			return TeamInfluence(pos, other_team);
		}

		// for code duplication reduction -> used by friend/foe influence
		public static float TeamInfluence(Vector3 pos, Unit.TEAM team)
		{
			float total = 0;


			foreach (GameObject other in GameObject.FindGameObjectsWithTag("Team_"+team.ToString ()))
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
		// friend and foe influences combined
		public static float TotalInfluence(float friend_influence, float foe_influence)
		{
			return friend_influence - foe_influence;
		}
		
		// friendly influence + foe influence
		// where everyone (regardless of team) is
		public static float TotalTension(float friend_influence, float foe_influence)
		{
			return (friend_influence + foe_influence);
		}
		
		// tension map - |influence map|
		// where the frontline will be / is
		public static float TotalVulnerability(float tension, float influence)
		{
			return tension - Mathf.Abs (influence);
		}

		// vulnerability map - scale of tension map (excluding self)
		// go towards front line while avoiding getting too close to friends
		public static float TotalFormationVulnerability(Unit u, Vector3 pos, float vulnerability, float tension)
		{
			float tension_scale = 0.03f;
			return vulnerability - tension_scale * (tension - InfluenceValue(pos, u));
		}

		public static float TotalInfluenceMinusSelf(Unit u, Vector3 pos, float influence)
		{
			return influence - InfluenceValue(pos, u);
		}
		
		public static float InfluenceValue(Vector3 pos, Unit other)
		{
			float r = Vector3.Distance(pos, other.transform.position);


			float ans = other.currentSoldierNumber/Mathf.Pow(1.1f,r);
			
			return ans;
			
		}

		public static float TerrainValue(Terrain terrain, Vector3 pos)
		{
			// use information from a manager/god-class
			return terrain.SampleHeight(pos);
		}
	}
}

