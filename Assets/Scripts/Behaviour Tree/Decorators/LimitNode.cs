
using System;
using System.Collections.Generic;
namespace AssemblyCSharp
{
	// iterates over its only child a set number of times.
	// returns false upon completion
	public class LimitNode : DecoratorNode
	{
		private int iterationNum;
		
		public  LimitNode (Node child, int num) : base(child)
		{
			iterationNum = num;
		}	
		
		
		protected override bool DecoratingFunc (Node child)
		{
			int cur = 0;
		
			if(cur < iterationNum) 
			{
				cur++;
				return child.Execute();
			}
			
			return false;
		}
	}
}

