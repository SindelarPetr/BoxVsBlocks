using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace BoxVsBlocks.Android
{
	[Activity(Label = "Box vs Blocks"
		, MainLauncher = true
		, Icon = "@drawable/icon"
		, Theme = "@style/Theme.Splash"
		, AlwaysRetainTaskState = true
		, LaunchMode = LaunchMode.SingleInstance
		, ScreenOrientation = ScreenOrientation.UserPortrait
		, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
	public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			Xamarin.Forms.Forms.Init(this, Bundle.Empty);
			Xamarin.Forms.DependencyService.Register<AdService>();

			base.OnCreate(bundle);


			var g = new Game1();
			var layout = new LinearLayout(this)
			{ Id = 1 };


			var gameView = (View)g.Services.GetService(typeof(View));
			layout.AddView(gameView);
			SetContentView(layout);

			g.Run();
		}
	}
}

