//using GameEngine.CameraEngine;

using GameEngine.CameraEngine;
using GameEngine.Content;
using GameEngine.Effects.Break;
using GameEngine.MathEngine;
using GameEngine.ObjectPrimitives;
using GameEngine.ValueHolders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace BoxVsBlock.GamePlay.LineElements
{
	internal class Block : GameLineObject
	{
		#region Static
		private static readonly Color[] Colors;
		static Block()
		{
			Colors = new[]
			{
				Color.Blue,
				Color.Red,
				Color.DarkViolet,
				Color.Yellow
			};
		}

		private static Color GetRandomColor()
		{
			return Colors[MyMath.Random.Next(0, Colors.Length)];
		}
		#endregion

		public int Lives
		{
			get => _lives;
			set
			{
				_lives = value;
				_livesText.Content = _lives.ToString();
			}
		}

		private readonly TextObject _livesText;
		private int _lives;

		private readonly ScaleHolder _boubler;
		private readonly Color _damageColor;
		private readonly PositionHolder _shakePosition;

		public Block(Camera camera, IWorldObject parent, int lives, float linePositionX, int iteration)
			: base(camera, GameLine.GetPositionInLine(linePositionX, iteration), GameLine.BlockSize, parent, TextureManager.Box2)
		{
			_lives = lives;
			_livesText = new TextObject(camera, FontManager.AntigoniMed50, _lives.ToString(), Vector2.Zero, GameLine.BlockEdgeSize * 2f / 3f);
			_livesText.ColorChanger.ResetColor(Color.Black);
			_damageColor = GetRandomColor();
			_shakePosition = new PositionHolder(Vector2.Zero)
			{
				Friction = 0.90f,
				BackForce = 0.030f
			};

			_boubler = new ScaleHolder(Vector2.One)
			{
				Friction = 0.90f,
				BackForce = 0.035f,
			};
		}

		public override Vector2 GetLocalScale()
		{
			return base.GetLocalScale() * _boubler.Scale;
		}

		public override Vector2 GetLocalPosition()
		{
			return base.GetLocalPosition() + _shakePosition.Position;
		}

		/// <summary>
		/// Decreases lifes of the block.
		/// </summary>
		/// <returns>Whether the block has been destroyed.</returns>
		public bool Damage()
		{
			Lives -= 1;
			_boubler.ApplyForce(new Vector2(0.05f, 0));
			ColorChanger.MyColor.Change(_damageColor);
			if (Lives != 0) return false;

			// Out of lives
			Destroy();
			return true;
		}

		public void Shake()
		{
			_shakePosition.ApplyForce(MyMath.RotatePoint((float)MyMath.Random.Next(1000, 2000) / 1000f + 1f, MyMath.GetRandomAngle()));
		}

		public override void Destroy()
		{
			TextureBreak breakEffect = new TextureBreak(Level, GetLocalPosition(), BasicSize, Texture);
			Level.AddEffect(breakEffect);

			base.Destroy();
		}

		public override void Update()
		{
			base.Update();

			_livesText.Update();
			_boubler.Update();
			_shakePosition.Update();
		}

		public override void Draw(SpriteBatch spriteBatch
			)
		{
			base.Draw(spriteBatch);

			_livesText.Draw(spriteBatch);
		}
	}
}