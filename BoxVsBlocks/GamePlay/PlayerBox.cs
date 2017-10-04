using System;
using System.Collections.Generic;
using BoxVsBlock.GamePlay.LineElements;
using GameEngine.CameraEngine;
using GameEngine.Content;
using GameEngine.Effects;
using GameEngine.Effects.Break;
using GameEngine.Effects.Explosions;
using GameEngine.Effects.Tail;
using GameEngine.GamePrimitives;
using GameEngine.MathEngine;
using GameEngine.ObjectPrimitives;
using GameEngine.Options;
using GameEngine.PropertiesEngine;
using GameEngine.RunPrimitives;
using GameEngine.ValueHolders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace BoxVsBlock.GamePlay
{
	/// <summary>
	/// This collision position is relative to Block.
	/// </summary>
	public enum CollisionPosition { Left, Right, Front, Back }

	internal class PlayerBox : GameObject
	{
		#region Constants
		public const int STARTUP_LIVES = 5;
		public const int FREQUENCY_MAX = 40;
		public const float VELOCITY_INCREASE = 0.000005f;
		public const double IMUNITY_DURATION = 3000f; // Miliseconds
		#endregion

		#region Static
		private static float PlayerPositionY => DisplayOptions.MiddleOfScreen.Y / 3;
		private static Vector2 PlayerSize => new Vector2(DisplayOptions.Resolution.X / 10f);
		#endregion

		#region Position
		/// <summary>
		/// Handles position movement when collides to a block
		/// </summary>
		private readonly PositionHolder _collisionPositionHolder;

		/// <summary>
		/// Handles movement position when receiving players input - moves player to sides
		/// </summary>
		private readonly PositionHolder _positionHolder;

		private Vector2 _previousFinalPosition;

		public override Vector2 GetLocalPosition() => base.GetLocalPosition() + _collisionPositionHolder.Position + _positionHolder.Position;
		#endregion

		#region Lives
		private int _lives;
		public int Lives
		{
			get => _lives;
			set
			{
				if (_lives == value) return;

				_tailEffect.ColorChanger.MyColor = _lives < value ? new MyColor(Color.Green) : new MyColor(Color.Red);
				_lives = value;
				_livesText.Content = value.ToString();
				_tailEffect.Frequency = Math.Min(value, 40);
				OnLifeCountChanged?.Invoke(this, value);
			}
		}
		public bool IsAlive { get; private set; }

		private readonly TextObject _livesText;
		#endregion

		#region Rotation
		private float _moveRotation;

		public override float GetLocalRotation()
		{
			return base.GetLocalRotation() + _moveRotation;
		}
		#endregion

		private readonly PlayerInput _playerInput;
		private readonly LinesHolder _linesHolder;
		private readonly SignalValueUser _imunityOpacitySignal;
		private readonly TextureSender _tailEffect;

		private Vector2 _velocity;
		private int _lastIteration;

		private bool _isImune;
		private double _imunityTime;

		public event EventHandler<PlayerBox> OnDeath;
		public event EventHandler<Block> OnFrontalCollision;
		public event EventHandler<int> OnLifeCountChanged;
		public event EventHandler<int> OnIterationDone;
		public event EventHandler OnPlayerRevived;

		public PlayerBox(Camera camera, LinesHolder linesHolder)
			: base(camera, new Vector2(0, PlayerPositionY), PlayerSize, TextureManager.Box2)
		{
			IsAlive = true;
			_linesHolder = linesHolder;
			_lives = STARTUP_LIVES;
			_livesText = new TextObject(Camera, FontManager.AntigoniMed25, _lives.ToString(), new Vector2(0, -BasicSize.Y), BasicSize.Y * 2f / 3f);
			_imunityOpacitySignal = CreateImunityOpacitySignal();
			_collisionPositionHolder = CreateCollisionPositionHolder();
			_playerInput = new PlayerInput();
			_playerInput.OnMove += playerInput_OnMove;
			_positionHolder = new PositionHolder
			{
				Friction = 0.85f
			};
			_velocity = new Vector2(0, -0.3f);
			_tailEffect = CreateTailEffect();
		}

		private SignalValueUser CreateImunityOpacitySignal()
		{
			var imunityOpacitySignal = new SignalValueUser(0.2f, 1f, 0.6f)
			{
				Friction = 0.009f,
				Value = 1
			};
			imunityOpacitySignal.StopSignalizing();
			return imunityOpacitySignal;
		}

		private PositionHolder CreateCollisionPositionHolder()
		{
			var collisionPositionHolder = new PositionHolder();
			collisionPositionHolder.BackForce = 0.03f;
			return collisionPositionHolder;
		}

		private TextureSender CreateTailEffect()
		{
			var tailEffect = new TextureSender(Camera, this, _lives, BasicSize, 0.3f);
			tailEffect.Opacity.Friction = 0.002f;
			tailEffect.ColorChanger.ResetColor(Color.DarkGray);
			tailEffect.ColorChanger.ChangeConst = 0.001f;
			return tailEffect;
		}

		private void playerInput_OnMove(object sender, Vector2 e)
		{
			_positionHolder.PrefferedPosition += new Vector2(e.X, 0);
		}

		public override float GetLocalOpacity()
		{
			return base.GetLocalOpacity() * _imunityOpacitySignal.Value;
		}

		public override void Update()
		{
			base.Update();

			_tailEffect.Update();
			if (!IsAlive) return;

			_previousFinalPosition = GetLocalPosition();
			_playerInput.Update();

			BasicPosition += new Vector2(0, _velocity.Y * (float)GeneralOptions.GameTime.ElapsedGameTime.TotalMilliseconds * (DisplayOptions.Resolution.Y / 1000f));
			UpdateIterations();

			_velocity.Y -= VELOCITY_INCREASE * (float)GeneralOptions.GameTime.ElapsedGameTime.TotalMilliseconds;

			_positionHolder.Update();
			_collisionPositionHolder.Update();
			_imunityOpacitySignal.Update();

			CheckGameBorders();

			if (!UpdateBlockCollisions())
				return;

			UpdateImunity();

			UpdateLiveCollisions();

			_livesText.Update();
			UpdateMoveRotation();

		}

		private void UpdateImunity()
		{
			if (!_isImune) return;

			_imunityTime -= GeneralOptions.GameTime.ElapsedGameTime.TotalMilliseconds;
			if (_imunityTime > 0) return;

			EndImunity();
		}

		private void EndImunity()
		{
			_isImune = false;

			_imunityOpacitySignal.StopSignalizing();
		}

		private void UpdateIterations()
		{
			int currentIteration = (int)(-BasicPosition.Y / GameLine.BlockEdgeSize);
			if (currentIteration > _lastIteration)
			{
				_lastIteration = currentIteration;
				GameManager.DrawedObjectsCount = currentIteration;
				OnIterationDone?.Invoke(0, currentIteration);
			}
		}

		private void UpdateMoveRotation()
		{
			_moveRotation = _positionHolder.ActualVelocity.X / 3f;
		}

		private void UpdateLiveCollisions()
		{
			var lives = _linesHolder.GetCollidingLives(GetLocalPosition(), BasicSize);

			foreach (var life in lives)
			{
				life.Destroy();

				Lives++;


			}
		}

		/// <summary>
		/// Checks and handles all action connected with block collisions.
		/// </summary>
		/// <returns>Whether the player is still alive.</returns>
		private bool UpdateBlockCollisions()
		{
			if (_isImune) return true;

			Vector2 finalPosition = GetLocalPosition();
			var collidingBlocks = _linesHolder.GetCollidingBlocks(_previousFinalPosition, finalPosition, BasicSize);

			if (CheckDoubleFrontalCollision(collidingBlocks))
				return !PerformFrontalCollision(GetXCloserBlock(collidingBlocks[0], collidingBlocks[1]));

			foreach (var colidingBlock in collidingBlocks)
			{
				var collisionType = GetCollisionPosition(colidingBlock);

				if (collisionType == CollisionPosition.Left)
				{
					_positionHolder.ImpactX(new Vector2(colidingBlock.BasicPosition.X - colidingBlock.BasicSize.X / 2 - BasicSize.X / 2, 0));
					continue;
				}

				if (collisionType == CollisionPosition.Right)
				{
					_positionHolder.ImpactX(new Vector2(colidingBlock.BasicPosition.X + colidingBlock.BasicSize.X / 2 + BasicSize.X / 2, 0));
					continue;
				}

				//Position = new Vector2(Position.X, colidingBlock.Position.Y + colidingBlock.BasicSize.Y / 2 + BasicSize.Y / 2);
				if (collisionType == CollisionPosition.Front && !PerformFrontalCollision(colidingBlock))
					return false; // Died
			}

			return true;
		}

		/// <summary>
		/// Chooses the block which has its X position closer to Player box.
		/// </summary>
		/// <param name="blockA">First block.</param>
		/// <param name="blockB">Second block.</param>
		/// <returns></returns>
		private Block GetXCloserBlock(Block blockA, Block blockB)
		{
			return (Math.Abs(blockA.BasicPosition.X - BasicPosition.X) < Math.Abs(blockB.BasicPosition.X - BasicPosition.X)) ? blockA : blockB;
		}

		/// <summary>
		/// Does action which occours on frontal collison like forcing box out, making damage.
		/// </summary>
		/// <param name="collisionBlock">Block with which the collision happened.</param>
		/// <returns>Whether Player is still alive.</returns>
		private bool PerformFrontalCollision(Block collisionBlock)
		{
			// Damage the block and take players lives
			if (_lives > 0)
			{
				collisionBlock.Damage();

				_collisionPositionHolder.ApplyForce(new Vector2(0, 2));

				BasicPosition = new Vector2(BasicPosition.X, collisionBlock.GetLocalPosition().Y + collisionBlock.BasicSize.Y / 2 + BasicSize.Y);

				Lives--;

				OnFrontalCollision?.Invoke(this, collisionBlock);

				return true;
			}

			// Player has not lives -> dies
			Die();
			return false;
		}

		private bool CheckDoubleFrontalCollision(List<Block> collidingBlocks)
		{
			if (collidingBlocks.Count != 2) return false;

			return collidingBlocks[0].BasicPosition.Y < BasicPosition.Y &&
				   collidingBlocks[1].BasicPosition.Y < BasicPosition.Y;
		}

		private bool IsSideCollision(CollisionPosition position)
		{
			return position == CollisionPosition.Left || position == CollisionPosition.Right;
		}

		private CollisionPosition GetCollisionPosition(Block colidingBlock)
		{
			Vector2 finalPosition = GetLocalPosition();

			// Lets determine if its side or top/bot collision
			Vector2 positionDiff = finalPosition - colidingBlock.BasicPosition;
			Vector2 parentPositionDiff = MyMath.Abs(positionDiff);
			if (!(parentPositionDiff.X > parentPositionDiff.Y))
			{
				// it is vertical collision
				if (positionDiff.Y <= 0)
					return CollisionPosition.Back;

				return CollisionPosition.Front;
			}

			//Its side collision
			if (finalPosition.X < colidingBlock.BasicPosition.X)
			{
				// Left side collision
				return CollisionPosition.Left;
			}

			// Right side collision
			return CollisionPosition.Right;
		}

		private void CheckGameBorders()
		{
			Vector2 finalPosition = GetLocalPosition();
			if (finalPosition.X + BasicSize.X / 2f > DisplayOptions.MiddleOfScreen.X)
			{
				// Too much on the right side
				_positionHolder.ImpactX(new Vector2(DisplayOptions.MiddleOfScreen.X - BasicSize.X / 2, 0));
				return;
			}

			if (finalPosition.X - BasicSize.X / 2f < -DisplayOptions.MiddleOfScreen.X)
			{
				// Too much on the right side
				_positionHolder.ImpactX(new Vector2(-DisplayOptions.MiddleOfScreen.X + BasicSize.X / 2, 0));
			}
		}

		public void Die()
		{
			IsAlive = false;
			OnDeath?.Invoke(this, this);
			MakeDeathEffects();
		}

		private void MakeDeathEffects()
		{
			EffectManager.AddEffect(new TextureBreak(Camera, GetLocalPosition(), BasicSize, null, Texture, 0.2f));
			EffectManager.AddEffect(new ExplosionBoxes(Camera, GetLocalPosition(), BasicSize.X / 3, 12, BasicSize.X * 2f));
		}

		public override void Draw(SpriteBatch spriteBatch
			)
		{
			_tailEffect.Draw(spriteBatch);

			if (IsAlive)
			{
				base.Draw(spriteBatch);

				_livesText.Draw(spriteBatch);
			}
		}

		public void Revive()
		{
			IsAlive = true;
			Lives = 20;

			StartImunity();

			OnPlayerRevived?.Invoke(this, null);
		}

		public void StartImunity()
		{
			_isImune = true;
			_imunityOpacitySignal.StartSignalizing();
			_imunityTime = IMUNITY_DURATION;
		}
	}
}