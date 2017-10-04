using BoxVsBlock.Menu.MenuGame;
using GameEngine.CameraEngine;
using GameEngine.Content;
using GameEngine.Input.TouchPanel;
using GameEngine.MathEngine;
using GameEngine.Menu.Screens;
using GameEngine.Menu.Screens.Buttons;
using GameEngine.Options;
using Microsoft.Xna.Framework;

namespace BoxVsBlock.Menu.MenuMain
{
	public class PlayButton : ScreenButtonSignalizing
	{
		#region Static
		public static Vector2 TextPosition => new Vector2(0, DisplayOptions.MiddleOfScreen.Y * 2 / 3f);
		public static float TextHeight => DisplayOptions.Resolution.Y / 13f;
		#endregion

		public PlayButton(Camera camera, IScreenParentObject parent) : base(camera, Vector2.Zero, DisplayOptions.Resolution, "Tap to play", parent)
		{
			Text.SpriteFont = FontManager.AntigoniMed100;
			Text.BasicPosition = TextPosition;
			Text.BasicSize = MyMath.ScaleByY(Text.BasicSize, TextHeight);

			OnPressed += PlayButton_OnPressed;
		}

		private void PlayButton_OnPressed(MyTouch e)
		{
			ScreenMain.Instance.Hide();
			ScreenGame.Instance.Show(ScreenMain.Instance);
			ScreenGame.Instance.StartNewGame(Touch);
			LooseTouches();
		}

		protected override float GetIndependentOpacity() => 0;
	}
}
