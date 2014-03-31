
using System;
namespace AssemblyCSharp
{
	/// <summary>
	/// Leaf node that always returns false.
	/// </summary>
	public class AlwaysFalse : Node
	{
		public AlwaysFalse ()
		{
		}
		
		public bool Execute()
		{
			return false;
		}
	}
}

