using GameEngine.CameraEngine;
using GameEngine.Content;
using GameEngine.Menu.Screens;
using GameEngine.Menu.Screens.Texts;
using GameEngine.ObjectPrimitives;
using GameEngine.Options;
using Microsoft.Xna.Framework;

namespace BoxVsBlock.GamePlay
{
	public class GameScoreText : ScreenBumpText
	{
		#region Static
		public static float TextHeight => DisplayOptions.Resolution.Y / 17f;
		public static Vector2 TextPosition => new Vector2(DisplayOptions.MiddleOfScreen.X, -DisplayOptions.MiddleOfScreen.Y);
		#endregion

		public GameScoreText(Camera camera, IScreenParentObject parent)
			: base(camera, FontManager.AntigoniMed50, "0", TextPosition, TextHeight, parent)
		{
			SetOriginMultiplier(OriginPositions.Top | OriginPositions.Right);
		}

		public void ActualiseScore(int newScore)
		{
			Content = newScore.ToString();
			Bump();
		}
	}
}
