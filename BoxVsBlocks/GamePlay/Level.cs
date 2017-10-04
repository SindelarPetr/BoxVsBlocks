using GameEngine.CameraEngine;
using GameEngine.GamePrimitives;

namespace BoxVsBlock.GamePlay
{
	public class Level : BasicLevel
	{
		private readonly Map _map;

		public int Score => _map.Score;

		public Level() : base(new Camera())
		{
			_map = new Map();
		}

		public void RevivePlayer()
		{
			_map.RevivePlayer();
		}
	}
}
