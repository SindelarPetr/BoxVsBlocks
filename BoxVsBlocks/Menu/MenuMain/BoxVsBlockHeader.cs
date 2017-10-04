using BoxVsBlock.TextProperties;
using GameEngine.CameraEngine;
using GameEngine.Menu.Screens;
using GameEngine.Options;
using GameEngine.PropertiesEngine;
using Microsoft.Xna.Framework;

namespace BoxVsBlock.Menu.MenuMain
{
	public class BoxVsBlockHeader : ScreenBag
	{
		#region Static
		public static Vector2 GetPosition()
		{
			return new Vector2(DisplayOptions.Resolution.X / 2, DisplayOptions.Resolution.Y / 3) - DisplayOptions.MiddleOfScreen;
		}

		public static Vector2 GetSize()
		{
			return new Vector2(DisplayOptions.Resolution.X, DisplayOptions.Resolution.Y / 3);
		}
		#endregion

		public BoxVsBlockHeader(Camera camera, IScreenParentObject parent)
			: base(camera, GetPosition(), GetSize(), parent)
		{
			AddNestedObject(CreateHeader(), 4);

			ColorChanger.ResetColor(ScreenMain.MenuColor.Color);
		}

		public override void ResolutionChanged()
		{
			BasicPosition = GetPosition();
			BasicSize = GetSize();

			base.ResolutionChanged();
		}

		private ScreenTexture CreateHeader()
		{
			var texture = new MyTexture2D(ContentProperties.BoxVsBlocksHeader);
			var height = this. BasicSize.Y * 0.65f;
			var scale = height / texture.Height;

			var header = new ScreenTexture(Camera, Vector2.Zero, new Vector2(texture.Width * scale, height), Parent, texture);
			header.ColorChanger.ResetColor(Color.White);
			return header;
		}
	}
}
