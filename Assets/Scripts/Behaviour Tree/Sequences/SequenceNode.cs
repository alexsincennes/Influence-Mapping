
using System;
using System.Collections.Generic;
namespace AssemblyCSharp
{
	// executes all its children in order
	// first returned failure fails this node's execute()
	// no failures -> success
	public class SequenceNode : InnerNode
	{
		public SequenceNode (List<Node> aChildren)
		{
			children = aChildren;
		}
		
		public override bool Execute()
		{
			foreach(Node child in children)
			{
				if(! child.Execute())
					return false;
					
			}
			
			return true;
		}
	}
}

