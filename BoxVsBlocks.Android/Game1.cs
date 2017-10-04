using Android.OS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using GameEngine.Options;

namespace BoxVsBlocks.Android
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		readonly GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;

		/// <summary>
		/// Constructor - the first thing which will run in this game. We will set the display options (resolution, orientations, do / dont show mouse,...)
		/// </summary>
		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);

			Content.RootDirectory = "Content";
			IsMouseVisible = false;

			Window.AllowUserResizing = true;

			_graphics.SupportedOrientations = DisplayOrientation.Portrait;
			_graphics.ApplyChanges();

			TargetElapsedTime = TimeSpan.FromTicks(166666);
			IsFixedTimeStep = true;
			_graphics.IsFullScreen = true;

			// Start listenning to event fired when the window size is resized.
			Window.ClientSizeChanged += Window_ClientSizeChanged;

			_graphics.SynchronizeWithVerticalRetrace = false;
			_graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
			_graphics.ApplyChanges();
			Window_ClientSizeChanged(this, null);
		}

		/// <summary>
		/// Method called when the window size is resized.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_ClientSizeChanged(object sender, EventArgs e)
		{

			if (DisplayOptions.Resolution.X != Window.ClientBounds.Width || DisplayOptions.Resolution.Y != Window.ClientBounds.Height)
			{
				_graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				_graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
				Main.ResolutionChanged(new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height));
				_graphics.ApplyChanges();
			}
		}

		void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			_graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
			_graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;

			e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;
		}

		protected override void Initialize()
		{
			base.Initialize();
			global::Xamarin.Forms.Forms.Init(Activity, Bundle.Empty);
			GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.One;
		}

		/// <summary>
		/// When everything is prepared, then this method is called.
		/// </summary>
		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			try
			{
				Main.Load(this, Platform.Android);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		protected override void UnloadContent()
		{

		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Main.Update(gameTime, IsActive);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(GeneralOptions.BackgroundColor);
			_spriteBatch.Begin();
			Main.Draw(_spriteBatch);
			_spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
