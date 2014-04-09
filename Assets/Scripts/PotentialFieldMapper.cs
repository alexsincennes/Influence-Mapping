using UnityEngine;
using System.Collections.Generic;

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

		int currentValue = 0;

		Queue<Vector3> q = new Queue<Vector3>();
		
		if(terrain.SampleHeight(new Vector3(x, 0, y)) < 1)
		{
			map[x,y] = - 100;
			return map;
		}
		
		marked_map[x,y] = true;
		map[x,y] = currentValue;
		q.Enqueue(new Vector3(x,y, currentValue));
		
		while(q.Count != 0)
		{
			Vector3 curPos = q.Dequeue();
			
			EnqueueAndMarkIfUnmarked(1,1,q,curPos,marked_map, map);
			EnqueueAndMarkIfUnmarked(1,-1,q,curPos,marked_map, map);
			EnqueueAndMarkIfUnmarked(-1,-1,q,curPos,marked_map, map);
			EnqueueAndMarkIfUnmarked(-1,1,q,curPos,marked_map, map);
			EnqueueAndMarkIfUnmarked(1,0,q,curPos,marked_map, map);
			EnqueueAndMarkIfUnmarked(-1,0,q,curPos,marked_map, map);
			EnqueueAndMarkIfUnmarked(0,1,q,curPos,marked_map, map);
			EnqueueAndMarkIfUnmarked(0,-1,q,curPos,marked_map, map);
			
		}
		

		return map;
	}
	
	private void EnqueueAndMarkIfUnmarked(int x_mod, int y_mod, Queue<Vector3> q, Vector3 curPos, bool[,] marked_map, float[,] map)
	{
		if((int)curPos.x+x_mod >= size || (int)curPos.x+x_mod < 0)
			return;
		if((int)curPos.y+y_mod >= size || (int)curPos.y+y_mod < 0)
			return;
		
		if(!marked_map[(int)curPos.x+x_mod,(int)curPos.y+y_mod]  && terrain.SampleHeight(new Vector3(curPos.x+x_mod, 0, curPos.y+y_mod)) > 1)
		{
			marked_map[(int)curPos.x+x_mod, (int)curPos.y+y_mod] = true;
			map[(int)curPos.x+x_mod, (int)curPos.y+y_mod] = curPos.z+1;
			
			q.Enqueue(new Vector3((int)curPos.x+x_mod,(int)curPos.y+y_mod, curPos.z+1));
		}
	}
}

