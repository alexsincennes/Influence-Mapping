using UnityEngine;
using System.Collections;
using AssemblyCSharp;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class InfluenceMapRenderer : MapRenderer
{
	public GameObject unit_obj;
	private Unit unit;
	public float unit_height = 0.5f;

	protected override void GetValues()
	{
		unit = unit_obj.GetComponent<Unit>();

		for(int x = 0; x < size; x++)
		{
			for(int z = 0; z < size; z++)
			{
				
				Vector3 pos = new Vector3(this.transform.position.x - size/2 + x, unit_height, (int)this.transform.position.z - size/2 + z);
				float friend_influence_value 	= InfluenceMapper.FriendInfluence(unit, pos);
				float foe_influence_value		= InfluenceMapper.FoeInfluence(unit, pos);
				float influences_value			= InfluenceMapper.TotalInfluence(friend_influence_value, foe_influence_value);
				float tension_value 			= InfluenceMapper.TotalTension(friend_influence_value, foe_influence_value);
				float vulnerability_value		= InfluenceMapper.TotalVulnerability(tension_value, influences_value);
				float form_vuln_value 			= InfluenceMapper.TotalFormationVulnerability(unit, pos, vulnerability_value, tension_value);
				float influence_minus_self 		= InfluenceMapper.TotalInfluenceMinusSelf(unit, pos, influences_value);
				float terrain_value 			= InfluenceMapper.TerrainValue(terrain, pos);

				values[x,z] = form_vuln_value / 5;
				
			}
		}		
	}
}

