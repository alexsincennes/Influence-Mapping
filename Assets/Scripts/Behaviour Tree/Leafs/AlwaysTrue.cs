
using System;
namespace AssemblyCSharp
{
	/// <summary>
	/// Leaf node that always returns true.
	/// </summary>
	public class AlwaysTrue : Node
	{
		public AlwaysTrue ()
		{
		}

		public bool Execute()
		{
			return true;
		}
	}
}

