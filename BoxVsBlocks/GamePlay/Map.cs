using System;
using BoxVsBlock.GamePlay.LineElements;
using BoxVsBlock.Menu;
using BoxVsBlock.Menu.MenuDeath;
using BoxVsBlock.Menu.MenuGame;
using GameEngine.Advertisement;
using GameEngine.Effects;
using GameEngine.Effects.Shapes;
using GameEngine.GamePrimitives;
using GameEngine.MathEngine;
using GameEngine.Options;
using GameEngine.PrimitiveObjects;
using GameEngine.RunPrimitives;
using GameEngine.ValueHolders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoxVsBlock.GamePlay
{
	public class Map : PrimitiveBag<IGameObject>
	{
		private readonly PlayersCamera _camera;
		private readonly LinesHolder _linesHolder;
		private readonly PlayerBox _playerBox;
		private readonly EffectManager _effectManager;

		private int _score;
		private readonly SmoothValue _darknessOpacity;
		private readonly SmoothValue _backgroundLight;

		private readonly GameScoreText _scoreText;

		public int Score
		{
			get => _score;
			set
			{
				_score = value;
				_scoreText.ActualiseScore(value);
				OnScoreChanged?.Invoke(this, value);
			}
		}

		public event EventHandler<int> OnScoreChanged;

		public Map(BasicLevel level)
		{
			_effectManager = new EffectManager();
			_camera = new PlayersCamera();
			_camera.DisableFingerControl();
			_linesHolder = new LinesHolder(_camera, _effectManager);
			_linesHolder.OnBlockDestroyed += linesHolder_OnBlockDestroyed;
			_darknessOpacity = new SmoothValue(0);
			_backgroundLight = new SmoothValue(0);

			_playerBox = GetPlayerBox();
			_playerBox.OnIterationDone += PlayerBoxOnIterationDone;
			_scoreText = new GameScoreText(_camera, MenuScreenManager.GetScreen<ScreenGame>());
		}

		private void linesHolder_OnBlockDestroyed(object sender, Block e)
		{
#if !DEBUG
			_backgroundLight.Value = 0.7f;
#endif
			_linesHolder.ShakeBlocks();
			Score += 5;
		}

		private void PlayerBoxOnIterationDone(object sender, int i)
		{
			_linesHolder.IterationDone();
		}

		private PlayerBox GetPlayerBox()
		{
			var playerBox = new PlayerBox(, _effectManager, _linesHolder);
			playerBox.OnFrontalCollision += playerBox_OnFrontalCollision;
			playerBox.OnDeath += playerBox_OnDeath;
			playerBox.OnPlayerRevived += PlayerBox_OnPlayerRevived;
			_camera.SetFocusObject(playerBox);
			return playerBox;
		}

		private void PlayerBox_OnPlayerRevived(object sender, EventArgs e)
		{
			_darknessOpacity.ValueToGo = 0;
		}

		private void playerBox_OnDeath(object sender, PlayerBox e)
		{
			ActualiseHighestScore();

			ScreenDeath.Instance.Show(null);
			_darknessOpacity.ValueToGo = 0.75f;

			// working - "ca-app-pub-6639044173799596/7932523865"
			if (GeneralOptions.Platform == Platform.Android)
				AdServiceProvider.ShowInterstitial("ca-app-pub-6639044173799596/7859043805");
			//_linesHolder.Stop();
		}

		private void ActualiseHighestScore()
		{
			Options.BestScore = Score;
			Options.Save();
		}

		private void playerBox_OnFrontalCollision(object sender, Block e)
		{
			//_linesHolder.BumpSpeed();
			_camera.ShakeByRotation(MyMath.GetRandomMarkNotNull() * 0.02f);
			Score++;
		}

		public void Update()
		{
			_camera.Update();
			_playerBox.Update();
			_linesHolder.Update();
			_darknessOpacity.Update();
			_effectManager.Update();
			_backgroundLight.Update();
			_scoreText.Update();
		}

		public void Draw(SpriteBatch spriteBatch
			)
		{
			ShapeDrawing.DrawDisplayCover(spriteBatch, _backgroundLight.Value, Color.White);

			_linesHolder.Draw(spriteBatch);
			_playerBox.Draw(spriteBatch);
			_effectManager.Draw(spriteBatch);

			// Draw horizontal line in players start position height
			//ShapeDrawing.DrawLine(spriteBatch, new Vector2(-DisplayOptions.MiddleOfScreen.X, playerStartHeight), new Vector2(DisplayOptions.MiddleOfScreen.X, playerStartHeight));
			_scoreText.Draw(spriteBatch);

			ShapeDrawing.DrawDisplayCover(spriteBatch, _darknessOpacity.Value, Color.Black);
		}

		public void RevivePlayer()
		{
			_playerBox.Revive();
		}
	}
}
