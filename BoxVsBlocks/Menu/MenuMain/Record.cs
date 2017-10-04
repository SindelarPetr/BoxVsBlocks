using GameEngine.CameraEngine;
using GameEngine.Menu.Screens;
using GameEngine.Options;
using Microsoft.Xna.Framework;

namespace BoxVsBlock.Menu.MenuMain
{
	public sealed class Record : ScreenBag
	{
		private const float SIZE_CONST = 0.06f;

		#region Static
		public static Vector2 GetPosition()
		{
			return new Vector2(DisplayOptions.Resolution.X / 2, DisplayOptions.Resolution.Y * 0.6f) - DisplayOptions.MiddleOfScreen;
		}

		public static Vector2 GetSize()
		{
			return new Vector2(DisplayOptions.Resolution.X, DisplayOptions.Resolution.Y / 15);
		}

		public static Vector2 GetStarAndTextPosition()
		{
			Vector2 recordPosition = GetPosition();
			return new Vector2(recordPosition.X - DisplayOptions.Resolution.X * SIZE_CONST, recordPosition.Y);
		}
		#endregion

		private readonly RecordStarAndScore _recordStarAndScore;

		public Record(Camera camera, IScreenParentObject parent = null)
			: base(camera, GetPosition(), GetSize(), parent)
		{
			ColorChanger.ResetColor(ScreenMain.MenuColor.Color);
			_recordStarAndScore = new RecordStarAndScore(camera, Vector2.Zero, BasicSize.Y * 0.6f, this);
			AddNestedObject(_recordStarAndScore, 4);
		}

		public override void Show(IScreenObject showInitializator = null)
		{
			base.Show(showInitializator);

			_recordStarAndScore.ActualiseScore();
		}
	}
}
