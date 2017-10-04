using GameEngine.CameraEngine;
using GameEngine.Menu.Screens;

namespace BoxVsBlock.Menu.MenuDeath
{
	public class ScreenDeath : MenuScreen
	{
		#region Static part
		public static ScreenDeath Instance { get; private set; }

		public static ScreenDeath CreateInstance(Camera camera)
		{
			Instance = new ScreenDeath(camera);
			return Instance;
		}
		#endregion

		public ScreenDeath(Camera camera) : base(camera)
		{
			MenuObjects.Add(new ButtonTapToContinue(this));
			MenuObjects.Add(new DeathSign(this));
			//MenuObjects.Add(new ButtonRevivePlayer(this));
		}
	}
}
