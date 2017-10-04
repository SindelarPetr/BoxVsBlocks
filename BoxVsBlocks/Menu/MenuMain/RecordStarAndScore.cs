using GameEngine.CameraEngine;
using GameEngine.Content;
using GameEngine.Menu.Screens;
using GameEngine.Menu.Screens.Texts;
using Microsoft.Xna.Framework;

namespace BoxVsBlock.Menu.MenuMain
{
	public class RecordStarAndScore : ScreenBag
	{
		#region Constants
		private const float STAR_AND_SCORE_DISTANCE = 0.3f;
		#endregion

		private readonly RecordStar _star;
		private readonly ScreenText _score;

		public RecordStarAndScore(Camera camera, Vector2 position, float height, IScreenParentObject parent) : base(camera, position, Vector2.Zero, parent)
		{
			_star = new RecordStar(Camera, new Vector2(-height * 5f * STAR_AND_SCORE_DISTANCE / 2f, 0), height, this);
			AddNestedObject(_star, 4);

			_score = CreateScore(height);
		}

		/// <summary>
		/// Creates text which shows the best score.
		/// </summary>
		/// <param name="height">Height of this RecordStarAndScore element.</param>
		/// <returns></returns>
		private ScreenText CreateScore(float height)
		{
			var text = new ScreenText(Camera, FontManager.AntigoniMed25Bold, Options.BestScore.ToString(),
				new Vector2(height * 5f * STAR_AND_SCORE_DISTANCE / 2f, 0), height, this);
			text.ColorChanger.ResetColor(Color.White);
			AddNestedObject(text, 4);
			return text;
		}

		public void ActualiseScore()
		{
			_score.Content = Options.BestScore.ToString();
		}

		public override void Show(IScreenObject showInitializator = null)
		{
			base.Show(showInitializator);

			ActualiseScore();
		}
	}
}
