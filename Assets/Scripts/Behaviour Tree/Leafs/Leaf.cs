
using System;
namespace AssemblyCSharp
{
	// leaf of behaviour tree: either condition or action,
	// but both are functions returning a boolean.
	// A leaf executes one such function associated with it.
	public class Leaf : Node
	{
		Func<bool> leafFunction;

		public Leaf (Func<bool> f)
		{
			leafFunction = f;
		}

		public bool Execute () {return leafFunction();}
	}
}

