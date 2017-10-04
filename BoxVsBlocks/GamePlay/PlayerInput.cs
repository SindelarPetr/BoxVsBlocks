using System;
using GameEngine.Input;
using GameEngine.Input.TouchPanel;
using Microsoft.Xna.Framework;

namespace BoxVsBlock.GamePlay
{
	public class PlayerInput
	{
		private MyTouch _touch;

		/// <summary>
		/// Is fired each loop when player has his finger on the screen. As a parameter passes the move of the touch.
		/// </summary>
		public event EventHandler<Vector2> OnMove;

		public void Update()
		{
			if (_touch != null && !MyTouch.IsTouchFresh(_touch))
			{
				_touch.SetAsNotOwned();
				_touch = null;
			}

			if (_touch == null)
				_touch = InputOptions.MyState.GetBrandNewTouch();

			if (_touch == null) return;

			OnMove?.Invoke(this, _touch.Move);
		}
	}
}
