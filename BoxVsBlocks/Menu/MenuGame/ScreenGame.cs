using BoxVsBlock.GamePlay;
using GameEngine.CameraEngine;
using GameEngine.Input.TouchPanel;
using GameEngine.Menu.Screens;
using Microsoft.Xna.Framework.Graphics;

namespace BoxVsBlock.Menu.MenuGame
{
	public class ScreenGame : MenuScreen
	{
		private Level _level;
		public int Score => _level.Score;

		public ScreenGame(Camera camera)
			: base(camera)
		{

		}

		public void StartNewGame(MyTouch touch)
		{
			_level = new Level();
		}

		public override void Update()
		{
			base.Update();

			_level?.Update();
		}

		public void RevivePlayer()
		{
			_level?.RevivePlayer();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			_level?.Draw(spriteBatch);
		}
	}
}
