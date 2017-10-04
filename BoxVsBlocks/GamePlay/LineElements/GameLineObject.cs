using GameEngine.GamePrimitives;
using GameEngine.ObjectPrimitives;
using GameEngine.Options;
using GameEngine.PropertiesEngine;
using Microsoft.Xna.Framework;

namespace BoxVsBlock.GamePlay.LineElements
{
	internal class GameLineObject : GameObject
	{
		public GameLineObject(Level level, Vector2 position, Vector2 size, IWorldObject parent = null, MyTexture2D texture = null)
			: base(level, position, size, parent, texture) { }

		public override void Update()
		{
			base.Update();

			CheckPositionIsInScreen();
		}

		/// <summary>
		/// Moves box down according to given speed.
		/// </summary>
		/// <param name="speed">The speed with which box should move down.</param>
		public void UpdatePosition(float speed)
		{
			BasicPosition += new Vector2(0, speed * (float)GeneralOptions.GameTime.ElapsedGameTime.TotalMilliseconds);
		}

		private void CheckPositionIsInScreen()
		{
			var position = GetAbsolutePosition().Y;
			if (position < DisplayOptions.Resolution.Y * 4f / 3f) return;

			// Block is out of screen
			;
		}
	}
}