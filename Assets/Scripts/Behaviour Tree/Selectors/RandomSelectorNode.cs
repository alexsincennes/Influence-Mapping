
using System;
using System.Collections.Generic;
namespace AssemblyCSharp
{
	// executes all its children in random order
	// first returned success makes execute() return success
	// no successes -> failure
	public class RandomSelectorNode : SelectorNode
	{
		private Random rand;
		List<Node> original_children;
	
		public RandomSelectorNode (List<Node> aChildren) : base(aChildren)
		{
			rand = new Random();
			original_children = new List<Node>();
		}
		
		public override bool Execute()
		{
			// put back children we had already selected in previous execution
			// and clear this list
			foreach (Node child in original_children)
			{
				children.Add(child);
			}
			original_children.RemoveRange(0,original_children.Count);
			
			while(children.Count > 0)
			{
				int index = rand.Next(0, children.Count);
				original_children.Add(children[index]);
				
				bool execute_result = children[index].Execute();
				children.RemoveAt(index);
				
				if(execute_result)
					return true;
			}
			
			return false;
		}
	}
}

