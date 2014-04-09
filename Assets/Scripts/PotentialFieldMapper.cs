using UnityEngine;
using System.Collections;

public class PotentialFieldMapper : MapRenderer
{
	// 2d potential field map for each (x,y) coordinate of the map
	// the first 2d array is for the coordinate
	// the second for the potential field map
	public float[,][,] potentialFieldValues;


	protected override void GetValues ()
	{
		potentialFieldValues = new float[size,size][,];

		// for coordinate (i,j) generate potential field map
		for(int i = 0; i < size; i++)
		{
			for(int j = 0; j < size; j++)
			{
				potentialFieldValues[i,j] = GeneratePotentialFieldMap(i,j);
			}
		}

		values = potentialFieldValues[0,0];
	}

	float[,] GeneratePotentialFieldMap (int x, int y)
	{
		float[,] map = new float[size,size];
		bool[,] marked_map = new bool[size,size]; // initializes to false

		MarkTile(x,y,0, map, marked_map);

		return map;
	}

	private void MarkTile(int x, int y, int current_value, float[,] map, bool[,] marked_map)
	{



		if(x >= 0 && x < size && y < size && y >= 0 && !marked_map[x,y])
		{
			marked_map[x,y] = true;

			// if not water/river, continue
			// else stop (can't pass through it!
			if(terrain.SampleHeight(new Vector3(x,0,y)) > 1)
		   	{
				map[x,y] = current_value;

				if(!marked_map[x+1,y])
				{
					MarkTile(x+1,y, current_value+1, map, marked_map);
				}

				if(!marked_map[x-1,y])
				{
					MarkTile(x-1,y, current_value+1, map, marked_map);
				}

				if(!marked_map[x,y+1])
				{
					MarkTile(x,y+1, current_value+1, map, marked_map);
				}

				if(!marked_map[x,y-1])
				{
					MarkTile(x,y-1, current_value+1, map, marked_map);
				}
			}
			else
			{
				map[x,y] = -100;
			}
		}
	}
}

