using System;

namespace BoxVsBlock.Menu
{
	public struct Score
	{
		public int BestScore { get; private set; }
		public int AllScore { get; private set; }
		public int BoxesDestroyed { get; private set; }

		public static Score operator +(Score a, Score b)
		{
			return new Score
			{
				AllScore = a.AllScore + b.AllScore,
				BestScore = Math.Max(a.BestScore, b.BestScore),
				BoxesDestroyed = a.BoxesDestroyed + b.BoxesDestroyed
			};
		}
	}
}
