using System;
using System.Collections.Generic;
using System.Linq;
using BoxVsBlock.GamePlay.LineElements;
using GameEngine.CameraEngine;
using GameEngine.Effects;
using GameEngine.GamePrimitives;
using GameEngine.MathEngine;
using GameEngine.ObjectPrimitives;
using GameEngine.Options;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoxVsBlock.GamePlay
{
	/// <summary>
	/// This class takes a care about creating and handling behaviour of all lines. It generates boxes, lives and bonuses in each one and takes a 
	/// care that elements in each will be synchronized
	/// </summary>
	internal class LinesHolder : IPrimitiveElement
	{
		private readonly Camera _camera;
		private readonly EffectManager _effectManager;

		#region Constants
		public const int GAME_LINES_COUNT = 5;
		#endregion

		private GameLine[] _gameLines;

		private int _iterationsCount;
		private int _nextBlockGenIteration;
		private int _nextLiveGenIteration;

		private int _blocksInLineMax;
		private int _blocksNextIterationMinDiff;
		private int _blocksNextIterationMaxDiff;
		private int _secondBlockInLineRandom = 6;
		private int _thirdBlockInLineRandom = 6;
		private int _fourthBlockInLineRandom = 6;

		public event EventHandler<Block> OnBlockDestroyed;

		public LinesHolder(Camera camera, EffectManager effectManager)
		{
			_camera = camera;
			_effectManager = effectManager;

			_blocksInLineMax = 2;
			_blocksNextIterationMinDiff = 3;
			_blocksNextIterationMaxDiff = 5;

			SetNextBlockIteration();
			SetNextLiveDistance();
			CreateGameLines();
		}

		private void CreateGameLines()
		{
			_gameLines = new GameLine[GAME_LINES_COUNT];
			for (int i = 0; i < GAME_LINES_COUNT; i++)
			{
				_gameLines[i] = new GameLine(_camera, _effectManager, GetPositionX(i));
			}
		}

		private float GetPositionX(int lineIndex)
		{
			float lineWidth = DisplayOptions.Resolution.X / GAME_LINES_COUNT;
			return -DisplayOptions.MiddleOfScreen.X + lineWidth * lineIndex + lineWidth / 2;
		}

		private void SetNextBlockIteration()
		{
			int rand = MyMath.Random.Next(_blocksNextIterationMinDiff, _blocksNextIterationMaxDiff);
			_nextBlockGenIteration += rand;
		}

		private void SetNextLiveDistance()
		{
			int rand = MyMath.Random.Next(1, 4);
			if (rand == 3) rand = 1;
			_nextLiveGenIteration += rand;
		}

		public void Update()
		{
			foreach (var gameLine in _gameLines)
			{
				gameLine.Update();
			}
		}

		public void IterationDone()
		{
			_iterationsCount++;

			switch (_iterationsCount - 5)
			{
				case 20: _secondBlockInLineRandom = 5; break;
				case 30: _blocksNextIterationMinDiff = 2; break;
				case 40: _blocksNextIterationMaxDiff = 5; break;
				case 60: _secondBlockInLineRandom = 4; break;
				case 80: _blocksInLineMax = 3; break;
				case 100: _blocksInLineMax = 3; break;
				case 110: _secondBlockInLineRandom = 3; break;
				case 120: _blocksNextIterationMaxDiff = 4; break;
				case 130: _thirdBlockInLineRandom = 3; break;
				case 140: _fourthBlockInLineRandom = 5; break;
				case 180: _secondBlockInLineRandom = 3; break;
				case 250: _secondBlockInLineRandom = 2; break;
				case 200: _fourthBlockInLineRandom = 4; break;
				case 210: _blocksNextIterationMaxDiff = 3; break;
				case 260: _secondBlockInLineRandom = 1; break;
				case 270: _fourthBlockInLineRandom = 3; break;
				case 280: _thirdBlockInLineRandom = 2; break;
				case 290: _fourthBlockInLineRandom = 2; break;
			}

			var indexes = UpdateBlockGenerator();
			UpdateLiveGenerator(indexes);
		}

		/// <summary>
		/// Updates everything around generating blocks and creates some if its the right time.
		/// </summary>
		/// <returns>The indexes of lines where the block has been generated</returns>
		private List<int> UpdateBlockGenerator()
		{
			_nextBlockGenIteration--;
			if (_nextBlockGenIteration <= 0)
			{
				var indexes = new List<int>();
				indexes.Add(GenerateBlock(_iterationsCount));

				SetNextBlockIteration();

				// Optional 2nd block on the same line
				if (_blocksInLineMax >= 2 && MyMath.Random.Next(0, _secondBlockInLineRandom) == 0)
					indexes.Add(GenerateBlock(indexes.ToArray()));

				// Optional 3rd block on the same line
				if (_blocksInLineMax >= 3 && MyMath.Random.Next(0, _thirdBlockInLineRandom) == 0)
					indexes.Add(GenerateBlock(indexes.ToArray()));

				// Optional 4th block on the same line
				if (_blocksInLineMax >= 4 && MyMath.Random.Next(0, _fourthBlockInLineRandom) == 0)
					indexes.Add(GenerateBlock(indexes.ToArray()));


				return indexes;
			}

			return new List<int>();
		}

		/// <summary>
		/// Generates block on a random line.
		/// </summary>
		/// <returns>The index of the line where the block has been generated.</returns>
		private int GenerateBlock(params int[] ignoreIndexes)
		{
			int lineIndex = GetRandomLineIndex(ignoreIndexes);

			int lives = MyMath.Random.Next(1, 30);
			Block block = _gameLines[lineIndex].GenerateBlock(lives, _iterationsCount);
			block.OnDestroying += Block_OnDestroying;
			return lineIndex;
		}

		private void Block_OnDestroying(object sender, IGameElement e)
		{
			var block = (Block)e;
			OnBlockDestroyed?.Invoke(this, block);
			block.OnDestroying -= Block_OnDestroying;
		}

		private void UpdateLiveGenerator(List<int> indexesToIgnore)
		{
			_nextLiveGenIteration--;
			if (indexesToIgnore.Count < _gameLines.Length && _nextLiveGenIteration <= 0)
			{
				indexesToIgnore.Add(GenerateLive(_iterationsCount, indexesToIgnore.ToArray()));

				// Optional 2nd live on the same row
				if (indexesToIgnore.Count < _gameLines.Length && MyMath.Random.Next(0, 3) == 0)
				{
					// Optional 3rd live on the same row
					indexesToIgnore.Add(GenerateLive(_iterationsCount, indexesToIgnore.ToArray()));
					if (indexesToIgnore.Count < _gameLines.Length && MyMath.Random.Next(0, 3) == 0)
					{
						GenerateLive(_iterationsCount, indexesToIgnore.ToArray());
					}
				}

				SetNextLiveDistance();
			}
		}

		int GenerateLive(int iteration, params int[] ignoreIndexes)
		{
			int lineIndex = GetRandomLineIndex(ignoreIndexes);

			_gameLines[lineIndex].GenerateLive(iteration);
			return lineIndex;
		}

		/// <summary>
		/// Returns index of a line which is <b>not</b> given as a parameter
		/// </summary>
		/// <param name="linesToIgnore">Those lines will be ignored.</param>
		/// <returns>The random index of the line.</returns>
		private int GetRandomLineIndex(params int[] linesToIgnore)
		{
			int lineIndex = GetRamdomLineIndexNoSecure();
			while (linesToIgnore.Contains(lineIndex)) lineIndex = GetRamdomLineIndexNoSecure();

			return lineIndex;
		}

		private int GetRamdomLineIndexNoSecure()
		{
			return MyMath.Random.Next(0, _gameLines.Length);
		}

		public List<Block> GetCollidingBlocks(Vector2 previousRectanglePosition, Vector2 rectanglePosition, Vector2 rectangleSize)
		{
			var result = new List<Block>();
			foreach (var gameLine in _gameLines)
			{
				var block = gameLine.GetCollidingBlock(previousRectanglePosition, rectanglePosition, rectangleSize);

				if (block != null)
					result.Add(block);
			}

			return result;
		}

		public void Draw(SpriteBatch spriteBatch
			)
		{
			foreach (var gameLine in _gameLines)
			{
				gameLine.Draw(spriteBatch);
			}
		}

		public List<Live> GetCollidingLives(Vector2 rectanglePosition, Vector2 rectangleSize)
		{
			var result = new List<Live>();
			foreach (var gameLine in _gameLines)
			{
				var live = gameLine.GetCollidingLive(rectanglePosition, rectangleSize);

				if (live != null)
					result.Add(live);
			}

			return result;
		}

		public void ShakeBlocks()
		{
			foreach (var gameLine in _gameLines)
			{
				gameLine.ShakeBlocks();
			}
		}
	}
}