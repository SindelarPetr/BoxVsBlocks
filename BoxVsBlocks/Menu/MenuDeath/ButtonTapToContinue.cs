using BoxVsBlock.Menu.MenuGame;
using BoxVsBlock.Menu.MenuMain;
using GameEngine.CameraEngine;
using GameEngine.Content;
using GameEngine.Input.TouchPanel;
using GameEngine.MathEngine;
using GameEngine.Menu.Screens;
using GameEngine.Menu.Screens.Buttons;
using GameEngine.Options;
using Microsoft.Xna.Framework;

namespace BoxVsBlock.Menu.MenuDeath
{
	public class ButtonTapToContinue : ScreenButtonSignalizing
	{
		public ButtonTapToContinue(Camera camera, IScreenParentObject parent)
			: base(camera, Vector2.Zero, DisplayOptions.Resolution, "Tap to continue", parent)
		{
			Text.SpriteFont = FontManager.AntigoniMed50;
			Text.BasicPosition = PlayButton.TextPosition;
			Text.BasicSize = MyMath.ScaleByY(Text.BasicSize, PlayButton.TextHeight);

			OnPressed += ButtonTapToContinue_OnPressed;
		}

		private void ButtonTapToContinue_OnPressed(MyTouch e)
		{
			ScreenDeath.Instance.Hide();
			ScreenGame.Instance.Hide();
			ScreenMain.Instance.Show();
		}

		protected override float GetIndependentOpacity() => 0;
	}
}
