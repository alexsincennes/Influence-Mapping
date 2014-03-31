
using System;
namespace AssemblyCSharp
{
	/// <summary>
	/// Decorator that returns opposite of child's output.
	/// </summary>
	public class NotNode : DecoratorNode
	{
		public NotNode (Node child) : base(child)
		{
		}

		protected override bool DecoratingFunc(Node child)
		{
			return !child.Execute();
		}
	}
}

