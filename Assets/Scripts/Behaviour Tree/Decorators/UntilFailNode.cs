
using System;
namespace AssemblyCSharp
{
	public class UntilFailNode : DecoratorNode
	{
		public UntilFailNode (Node child) : base(child) {}
		
		protected override bool DecoratingFunc (Node child)
		{	
			while( child.Execute())
			{
				return true;
			}
      
      		return false;
    	}
	}
}

