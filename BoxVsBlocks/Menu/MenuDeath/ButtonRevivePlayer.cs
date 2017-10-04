using BoxVsBlock.Menu.MenuGame;
using GameEngine.CameraEngine;
using GameEngine.Menu.Screens;
using GameEngine.Menu.Screens.Buttons;
using GameEngine.Options;
using GameEngine.PropertiesEngine;
using GameEngine.RunPrimitives;
using GameEngine.ValueHolders;
using Microsoft.Xna.Framework;

namespace BoxVsBlock.Menu.MenuDeath
{
	public class ButtonRevivePlayer : ScreenTextButton
	{
		#region Static
		public static Vector2 ButtonPosition => new Vector2(0, DisplayOptions.MiddleOfScreen.Y / 3);
		public static Vector2 ButtonSize => new Vector2(DisplayOptions.Resolution.X * 2f / 3, DisplayOptions.Resolution.Y / 7);
		#endregion

		private readonly ScaleHolder _boubleScale;

		public ButtonRevivePlayer(Camera camera, IScreenParentObject parent)
			: base(camera, ButtonPosition, ButtonSize, "One more life!", parent)
		{
			_boubleScale = new ScaleHolder(Vector2.One)
			{
				Friction = 1,
				BackForce = 0.01f
			};
			_boubleScale.ApplyForce(new Vector2(0.005f));

			var color = (MyColor)Color.Gold;
			color.AdjustLight(-0.1f);
			ChangeColor(color);

			OnClick += t =>
			{
				MenuScreenManager.GetScreen<ScreenDeath>().Hide();
				MenuScreenManager.GetScreen<ScreenGame>().RevivePlayer();
			};
		}

		public override Vector2 GetLocalScale()
		{
			return base.GetLocalScale() * _boubleScale.Scale;
		}

		public override void Update()
		{
			base.Update();

			_boubleScale.Update();
		}
	}
}
