using System.Collections.Generic;
using System.Diagnostics;
using BoxVsBlock.GamePlay.LineElements;
using GameEngine.CameraEngine;
using GameEngine.Effects;
using GameEngine.GamePrimitives;
using GameEngine.MathEngine;
using GameEngine.Options;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoxVsBlock.GamePlay
{
	internal sealed class GameLine
	{

		#region Static
		public static float BlockEdgeSize => DisplayOptions.Resolution.X / 5;
		public static Vector2 BlockSize => new Vector2(BlockEdgeSize);
		public static Vector2 GetPositionInLine(float positionX, int iteration) => new Vector2(positionX, -iteration * BlockEdgeSize - DisplayOptions.Resolution.Y);
		#endregion

		private readonly Camera _camera;
		private readonly float _linePositionX;
		private readonly List<Block> _blocks;
		private readonly List<Live> _lives;
		private readonly EffectManager _effectManager;

		public GameLine(Camera camera, EffectManager effectManager, float linePositionX)
		{
			_camera = camera;
			_effectManager = effectManager;
			_linePositionX = linePositionX;
			_blocks = new List<Block>();
			_lives = new List<Live>();
		}

		public Block GenerateBlock(int lives, int iteration)
		{
			Debug.Assert(lives >= 1);

			var block = new Block(_camera, _effectManager, lives, _linePositionX, iteration);
			block.OnRemoving += BlockOnRemoving;
			_blocks.Add(block);
			return block;
		}

		private void BlockOnRemoving(object sender, IGameElement e)
		{
			Block block = (Block)e;
			_blocks.Remove(block);
			e.OnRemoving -= BlockOnRemoving;
		}

		public void GenerateLive(int iteration)
		{
			var life = new Live(_camera, _effectManager, _linePositionX, iteration);
			life.OnRemoving += LifeOnRemoving;

			_lives.Add(life);
		}

		private void LifeOnRemoving(object sender, IGameElement gameObject)
		{
			Live life = (Live)gameObject;
			_lives.Remove(life);
			life.OnRemoving -= LifeOnRemoving;
		}

		public void Update()
		{
			for (var i = _blocks.Count - 1; i >= 0; i--)
			{
				Block block = _blocks[i];
				block.Update();
			}

			for (var i = _lives.Count - 1; i >= 0; i--)
			{
				Live live = _lives[i];
				live.Update();
			}
		}

		public void Draw(SpriteBatch spriteBatch
			)
		{
			foreach (var block in _blocks) block.Draw(spriteBatch);
			foreach (var live in _lives) live.Draw(spriteBatch);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rectanglePosition"></param>
		/// <param name="rectangleSize"></param>
		/// <returns></returns>
		public Block GetCollidingBlock(Vector2 previousRectanglePosition, Vector2 rectanglePosition, Vector2 rectangleSize)
		{
			Block collidingBlock = null;
			for (int i = _blocks.Count - 1; i >= 0; i--)
			{
				Block block = _blocks[i];

				//bool rightTopCorner = MyMath.CollisionLineAndRectangle(
				//	previousRectanglePosition + new Vector2(rectangleSize.X, -rectangleSize.Y) / 2,
				//	rectanglePosition + new Vector2(rectangleSize.X, -rectangleSize.Y) / 2, block.Position, block.BasicSize);

				//bool rightBotCorner = MyMath.CollisionLineAndRectangle(
				//	previousRectanglePosition + new Vector2(rectangleSize.X, rectangleSize.Y) / 2,
				//	rectanglePosition + new Vector2(rectangleSize.X, rectangleSize.Y) / 2, block.Position, block.BasicSize);

				//bool leftTopCorner = MyMath.CollisionLineAndRectangle(
				//	previousRectanglePosition + new Vector2(-rectangleSize.X, -rectangleSize.Y) / 2,
				//	rectanglePosition + new Vector2(-rectangleSize.X, -rectangleSize.Y) / 2, block.Position, block.BasicSize);

				//bool leftBotCorner = MyMath.CollisionLineAndRectangle(
				//	previousRectanglePosition + new Vector2(-rectangleSize.X, rectangleSize.Y) / 2,
				//	rectanglePosition + new Vector2(-rectangleSize.X, rectangleSize.Y) / 2, block.Position, block.BasicSize);

				bool rectangles = MyMath.CollisionRectangleAndRectangle(rectanglePosition, rectangleSize, block.BasicPosition,
					block.BasicSize);

				if (//!rightBotCorner && !rightTopCorner && !leftBotCorner && !leftTopCorner &&  
					!rectangles)
					continue;

				// They collide
				if (collidingBlock != null) return block;

				collidingBlock = block;
			}

			return collidingBlock;
		}

		public Live GetCollidingLive(Vector2 rectanglePosition, Vector2 rectangleSize)
		{
			foreach (var life in _lives)
			{
				if (MyMath.CollisionRectangleAndRectangle(life.BasicPosition, life.BasicSize, rectanglePosition, rectangleSize))
					return life;
			}

			return null;
		}

		public void ShakeBlocks()
		{
			foreach (var block in _blocks)
			{
				block.Shake();
			}
		}
	}
}