using GameEngine.CameraEngine;
using GameEngine.Menu.Screens;
using GameEngine.PropertiesEngine;
using Color = Microsoft.Xna.Framework.Color;

namespace BoxVsBlock.Menu.MenuMain
{
	public class ScreenMain : MenuScreen
	{
		#region Static part
		public static ScreenMain Instance { get; private set; }

		public static ScreenMain CreateInstance(Camera camera)
		{
			Instance = new ScreenMain(camera);
			return Instance;
		}

		public static MyColor MenuColor = new MyColor(new Color(0, 76, 146));
		#endregion

		public ScreenMain(Camera camera) : base(camera)
		{
			MenuObjects.Add(new PlayButton(this));
			MenuObjects.Add(new BoxVsBlockHeader(this));
			MenuObjects.Add(new Record(this));
		}
	}
}
