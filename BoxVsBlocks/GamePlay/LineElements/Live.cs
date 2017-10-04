using GameEngine.Content;
using GameEngine.Effects;
using GameEngine.Effects.Explosions;
using GameEngine.ObjectPrimitives;
using GameEngine.Options;
using GameEngine.ValueHolders;
using Microsoft.Xna.Framework;

namespace BoxVsBlock.GamePlay.LineElements
{
	internal class Live : GameLineObject
	{
		#region Static
		public static Vector2 LiveSize => new Vector2(DisplayOptions.Resolution.X / 20f);
		#endregion

		private readonly ValueHolder _scaleEffect;

		public Live(Level level, IWorldObject parent, float linePositionX, int iteration)
			: base(level, GameLine.GetPositionInLine(linePositionX, iteration), LiveSize, parent, TextureManager.Box2)
		{
			_scaleEffect = new ValueHolder(1)
			{
				Friction = 0f,
			};

			_scaleEffect.ApplyForce(1f);
		}

		public override Vector2 GetLocalScale()
		{
			return base.GetLocalScale() * _scaleEffect.Value;
		}

		public override void Update()
		{
			base.Update();

			_scaleEffect.Update();
		}

		public override void Destroy()
		{
			base.Destroy();

			EffectManager.AddEffect(new ExplosionBoxes(Camera, BasicPosition, BasicSize.X / 2f, 8, BasicSize.X * 8));
		}
	}
}