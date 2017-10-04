using GameEngine.CameraEngine;
using GameEngine.ObjectPrimitives;
using GameEngine.Options;
using Microsoft.Xna.Framework;

namespace BoxVsBlock.GamePlay
{
	internal class PlayersCamera : Camera
	{
		private BaseObject _focusedObject;

		public bool FocusPositionX { get; set; }
		public bool FocusPositionY { get; set; } = true;

		public void SetFocusObject(BaseObject baseObject)
		{
			_focusedObject = baseObject;
		}

		public override void Update()
		{
			Vector2 focusedPosition = GetFocusedPosition();
			if (FocusPositionX) SmoothViewMove.ValueToGo = new Vector2(-focusedPosition.X, SmoothViewMove.ValueToGo.Y);
			if (FocusPositionY) SmoothViewMove.ValueToGo = new Vector2(SmoothViewMove.ValueToGo.X, -focusedPosition.Y);

			base.Update();
		}

		private Vector2 GetFocusedPosition()
		{
			return _focusedObject.BasicPosition - new Vector2(0, DisplayOptions.Resolution.Y / 6f);
		}
	}
}
