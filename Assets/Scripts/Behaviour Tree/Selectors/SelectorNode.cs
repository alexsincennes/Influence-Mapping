
using System;
using System.Collections.Generic;
namespace AssemblyCSharp
{
	// executes all its children in order
	// first returned success makes execute() return success
	// no successes -> failure
	public class SelectorNode : InnerNode
	{
		public SelectorNode (List<Node> aChildren)
		{
			children = aChildren;
		}
		
		public override bool Execute()
		{
			foreach(Node child in children)
			{
				if(child.Execute())
					return true;
				
			}
			
			return false;
		}
	}
}

