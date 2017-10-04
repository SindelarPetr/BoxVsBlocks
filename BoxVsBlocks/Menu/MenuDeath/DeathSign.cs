using BoxVsBlock.Menu.MenuGame;
using BoxVsBlock.Menu.MenuMain;
using GameEngine.CameraEngine;
using GameEngine.Content;
using GameEngine.Menu.Screens;
using GameEngine.Menu.Screens.Texts;
using GameEngine.RunPrimitives;
using Microsoft.Xna.Framework;

namespace BoxVsBlock.Menu.MenuDeath
{
	public class DeathSign : ScreenBag
	{
		#region Static
		public static Vector2 DeathSignSize =>  new Vector2(BoxVsBlockHeader.GetSize().X * 2f / 3f, BoxVsBlockHeader.GetSize().Y);
		public static float RecordStarAndScoreHeight => DeathSignSize.Y / 6f;
		public static float ScoreTextHeight => DeathSignSize.Y / 4f;
		#endregion

		private Vector2 ScoreTextPosition => new Vector2(0, -DeathSignSize.Y / 7f);
		private Vector2 RecorStarAndScorePosition => new Vector2(0, DeathSignSize.Y / 3.5f);

		private readonly ScreenText _scoreText;
		public DeathSign(Camera camera, IScreenParentObject parent) : base(camera, BoxVsBlockHeader.GetPosition(), DeathSignSize, parent)
		{
			ColorChanger.ResetColor(ScreenMain.MenuColor.Color);

			AddNestedObject(new RecordStarAndScore(camera, RecorStarAndScorePosition, RecordStarAndScoreHeight, this), 4);

			_scoreText = new ScreenText(Camera, FontManager.AntigoniMed100, "0", ScoreTextPosition, null, this);
			AddNestedObject(_scoreText, 4);
		}

		private string GetScoreText()
		{
			return MenuScreenManager.GetScreen<ScreenGame>().Score.ToString();
		}

		public override void Show(IScreenObject showInitializator = null)
		{
			base.Show(showInitializator);
			
			_scoreText.Content = GetScoreText();
		}
	}
}
