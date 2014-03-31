
using System;
using System.Collections.Generic;
namespace AssemblyCSharp
{
	public abstract class InnerNode : Node
	{
		protected List<Node> children;
		
		public abstract bool Execute();
	}
}