
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
		public static float TotalInfluence(float friend_influence, float foe_influence)
		{
			return friend_influence - foe_influence;
		}
		
		// friendly influence + foe influence
		public static float TotalTension(float friend_influence, float foe_influence)
		{
			return (friend_influence + foe_influence);
		}
		
		// tension map - |influence map|
		public static float TotalVulnerability(float tension, float influence)
		{
			return tension - Mathf.Abs (influence);
		}
		
		
		public static float InfluenceValue(Vector3 pos, Unit other)
		{
			// take into account the various fields of other
			// to adjust the influence value
			float r = Vector3.Distance(pos, other.transform.position);
			
			float ans = 1/Mathf.Pow(1.1f,r);
			
			//if (!other.team.Equals(this.team))
			//	ans = -ans;
			
			return ans;
			
		}
		
		public static float TerrainValue(Terrain terrain, Vector3 pos)
		{
			// use information from a manager/god-class
			return terrain.SampleHeight(pos);
		}
	}
}

