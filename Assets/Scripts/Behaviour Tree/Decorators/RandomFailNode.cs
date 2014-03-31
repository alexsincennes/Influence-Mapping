
using System;
namespace AssemblyCSharp
{
	/// <summary>
	/// Randomly chooses to fail based on a parameter from 0 to 1.
	/// </summary>
	public class RandomFailNode : DecoratorNode
	{
		private float _failChance;
		Random rand;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.RandomFailNode"/> class.
		/// </summary>
		/// <param name="failChance">Odds of returning false without running decorated child.</param>
		public RandomFailNode (Node child, float failChance) : base(child)
		{
			_failChance = failChance;
			rand = new Random();
		}


		protected override bool DecoratingFunc (Node child)
		{
			if(rand.NextDouble() > _failChance)
				return base.Execute ();
			else
				return false;
		}
	}
}

