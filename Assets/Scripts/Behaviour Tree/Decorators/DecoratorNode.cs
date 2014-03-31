
using System;
using System.Collections.Generic;
namespace AssemblyCSharp
{
	// takes the boolean input of its one and only child
	// and applies a function which returns a bool
	public abstract class DecoratorNode : InnerNode
	{
		protected  abstract bool DecoratingFunc(Node child);
	
		public DecoratorNode (Node child)
		{
			children = new List<Node>();
			children.Add (child);
		}
		
		public override bool Execute()
		{
			return DecoratingFunc(children[0]);
		}
	}
}

